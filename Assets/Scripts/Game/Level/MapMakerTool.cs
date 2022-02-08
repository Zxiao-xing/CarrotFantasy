using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapMakerTool : MonoSingleton<MapMakerTool>
{
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
    [SerializeField]
    private int m_curLevelGroupId;              // 当前关卡组 id
    [SerializeField]
    private int m_curLevelId;                   // 当前关卡 id
    public List<GridPosIndex> m_monsterPath { get; private set; } = new List<GridPosIndex>();          // 怪物路径
    public List<Round.RoundInfo> roundInfo = new List<Round.RoundInfo>();                           // 每轮怪物
    public GridTool[,] m_gridArr = new GridTool[m_column, m_row];                                       // 格子信息


    // 显示所需
    [SerializeField]
    GameObject grid;

    // 路和背景
    Sprite m_roadSprite;
    Sprite m_bgSprite;
    SpriteRenderer m_bgSpriteRenderer;
    SpriteRenderer m_roadSpriteRenderer;

    private GameObject m_startPoint;                    // 产怪起点
    public GameObject m_carrot { get; private set; }    // 终点萝卜

    [HideInInspector]
    public Sprite m_monsterPosSprite;           // 用于显示怪物路径的图片
    [HideInInspector]
    public Sprite gridSp, gridStartSp, gridCannotBuildSp;//点击格子后显示格子三种状态资源

    public GameObject[] itemPrefabs;

    private bool m_isEditMonsterPath = false;             // 是否正在编辑怪物路径
    private List<GridPosIndex> m_editMosterPathList = new List<GridPosIndex>();             // 当前编辑的临时路径
    private GridPosIndex m_lastMousePos;                    // 鼠标上一个指向的位置
    private bool m_canBePath;                             // 可以成为怪物路径点

    // Start is called before the first frame update
    void Start()
    {
        // 若存在 GameController 说明是走正常游戏流程进来的，就不是走的工具流程，此时移除掉该物体
        if (GameController.HasInstance())
        {
            Destroy(this.gameObject);
            return;
        }
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
    }

    // 编辑路径时候的显示
    private void ShowEditingPath()
    {
        // 当正在编辑路径，且不是路径起点时进行处理
        if (m_isEditMonsterPath && m_editMosterPathList.Count > 0)
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

            // 当前鼠标位置和上一个已确定路径的显示逻辑
            Vector3 vec = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit2D = Physics2D.Raycast(new Vector2(vec.x, vec.y), new Vector3(vec.x, vec.y, 0.1f));
            if (hit2D.collider != null)
            {
                GridTool gridTool = hit2D.collider.gameObject.GetComponent<GridTool>();

                xOffset = gridTool.pos.xIndex - lastPos.xIndex;
                yOffset = gridTool.pos.yIndex - lastPos.yIndex;
                // 路径上有其他的 item 就不允许修路
                m_canBePath = true;
                if (xOffset != 0)
                {
                    int offset = xOffset / Mathf.Abs(xOffset);
                    int j = gridTool.pos.yIndex;
                    for (int i = lastPos.xIndex + offset; i != gridTool.pos.xIndex + offset; i += offset)
                    {
                        if (m_gridArr[i, j].state.hasItem)
                        {
                            m_canBePath = false;
                            break;
                        }
                    }
                }
                else
                {
                    int offset = yOffset / Mathf.Abs(yOffset);
                    int j = gridTool.pos.yIndex;
                    for (int i = lastPos.yIndex + offset; i != gridTool.pos.yIndex + offset; i += offset)
                    {
                        if (m_gridArr[j, i].state.hasItem)
                        {
                            m_canBePath = false;
                            break;
                        }
                    }
                }

                // 若路上没有 item，再判断是否在同一个直线上
                if (m_canBePath)
                {
                    if (xOffset != 0 && yOffset == 0)
                    {
                        int offset = xOffset / Mathf.Abs(xOffset);
                        int j = m_lastMousePos.yIndex;
                        for (int i = lastPos.xIndex + offset; i != gridTool.pos.xIndex + offset; i += offset)
                        {
                            m_gridArr[i, j].SetSprite(gridSp, true);
                        }
                    }
                    else if (xOffset == 0 && yOffset != 0)
                    {
                        int offset = yOffset / Mathf.Abs(yOffset);
                        int j = m_lastMousePos.xIndex;
                        for (int i = lastPos.yIndex + offset; i != gridTool.pos.yIndex + offset; i += offset)
                        {
                            m_gridArr[j, i].SetSprite(gridSp, true);
                        }
                    }
                    else
                    {
                        // 不在一条直线上，不可以修路
                        m_canBePath = false;
                    }
                }

            }
            // 显示当前已经确定的路径
            ShowCurMonsterPath();
        }
    }

    // 根据是否是编辑路径状态来显示对应路径
    private void ShowCurMonsterPath()
    {
        List<GridPosIndex> gridIndexList;
        // 先移除另一个的
        if (m_isEditMonsterPath)
        {
            foreach (var gridIndex in m_monsterPath)
            {
                m_gridArr[gridIndex.xIndex, gridIndex.yIndex].SetSprite(null, false);
            }
            gridIndexList = m_editMosterPathList;
        }
        else
        {
            foreach (var gridIndex in m_editMosterPathList)
            {
                m_gridArr[gridIndex.xIndex, gridIndex.yIndex].SetSprite(null, false);
            }
            gridIndexList = m_monsterPath;
        }

        foreach (var gridIndex in gridIndexList)
        {
            m_gridArr[gridIndex.xIndex, gridIndex.yIndex].SetSprite(m_monsterPosSprite, true);
        }
    }

    // 处理按键输入
    private void HandleKeyInput()
    {
        if (Input.anyKeyDown)
        {
            // 可以用 Physics 和 Physics2D 区分两个，暂时不这样
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            GameObject hitGo = null;
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                hitGo = hit.collider.gameObject;
            }
            Vector3 vec = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit2D = Physics2D.Raycast(new Vector2(vec.x, vec.y), new Vector3(vec.x, vec.y, 0.1f));
            if (hit2D.collider != null)
            {
                hitGo = hit2D.collider.gameObject;
            }
            if (hitGo != null)
            {
                Item item = hitGo.GetComponent<Item>();
                if (item != null)                       // 对 item 的操作
                {
                    // 删除格子上的 item
                    if (Input.GetKeyDown(KeyCode.D))
                    {
                        item.m_gridTool.state.hasItem = false;
                        Destroy(hitGo);
                        return;
                    }
                    else if (Input.GetKeyDown(KeyCode.W))       // 下一个 item
                    {
                        item.m_gridTool.state.itemID++;
                        // 超过最大的 item 数就再次从 0 开始，建议这个还是不要固定死了，到时候康康
                        if (item.ID == 7)
                        {
                            item.m_gridTool.state.itemID = 0;
                        }

                        item.m_gridTool.UpdateState(item.m_gridTool.state);
                        return;
                    }
                    else if (Input.GetKeyDown(KeyCode.S))       // 上一个 item
                    {
                        item.m_gridTool.state.itemID--;
                        if (item.m_gridTool.state.itemID < 0)
                        {
                            item.m_gridTool.state.itemID = 7;
                        }
                        item.m_gridTool.UpdateState(item.m_gridTool.state);
                        return;
                    }
                }
                else                                    // 对 grid 的操作
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
                                // 清空临时存储的路径，如果后面要添加专门的清空操作就改改
                                m_editMosterPathList.Clear();
                            }
                            else if (Input.GetKeyDown(KeyCode.Space))       // 确定编辑
                            {
                                // 修完路要点击确定才能进行覆盖，然后进行一系列操作

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
                                    GridPosIndex gridPosIndex1 = m_editMosterPathList[m_editMosterPathList.Count - 2];
                                    GridPosIndex gridPosIndex2 = m_editMosterPathList[m_editMosterPathList.Count - 1];
                                    if ((gridPosIndex1.xIndex == gridPosIndex2.xIndex && gridPosIndex1.xIndex == gridTool.pos.xIndex) || (gridPosIndex1.yIndex == gridPosIndex2.yIndex && gridPosIndex1.yIndex == gridTool.pos.yIndex))
                                    {
                                        m_editMosterPathList.RemoveAt(m_editMosterPathList.Count - 1);
                                    }
                                    m_editMosterPathList.Add(gridTool.pos);
                                }
                            }
                            else if (Input.GetMouseButtonDown(2))               // 移除路径
                            {

                            }
                        }
                        else
                        {
                            ref GridState state = ref gridTool.state;
                            // 添加一个 item
                            if (Input.GetKeyDown(KeyCode.A))
                            {
                                state.hasItem = true;
                                state.itemID = 0;
                                gridTool.UpdateState(gridTool.state);
                                return;
                            }
                            else if (Input.GetKeyDown(KeyCode.M))       // 编辑路径
                            {
                                // 进入编辑路径模式
                                m_isEditMonsterPath = true;
                                // 显示当前的路径
                                ShowCurMonsterPath();
                            }
                            else if (Input.GetMouseButtonDown(0))           // 按下鼠标左键修改该位置能否建造的属性
                            {
                                state.canBuild = !state.canBuild;
                                gridTool.UpdateState(gridTool.state);
                                return;
                            }
                        }
                    }
                }
            }
        }

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

    // 计算地图和格子的大小
    private void CalculateSize()
    {
        /*相机视口坐标转为世界坐标  视口坐标:从左下角为(0,0,0)开始 到右上角为(1,1,1)结束*/
        Camera camer = Camera.main;
        Vector3 leftDown = camer.ViewportToWorldPoint(Vector3.zero);
        Vector3 rightUp = camer.ViewportToWorldPoint(Vector3.one);
        m_mapWidth = rightUp.x - leftDown.x;
        m_mapHeight = rightUp.y - leftDown.y;

        m_gridHeight = m_mapHeight / m_row;
        m_gridWidth = m_mapWidth / m_column;
    }

    void LoadGridRes()
    {
        m_monsterPosSprite = FactoryManager.GetInstance().GetSprite("NormalMordel/Game/1/Monster/3-2");

        gridSp = FactoryManager.GetInstance().GetSprite("NormalMordel/Game/Grid");
        gridStartSp = FactoryManager.GetInstance().GetSprite("NormalMordel/Game/StartSprite");
        gridCannotBuildSp = FactoryManager.GetInstance().GetSprite("NormalMordel/Game/cantBuild");
        itemPrefabs = new GameObject[8];
        // todo:这弄了改一下
        for (int i = 0; i < 8; ++i)
        {
            itemPrefabs[i] = Resources.Load<GameObject>("Prefabs/Game/1/Items/" + i);
            if (itemPrefabs[i] == null)
            {
                Debug.Log("错路径:" + "Prefabs/Game/1/Items/" + i);
            }
        }
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
                //地图编辑时没有工厂就直接实例化
                gridGO = Instantiate(grid, pos, Quaternion.identity, transform);

                // 适配格子大小
                AdaptGridObjSize(gridGO.GetComponent<SpriteRenderer>());

                m_gridArr[i, j] = gridGO.GetComponent<GridTool>();
                m_gridArr[i, j].pos.xIndex = i;
                m_gridArr[i, j].pos.yIndex = j;
            }
        }
    }

    // spriteRenderer 适配到计算出的单个格子大小
    private void AdaptGridObjSize(SpriteRenderer spriteRenderer)
    {
        AdaptGridObjSize(spriteRenderer, 1, 1);
    }

    // spriteRenderer 适配到给定数量的格子大小
    private void AdaptGridObjSize(SpriteRenderer spriteRenderer, float columnGrid, float rowGrid)
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

    /// <summary>
    /// 给MapEditor调用的函数
    /// </summary>
    public void SaveLevel()
    {
        MapInfo mapInfo = new MapInfo
        {
            LevelGroupId = m_curLevelGroupId,
            LevelId = m_curLevelId,
            monsterPath = this.m_monsterPath,
            roundInfo = this.roundInfo
        };

        for (int i = 0; i < m_column; ++i)
        {
            for (int j = 0; j < m_row; ++j)
            {
                mapInfo.gridPoints.Add(m_gridArr[i, j].state);
            }
        }

        FactoryManager.GetInstance().SaveJsonFile(mapInfo, m_curLevelGroupId + "-" + m_curLevelId);
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
        m_monsterPath = mapInfo.monsterPath;
        roundInfo = mapInfo.roundInfo;
        for (int i = 0; i < m_column; ++i)
        {
            for (int j = 0; j < m_row; ++j)
            {
                m_gridArr[i, j].UpdateState(mapInfo.gridPoints[i * m_row + j]);
            }
        }
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
        if (m_monsterPath.Count == 0)
            return;
        m_monsterPath.Clear();
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
        if (m_monsterPath.Count > 0)
            m_monsterPath.Clear();
        for (int i = 0; i < m_column; ++i)
        {
            for (int j = 0; j < m_row; ++j)
            {
                m_gridArr[i, j].Init();
            }
        }
    }

}
