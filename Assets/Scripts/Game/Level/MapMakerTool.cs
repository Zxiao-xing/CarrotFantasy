using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapMakerTool : MonoSingleton<MapMakerTool>
{
    private class ItemInfoListNode
    {
        public GameObject Go;
        public int NextItemId;
        public int PreItemId;
    }

    public GameObject m_camera;

    /*地图数据*/
    [HideInInspector]
    public float m_mapHeight;
    [HideInInspector]
    public float m_mapWidth;
    [HideInInspector]
    public float m_gridHeight;
    [HideInInspector]
    public float m_gridWidth;
    [HideInInspector]
    public const int m_column = 12;
    [HideInInspector]
    public const int m_row = 8;

    // 需要保存的信息
    //[SerializeField]
    public int m_curLevelGroupId;              // 当前关卡组 id
    public int m_curLevelId;                   // 当前关卡 id
    public List<GridPosIndex> m_monsterPathList { get; private set; } = new List<GridPosIndex>();           // 怪物路径
    public List<Round.RoundInfo> m_roundInfo = new List<Round.RoundInfo>();                                 // 每轮怪物
    public GridTool[,] m_gridArr = new GridTool[m_column, m_row];                                           // 格子信息

    // 显示所需
    [SerializeField]
    GameObject gridTool;

    // 路和背景图
    Sprite m_roadSprite;
    Sprite m_bgSprite;
    SpriteRenderer m_bgSpriteRenderer;
    SpriteRenderer m_roadSpriteRenderer;


    [HideInInspector]
    public Sprite m_gridSprite, m_gridStartSprite, m_gridCantBuildSprite;       //点击格子后显示格子三种状态资源

    [HideInInspector]
    private Dictionary<int, ItemInfoListNode> m_itemInfoListDict;

    // 路径编辑相关
    private bool m_isEditMonsterPath = false;             // 是否正在编辑怪物路径
    private List<GridPosIndex> m_editMosterPathList = new List<GridPosIndex>();             // 当前编辑的临时路径
    private GridPosIndex m_lastMousePos;                    // 鼠标上一个指向的位置
    private bool m_canBePath;                               // 可以成为怪物路径点
    private Sprite m_startBoard;                            // 产怪起点的路牌图片
    private Sprite m_endCarrot;                             // 终点的萝卜图片
    private Sprite m_monsterPosSprite;                      // 中间路径点的怪物图片

    // Start is called before the first frame update
    void Start()
    {
        // 若存在 GameController 说明是走正常游戏流程进来的，就不是走的工具流程，此时移除掉该物体
        if (GameController.HasInstance())
        {
            Destroy(this.gameObject);
            return;
        }

        // 加载所需数据
        PlayerManager.CreateInstance();

        // 单独启动这个场景需要启动相机
        m_camera.SetActive(true);

        m_bgSpriteRenderer = transform.Find("BG").GetComponent<SpriteRenderer>();
        m_roadSpriteRenderer = transform.Find("Road").GetComponent<SpriteRenderer>();

        CalculateSize();

        LoadGridRes();

        InitMap();
    }

    private void Update()
    {
        ShowEditingPath();
        HandleKeyInput();
    }

    private void OnDrawGizmos()
    {
        if (m_camera != null && m_camera.activeInHierarchy == false)
        {
            m_camera.SetActive(true);
        }
        CalculateSize();
        Gizmos.color = Color.red;
        //画行
        for (int y = 0; y <= m_row; y++)
        {
            Vector3 startPos = new Vector3(-m_mapWidth / 2, -m_mapHeight / 2 + y * m_gridHeight);
            Vector3 endPos = new Vector3(m_mapWidth / 2, -m_mapHeight / 2 + y * m_gridHeight);
            Gizmos.DrawLine(startPos, endPos);
        }
        //画列
        for (int x = 0; x <= m_column; x++)
        {
            Vector3 startPos = new Vector3(-m_mapWidth / 2 + m_gridWidth * x, m_mapHeight / 2);
            Vector3 endPos = new Vector3(-m_mapWidth / 2 + x * m_gridWidth, -m_mapHeight / 2);
            Gizmos.DrawLine(startPos, endPos);
        }
        m_bgSpriteRenderer = transform.Find("BG").GetComponent<SpriteRenderer>();
        m_roadSpriteRenderer = transform.Find("Road").GetComponent<SpriteRenderer>();
        AdaptGridObjSize(m_bgSpriteRenderer, m_column, m_row);
        AdaptGridObjSize(m_roadSpriteRenderer, m_column, m_row - 2);
    }

    // 编辑路径时候的显示
    private void ShowEditingPath()
    {
        // 当正在编辑路径
        if (m_isEditMonsterPath)
        {
            // 非路径起点
            if(m_editMosterPathList.Count > 0)
            {
                // 取消上次显示的路径
                HideLastMousePath();
                // 显示当前的路径
                ShowCurMousePath();
            }

            // 刷新当前已经确定的路径
            ShowCurMonsterPath();
        }
    }

    // 根据是否是编辑路径状态来显示对应路径
    private void ShowCurMonsterPath()
    {
        List<GridPosIndex> gridIndexList;

        if (m_isEditMonsterPath)
        {
            gridIndexList = m_editMosterPathList;
        }
        else
        {
            gridIndexList = m_monsterPathList;
        }

        // 设置当前路径状态，这个可以优化，当前是全量形式，可以改为增量
        for (int i = 0; i < gridIndexList.Count; i++)
        {
            m_gridArr[gridIndexList[i].xIndex, gridIndexList[i].yIndex].m_state.canBuild = false;
            m_gridArr[gridIndexList[i].xIndex, gridIndexList[i].yIndex].m_state.isMonsterPoint = true;
            // 起点显示路牌，中间显示怪物图片，终点显示萝卜
            if(i == 0)
            {
                m_gridArr[gridIndexList[i].xIndex, gridIndexList[i].yIndex].SetSprite(m_startBoard, true);
            }
            if (i != 0)
            {
                if(i == gridIndexList.Count - 1)
                {
                    m_gridArr[gridIndexList[i].xIndex, gridIndexList[i].yIndex].SetSprite(m_endCarrot, true);
                }
                else
                {
                    m_gridArr[gridIndexList[i].xIndex, gridIndexList[i].yIndex].SetSprite(m_monsterPosSprite, true);
                }
                // 设置路上可以 build 的图案
                if (gridIndexList[i].xIndex == gridIndexList[i - 1].xIndex)
                {
                    int offset = gridIndexList[i].yIndex -  gridIndexList[i - 1].yIndex;
                    offset /= Mathf.Abs(offset);
                    for (int j = gridIndexList[i - 1].yIndex + offset; j != gridIndexList[i].yIndex; j += offset)
                    {
                        GridState state = m_gridArr[gridIndexList[i].xIndex, j].m_state;
                        state.canBuild = false;
                        m_gridArr[gridIndexList[i].xIndex, j].UpdateState(state);
                    }
                }
                else
                {
                    int offset = gridIndexList[i].xIndex - gridIndexList[i - 1].xIndex;
                    offset /= Mathf.Abs(offset);
                    for (int j = gridIndexList[i - 1].xIndex + offset; j != gridIndexList[i].xIndex; j += offset)
                    {
                        GridState state = m_gridArr[j, gridIndexList[i].yIndex].m_state;
                        state.canBuild = false;
                        m_gridArr[j, gridIndexList[i].yIndex].UpdateState(state);
                    }
                }
            }

        }
    }

    // 根据是否是编辑路径状态来清除非当前显示的路径
    private void ClearLastMonsterPath()
    {
        List<GridPosIndex> lastGridIndexList;
        if (m_isEditMonsterPath)
        {
            lastGridIndexList = m_monsterPathList;
        }
        else
        {
            lastGridIndexList = m_editMosterPathList;
        }
        // 移除上一个的
        for (int i = 0; i < lastGridIndexList.Count; i++)
        {
            m_gridArr[lastGridIndexList[i].xIndex, lastGridIndexList[i].yIndex].SetSprite(null, false);
            m_gridArr[lastGridIndexList[i].xIndex, lastGridIndexList[i].yIndex].m_state.canBuild = true;
            m_gridArr[lastGridIndexList[i].xIndex, lastGridIndexList[i].yIndex].m_state.isMonsterPoint = false;
            // 把路上 cantbuild 的图案清掉
            if (i != 0)
            {
                if (lastGridIndexList[i].xIndex == lastGridIndexList[i - 1].xIndex)
                {
                    int offset = lastGridIndexList[i].yIndex - lastGridIndexList[i - 1].yIndex;
                    offset /= Mathf.Abs(offset);
                    for (int j = lastGridIndexList[i - 1].yIndex + offset; j != lastGridIndexList[i].yIndex; j += offset)
                    {
                        GridState state = m_gridArr[lastGridIndexList[i].xIndex, j].m_state;
                        state.canBuild = true;
                        m_gridArr[lastGridIndexList[i].xIndex, j].UpdateState(state);
                    }
                }
                else
                {
                    int offset = lastGridIndexList[i].xIndex - lastGridIndexList[i - 1].xIndex;
                    offset /= Mathf.Abs(offset);
                    for (int j = lastGridIndexList[i - 1].xIndex + offset; j != lastGridIndexList[i].xIndex; j += offset)
                    {
                        GridState state = m_gridArr[j, lastGridIndexList[i].yIndex].m_state;
                        state.canBuild = true;
                        m_gridArr[j, lastGridIndexList[i].yIndex].UpdateState(state);
                    }
                }
            }
        }
    }

    // 当前鼠标位置和上一个已确定路径的显示
    private void ShowCurMousePath()
    {
        Vector3 vec = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit2D = Physics2D.Raycast(new Vector2(vec.x, vec.y), new Vector3(vec.x, vec.y, 0.1f));
        if (hit2D.collider != null)
        {
            GridTool gridTool = hit2D.collider.gameObject.GetComponent<GridTool>();

            // 如果命中的是 gridTool
            if (gridTool != null)
            {
                GridPosIndex lastPos = m_editMosterPathList[m_editMosterPathList.Count - 1];

                int xOffset = gridTool.pos.xIndex - lastPos.xIndex;
                int yOffset = gridTool.pos.yIndex - lastPos.yIndex;

                // 是否允许修路
                m_canBePath = true;

                // 修路条件：1.在同一条直线上；2.路上没有其他 item；3.路上都是 canbuild
                int offset = 0;
                if (xOffset != 0 && yOffset == 0)
                {
                    offset = xOffset / Mathf.Abs(xOffset);
                    int j = gridTool.pos.yIndex;
                    for (int i = lastPos.xIndex + offset; i != gridTool.pos.xIndex + offset; i += offset)
                    {
                        if (m_gridArr[i, j].m_state.hasItem || m_gridArr[i, j].m_state.canBuild == false)
                        {
                            m_canBePath = false;
                            break;
                        }
                    }

                    // 给路径上的格子都显示格子sprite，以显示将要设置的路径
                    if (m_canBePath)
                    {
                        for (int i = lastPos.xIndex + offset; i != gridTool.pos.xIndex + offset; i += offset)
                        {
                            m_gridArr[i, j].SetSprite(m_gridSprite, true);
                        }
                    }
                }
                else if (yOffset != 0 && xOffset == 0)
                {
                    offset = yOffset / Mathf.Abs(yOffset);
                    int j = gridTool.pos.xIndex;
                    for (int i = lastPos.yIndex + offset; i != gridTool.pos.yIndex + offset; i += offset)
                    {
                        if (m_gridArr[j, i].m_state.hasItem || m_gridArr[j, i].m_state.canBuild == false)
                        {
                            m_canBePath = false;
                            break;
                        }
                    }

                    if (m_canBePath)
                    {
                        for (int i = lastPos.yIndex + offset; i != gridTool.pos.yIndex + offset; i += offset)
                        {
                            m_gridArr[j, i].SetSprite(m_gridSprite, true);
                        }
                    }
                }
                else
                {
                    // 不是直线
                    m_canBePath = false;
                }

                if (m_canBePath)
                {
                    m_lastMousePos = gridTool.pos;
                }
                else
                {
                    m_lastMousePos = m_editMosterPathList[m_editMosterPathList.Count - 1];
                }
            }

        }
    }

    // 隐藏上一次鼠标位置显示的路径
    private void HideLastMousePath()
    {
        GridPosIndex lastPos = m_editMosterPathList[m_editMosterPathList.Count - 1];

        int xOffset = m_lastMousePos.xIndex - lastPos.xIndex;
        int yOffset = m_lastMousePos.yIndex - lastPos.yIndex;
        // 当上一次鼠标位置是一条直线时，需要清理上一次的显示，清理的时候不包括端点
        if (xOffset != 0 && yOffset == 0)
        {
            int offset = xOffset / Mathf.Abs(xOffset);
            int j = m_lastMousePos.yIndex;
            for (int i = lastPos.xIndex + offset; i != m_lastMousePos.xIndex + offset; i += offset)
            {
                m_gridArr[i, j].SetSprite(null, false);
            }
        }
        else if (xOffset == 0 && yOffset != 0)
        {
            int offset = yOffset / Mathf.Abs(yOffset);
            int j = m_lastMousePos.xIndex;
            for (int i = lastPos.yIndex + offset; i != m_lastMousePos.yIndex + offset; i += offset)
            {
                m_gridArr[j, i].SetSprite(null, false);
            }
        }
    }

    // 处理按键输入
    private void HandleKeyInput()
    {
        if (Input.anyKeyDown)
        {
            GameObject hitGo = null;
            Vector3 vec = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit2D = Physics2D.Raycast(new Vector2(vec.x, vec.y), new Vector3(vec.x, vec.y, 0.1f));
            if (hit2D.collider != null)
            {
                hitGo = hit2D.collider.gameObject;
            }
            if(hitGo != null)
            {
                Item item = hitGo.GetComponent<Item>();
                if(item != null)
                {
                    // 对 Item 进行得处理
                    HandleItem(item);
                }
                else
                {
                    // 对 Grid进行处理
                    HandleGridTool(hitGo);
                }
            }
        }
    }

    /// 对 Item 按键的处理
    // 按 D：删除 Item
    // 按 W：下一个 Item
    // 按 S：上一个 Item
    private void HandleItem(Item item)
    {
        // 删除格子上的 item
        if (Input.GetKeyDown(KeyCode.D))
        {
            item.m_gridTool.m_state.hasItem = false;
            Destroy(item.gameObject);
            return;
        }
        else if (Input.GetKeyDown(KeyCode.W))       // 下一个 item
        {
            //item.m_gridTool.m_state.itemID = ;
            item.m_gridTool.m_state.itemID = m_itemInfoListDict[item.ID].NextItemId;
            item.m_gridTool.UpdateState(item.m_gridTool.m_state);
            return;
        }
        else if (Input.GetKeyDown(KeyCode.S))       // 上一个 item
        {
            item.m_gridTool.m_state.itemID = m_itemInfoListDict[item.ID].PreItemId;
            item.m_gridTool.UpdateState(item.m_gridTool.m_state);
            return;
        }
    }

    /// 对 Grid Tool 按键的处理
    /// 若是在编辑怪物路径：
    // 按 Q：退出编辑
    // 按 空格：确认编辑
    // 按 鼠标左键：添加路径
    // 按 鼠标右键：删除路径（必须是上一个添加的路径）

    /// 若不是编辑怪物路径：
    // 按 A：添加 Item
    // 按 M：编辑怪物路径
    // 按 鼠标左键：更改该格子是否能建造的状态
    private void HandleGridTool(GameObject hitGo)
    {
            GridTool gridTool = hitGo.GetComponent<GridTool>();
            if (gridTool != null)
            {
                // 如果处于路径编辑状态
                if (m_isEditMonsterPath)
                {
                    if (Input.GetKeyDown(KeyCode.Q))            // 退出路径编辑状态
                    {
                        m_isEditMonsterPath = false;
                        // 清除上一个路径，显示当前路径
                        ClearLastMonsterPath();
                        ShowCurMonsterPath();
                    }
                    else if (Input.GetKeyDown(KeyCode.Space))       // 确定编辑
                    {
                        // 修完路要点击确定才能进行覆盖，然后进行一系列操作
                        m_monsterPathList = new List<GridPosIndex>(m_editMosterPathList);
                        m_editMosterPathList.Clear();
                        m_isEditMonsterPath = false;

                    }
                    else if (Input.GetMouseButtonDown(0))               // 添加路径
                    {
                        if (m_editMosterPathList.Count == 0)
                        {
                            // 起点直接加进来
                            m_editMosterPathList.Add(gridTool.pos);
                        }
                        else if (m_canBePath)         // 若可以修路
                        {
                            // 看一下是否和上上个路径在同一条直线上，在就移除上一个
                            if (m_editMosterPathList.Count > 1)
                            {
                                GridPosIndex gridPosIndex1 = m_editMosterPathList[m_editMosterPathList.Count - 2];
                                GridPosIndex gridPosIndex2 = m_editMosterPathList[m_editMosterPathList.Count - 1];
                                if ((gridPosIndex1.xIndex == gridPosIndex2.xIndex && gridPosIndex1.xIndex == gridTool.pos.xIndex) || (gridPosIndex1.yIndex == gridPosIndex2.yIndex && gridPosIndex1.yIndex == gridTool.pos.yIndex))
                                {
                                    m_editMosterPathList.Remove(gridPosIndex2);
                                    m_gridArr[gridPosIndex2.xIndex, gridPosIndex2.yIndex].m_state.isMonsterPoint = false;
                                }
                            }

                            m_editMosterPathList.Add(gridTool.pos);
                        }
                    }
                    else if (Input.GetMouseButtonDown(1))               // 移除路径
                    {
                        if (m_editMosterPathList[m_editMosterPathList.Count - 1].Equals(gridTool.pos))
                        {
                            gridTool.m_state.canBuild = true;
                            gridTool.m_state.isMonsterPoint = false;
                            gridTool.SetSprite(null, false);
                            m_editMosterPathList.Remove(gridTool.pos);
                            // 把路上的 canbuild 属性改一下
                            if (m_editMosterPathList.Count > 0)
                            {
                                GridPosIndex lastIndex = gridTool.pos;
                                GridPosIndex curIndex = m_editMosterPathList[m_editMosterPathList.Count - 1];
                                if (lastIndex.xIndex == curIndex.xIndex)
                                {
                                    int offset = lastIndex.yIndex - curIndex.yIndex;
                                    offset /= Mathf.Abs(offset);
                                    for (int i = curIndex.yIndex + offset; i != lastIndex.yIndex; i += offset)
                                    {
                                        m_gridArr[curIndex.xIndex, i].m_state.canBuild = true;
                                    }
                                }
                                else
                                {
                                    int offset = lastIndex.xIndex - curIndex.xIndex;
                                    offset /= Mathf.Abs(offset);
                                    for (int i = curIndex.xIndex + offset; i != lastIndex.xIndex; i += offset)
                                    {
                                        m_gridArr[i, curIndex.yIndex].m_state.canBuild = true;
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    ref GridState state = ref gridTool.m_state;
                    // 添加一个 item
                    if (Input.GetKeyDown(KeyCode.A))
                    {
                        state.hasItem = true;
                        state.itemID = 0;
                        gridTool.UpdateState(gridTool.m_state);
                        return;
                    }
                    else if (Input.GetKeyDown(KeyCode.M))       // 编辑路径
                    {
                        // 进入编辑路径模式
                        m_isEditMonsterPath = true;
                        // 清除上一个路径，显示当前路径
                        ClearLastMonsterPath();
                        ShowCurMonsterPath();
                    }
                    else if (Input.GetMouseButtonDown(0))           // 按下鼠标左键修改该位置能否建造的属性
                    {
                        // 不在怪物路径点上才能修改状态
                        if (IsOnMonsterPath(gridTool.pos) == false)
                        {
                            state.canBuild = !state.canBuild;
                            gridTool.UpdateState(gridTool.m_state);
                        }
                        return;
                    }
                }
            }
    }

    // 是否处于怪物路径上
    private bool IsOnMonsterPath(GridPosIndex gridIndex)
    {
        List<GridPosIndex> gridIndexList;

        if (m_isEditMonsterPath)
        {
            gridIndexList = m_editMosterPathList;
        }
        else
        {
            gridIndexList = m_monsterPathList;
        }

        for (int i = 1; i < gridIndexList.Count; i++)
        {
            if(gridIndexList[i].yIndex == gridIndexList[i - 1].yIndex)
            {
                if(gridIndexList[i].xIndex == gridIndex.xIndex)
                {
                    int min = Mathf.Min(gridIndexList[i].yIndex, gridIndexList[i - 1].yIndex);
                    int max = Mathf.Max(gridIndexList[i].yIndex, gridIndexList[i - 1].yIndex);
                    if (gridIndex.yIndex >= min && gridIndex.yIndex <= max)
                    {
                        return true;
                    }
                }
            }
            else if(gridIndexList[i].yIndex == gridIndex.yIndex)
            {
                int min = Mathf.Min(gridIndexList[i].xIndex, gridIndexList[i - 1].xIndex);
                int max = Mathf.Max(gridIndexList[i].xIndex, gridIndexList[i - 1].xIndex);
                if (gridIndex.xIndex >= min && gridIndex.xIndex <= max)
                {
                    return true;
                }
            }
        }
        return false;
    }

    // 计算地图和格子的大小
    private void CalculateSize()
    {
        /*相机视口坐标转为世界坐标  视口坐标:从左下角为(0,0,0)开始 到右上角为(1,1,1)结束*/
        Vector3 leftDown = Camera.main.ViewportToWorldPoint(Vector3.zero);
        Vector3 rightUp = Camera.main.ViewportToWorldPoint(Vector3.one);
        m_mapWidth = rightUp.x - leftDown.x;
        m_mapHeight = rightUp.y - leftDown.y;

        m_gridHeight = m_mapHeight / m_row;
        m_gridWidth = m_mapWidth / m_column;
    }

    private void LoadGridRes()
    {
        m_gridSprite = FactoryManager.GetInstance().GetSprite("NormalMordel/Game/Grid");
        m_gridStartSprite = FactoryManager.GetInstance().GetSprite("NormalMordel/Game/StartSprite");
        m_gridCantBuildSprite = FactoryManager.GetInstance().GetSprite("NormalMordel/Game/cantBuild");

        m_startBoard = FactoryManager.GetInstance().GetSprite("NormalMordel/Game/startPoint");
        m_endCarrot = FactoryManager.GetInstance().GetSprite("NormalMordel/Game/Carrot/Items01-hd_30");
        //LoadItemRes();
    }

    // 初始化地图
    private void InitMap()
    {
        // 路和地图适配格子大小
        AdaptGridObjSize(m_bgSpriteRenderer, m_column, m_row);
        AdaptGridObjSize(m_roadSpriteRenderer, m_column, m_row - 2);
        // 创建格子
        Vector3 pos;
        GameObject gridGO;
        for (int i = 0; i < m_column; ++i)
        {
            for (int j = 0; j < m_row; ++j)
            {
                pos = new Vector3(i * m_gridWidth - m_mapWidth / 2 + m_gridWidth / 2, j * m_gridHeight - m_mapHeight / 2 + m_gridHeight / 2);
                // 地图编辑时没有工厂就直接实例化
                gridGO = Instantiate(gridTool, pos, Quaternion.identity, transform);

                // 适配格子大小
                AdaptGridObjSize(gridGO.GetComponent<SpriteRenderer>());

                m_gridArr[i, j] = gridGO.GetComponent<GridTool>();
                m_gridArr[i, j].pos.xIndex = i;
                m_gridArr[i, j].pos.yIndex = j;
            }
        }
    }

    private void LoadItemRes()
    {
        // 从文件里面读
        m_monsterPosSprite = FactoryManager.GetInstance().GetSprite($"NormalMordel/Game/1/Monster/3-2");

        // 初始化 item 的 Prefab
        m_itemInfoListDict = new Dictionary<int, ItemInfoListNode>();
        List<ItemInfo> itemInfoList = PlayerManager.GetInstance().GetItemInfoListByLevelGroup(m_curLevelGroupId);
        
        if (itemInfoList.Count > 0)
        {
            for(int i = 0; i < itemInfoList.Count; i++)
            {
                GameObject go = Resources.Load<GameObject>($"Prefabs/Game/{m_curLevelGroupId}/Items/{itemInfoList[i].Name}");
                if (go != null)
                {
                    ItemInfoListNode node = new ItemInfoListNode();
                    node.Go = go;
                    if (i != 0)
                    {
                        node.PreItemId = itemInfoList[i - 1].Id;
                    }
                    else
                    {
                        node.PreItemId = itemInfoList[itemInfoList.Count - 1].Id;
                    }

                    if (i != itemInfoList.Count - 1)
                    {
                        node.NextItemId = itemInfoList[i + 1].Id;
                    }
                    else
                    {
                        node.NextItemId = itemInfoList[0].Id;
                    }
                    m_itemInfoListDict.Add(itemInfoList[i].Id, node);
                }
                else
                {
                    Debug.Log($"Item加载失败，路径：Prefabs/Game/{m_curLevelGroupId}/Items/{itemInfoList[i].Name}");
                }
            }
        }
    }

    // spriteRenderer 适配到计算出的单个格子大小
    public void AdaptGridObjSize(SpriteRenderer spriteRenderer)
    {
        AdaptGridObjSize(spriteRenderer, 1, 1);
    }

    // spriteRenderer 适配到给定数量的格子大小
    public void AdaptGridObjSize(SpriteRenderer spriteRenderer, float columnGrid, float rowGrid)
    {
        if (spriteRenderer == null)
        {
            return;
        }
        Vector3 scale = spriteRenderer.transform.localScale;
        scale.x *= (m_gridWidth * columnGrid) / spriteRenderer.bounds.size.x;
        scale.y *= (m_gridHeight * rowGrid) / spriteRenderer.bounds.size.y;
        spriteRenderer.transform.localScale = scale;
    }

    public GameObject GetItemPrefab(int itemId)
    {
        if (m_itemInfoListDict.ContainsKey(itemId))
        {
            return m_itemInfoListDict[itemId].Go;
        }
        return null;
    }

    /// <summary>
    /// 给MapEditor调用的函数
    /// </summary>
    public void SaveLevel()
    {
        // 若处于路径编辑状态，则恢复之前状态再保存
        ClearLastMonsterPath();
        ShowCurMonsterPath();

        MapInfo mapInfo = new MapInfo
        {
            LevelGroupId = m_curLevelGroupId,
            LevelId = m_curLevelId,
            monsterPath = m_monsterPathList,
            roundInfo = m_roundInfo
        };

        for (int i = 0; i < m_column; ++i)
        {
            for (int j = 0; j < m_row; ++j)
            {
                mapInfo.gridPoints.Add(m_gridArr[i, j].m_state);
            }
        }

        FactoryManager.GetInstance().SaveJsonFile(mapInfo, $"Maps/{m_curLevelGroupId}-{m_curLevelId}");
    }

    //加载关卡-按大关卡数加小关卡数
    public void LoadLevel(int levelGroupId, int levelId)
    {
        m_curLevelGroupId = levelGroupId;
        m_curLevelId = levelId;
        LoadLevel(m_curLevelGroupId + "-" + m_curLevelId);
    }

    //加载关卡-按照文件名称
    public void LoadLevel(string fileName)
    {
        MapInfo mapInfo = FactoryManager.GetInstance().GetJsonObject<MapInfo>("Maps/" + fileName);
        m_curLevelGroupId = mapInfo.LevelGroupId;
        m_curLevelId = mapInfo.LevelId;

        LoadMapAndRoad();
        LoadItemRes();

        m_monsterPathList = mapInfo.monsterPath;
        m_roundInfo = mapInfo.roundInfo;
        for (int i = 0; i < m_column; ++i)
        {
            for (int j = 0; j < m_row; ++j)
            {
                // 刷数据用，保证 hasItem 的 ItemId 不为 -1，且 ItemId 为 -1 时，hasItem 为 false
                GridState state = mapInfo.gridPoints[i * m_row + j];
                if (state.hasItem == false)
                {
                    state.itemID = -1;
                }
                else if (state.itemID == -1)
                {
                    state.hasItem = true;
                }
                // 刷数据用，把对应关卡组的 Item id 改了
                if(m_curLevelGroupId == 1)
                {
                    if(state.itemID >=0 && state.itemID <= 9)
                    {
                        state.itemID += 10;
                    }
                }else if(m_curLevelGroupId == 2)
                {
                    if (state.itemID >= 0 && state.itemID <= 9)
                    {
                        state.itemID += 20;
                    }
                }

                m_gridArr[i, j].UpdateState(state);

            }
        }
        // 显示怪物路径
        ShowCurMonsterPath();
    }

    // 加载地图和路
    public void LoadMapAndRoad()
    {
        m_roadSprite = Resources.Load<Sprite>("Pictures/NormalMordel/Game/" + m_curLevelGroupId + "/Road" + m_curLevelId);
        m_bgSprite = Resources.Load<Sprite>("Pictures/NormalMordel/Game/" + m_curLevelGroupId + "/BG" + m_curLevelId / 4);

        m_roadSpriteRenderer.sprite = m_roadSprite;
        m_bgSpriteRenderer.sprite = m_bgSprite;
    }

    public void ClearMonsterPos()
    {
        if (m_monsterPathList.Count == 0)
        {
            return;
        }
        m_monsterPathList.Clear();
        for (int i = 0; i < m_column; ++i)
        {
            for (int j = 0; j < m_row; ++j)
            {
                m_gridArr[i, j].ClearMonsterPos();
            }
        }
    }

    public void ClearAll()
    {
        if (m_monsterPathList.Count > 0)
        {
            m_monsterPathList.Clear();
        }
        for (int i = 0; i < m_column; ++i)
        {
            for (int j = 0; j < m_row; ++j)
            {
                m_gridArr[i, j].Init();
            }
        }
    }

}
