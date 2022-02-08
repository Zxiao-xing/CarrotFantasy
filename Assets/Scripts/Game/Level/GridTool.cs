using UnityEngine;
using DG.Tweening;

public class GridTool : MonoBehaviour
{
    private SpriteRenderer m_spriteRenderer;

    public GridState state = new GridState();
    public GridPosIndex pos = new GridPosIndex();
    GameObject curretnItem;
    [HideInInspector]
    public GameObject towerGo = null;//格子上建的塔

    private void Awake()
    {
        m_spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        Init();
    }

    public void Init()
    {
        if (curretnItem != null)
        {
            Destroy(curretnItem);
        }
        m_spriteRenderer.enabled = true;
        m_spriteRenderer.sprite = MapMakerTool.GetInstance().gridSp;
        state.isMonsterPoint = false;
        state.itemID = -1;
        state.canBuild = true;
    }

    // 清除怪物路径
    public void ClearMonsterPos()
    {
        if (state.isMonsterPoint == false)
        {
            return;
        }
        m_spriteRenderer.enabled = true;
        m_spriteRenderer.sprite = MapMakerTool.GetInstance().gridSp;
    }

    // 更新格子状态
    public void UpdateState(GridState state)
    {
        this.state = state;
        if (curretnItem != null)
        {
            Destroy(curretnItem);
        }
        m_spriteRenderer.enabled = true;
        if (state.canBuild == true)
        {
            //m_spriteRenderer.sprite = MapMakerTool.GetInstance().gridSp;
            m_spriteRenderer.enabled = false;
            if (state.hasItem && state.itemID != -1)
            {
                CreatItem();
            }
            //ShowGridShape();
        }
        else
        {
            // 不能建造显示那个不能建造的图标
            m_spriteRenderer.enabled = true;
            //m_spriteRenderer.enabled = false;
            m_spriteRenderer.sprite = MapMakerTool.GetInstance().gridCannotBuildSp;
            //在编辑地图的时候怪物路径点会显示出来,便于编辑 游戏时就该隐藏
            if (state.isMonsterPoint)
            {
                m_spriteRenderer.sprite = MapMakerTool.GetInstance().m_monsterPosSprite;
            }
        }
    }

    // 创建物体
    private void CreatItem()
    {
        curretnItem = Instantiate(MapMakerTool.GetInstance().itemPrefabs[state.itemID]);

        curretnItem.transform.SetParent(MapMakerTool.GetInstance().transform);
        // curretnItem.transform.localScale = Vector3.one;
        curretnItem.GetComponent<Item>().ID = state.itemID;
        curretnItem.GetComponent<Item>().m_gridTool = this;
        if (state.itemID <= 2)
        {
            Vector3 pos = transform.position;
            pos.x += MapMakerTool.GetInstance().m_gridWidth / 2;
            pos.y += MapMakerTool.GetInstance().m_gridHeight / 2;
            curretnItem.transform.position = pos;
        }
        else if (state.itemID <= 4)
        {
            Vector3 pos = transform.position;
            pos.x -= MapMakerTool.GetInstance().m_gridWidth / 2;
            curretnItem.transform.position = pos;
        }
        else
        {
            curretnItem.transform.position = transform.position;
        }
        //让物品在z轴更靠近摄像机,做碰撞检测时就可以挡住格子的碰撞器
        curretnItem.transform.localPosition += Vector3.forward * -2;

    }

    /// <summary>
    /// 在开始游戏时让可以建塔的格子闪烁一下,提示玩家此处可建塔
    /// </summary>
    private void ShowGridShape()
    {
        if (state.canBuild)
        {
            m_spriteRenderer.sprite = MapMakerTool.GetInstance().gridStartSp;
            m_spriteRenderer.DOFade(0, 2.3f).OnComplete(() =>
            {
                m_spriteRenderer.enabled = false;
                Color color = m_spriteRenderer.color;
                color.a = 1;
                m_spriteRenderer.color = color;
            });
        }
    }

    public void SetSprite(Sprite sprite, bool enabled)
    {
        m_spriteRenderer.enabled = enabled;
        m_spriteRenderer.sprite = sprite;
    }


    //public void ShowGrid()
    //{
    //    if (state.canBuild == false)
    //    {
    //        ShowCannotBuild();
    //    }
    //    else
    //    {
    //        spriteRenderer.enabled = true;
    //        spriteRenderer.sprite = MapMakerTool.GetInstance().gridSp;
    //        if (towerGo == null)
    //        {
    //            GameController.GetInstance().towerList.SetActive(true);
    //            SetTowerListToRightPos();
    //        }
    //        else
    //        {
    //            GameController.GetInstance().towerShow.SetActive(true);
    //            towerGo.GetComponent<Tower>().attackRender.enabled = true;
    //            SetTowerShowToRightPos();
    //        }

    //    }
    //}

    /// <summary>
    /// 如果玩家点击的格子是边缘位置,就要调整一下
    /// 建塔列表的显示位置
    /// </summary>
    //void SetTowerListToRightPos()
    //{
    //    int towerNums = GameController.GetInstance().towerList.transform.childCount;
    //    Vector3 pos = transform.position;
    //    float gridHeight = MapMakerTool.GetInstance().m_gridHeight;
    //    float gridWidth = MapMakerTool.GetInstance().m_gridWidth;
    //    pos.y += gridHeight;
    //    if (this.pos.yIndex >= MapMakerTool.m_row - 2)
    //    {
    //        pos.y = transform.position.y - gridHeight;
    //    }
    //    if (towerNums < 5)
    //    {
    //        if (this.pos.xIndex == 0)
    //        {
    //            pos.x = transform.position.x + gridWidth * (towerNums - 1) * .5f;
    //        }
    //        else if (this.pos.xIndex == MapMakerTool.m_column - 1)
    //        {
    //            pos.x = transform.position.x - gridWidth * (towerNums - 1) * .5f;
    //        }
    //    }
    //    else
    //    {
    //        if (this.pos.xIndex <= 1)
    //        {
    //            pos.x = transform.position.x + gridWidth * (towerNums - 1) * .5f;
    //        }
    //        else if (this.pos.xIndex >= MapMakerTool.m_column - 2)
    //        {
    //            pos.x = transform.position.x - gridWidth * (towerNums - 1) * .5f;
    //        }
    //    }
    //    GameController.GetInstance().towerList.transform.position = pos;

    //}

    /// <summary>
    /// 如果玩家点击的格子是边缘位置,就要调整一下
    /// 升级/销毁塔列表的显示位置
    /// </summary>
    //private void SetTowerShowToRightPos()
    //{
    //    GameController.GetInstance().towerShow.transform.position = transform.position;
    //    GameController.GetInstance().towerDesTrans.localPosition = GameController.GetInstance().btnDown.localPosition;
    //    GameController.GetInstance().towerUpTrans.localPosition = GameController.GetInstance().btnUp.localPosition;
    //    if (pos.yIndex == 7)
    //    {
    //        if (pos.xIndex <= 1)
    //        {
    //            GameController.GetInstance().towerUpTrans.localPosition = GameController.GetInstance().btnRight.localPosition;
    //        }
    //        else
    //        {
    //            GameController.GetInstance().towerUpTrans.localPosition = GameController.GetInstance().btnLeft.localPosition;
    //        }
    //    }
    //    else if (pos.yIndex == 0)
    //    {
    //        if (pos.xIndex <= 1)
    //        {
    //            GameController.GetInstance().towerDesTrans.localPosition = GameController.GetInstance().btnRight.localPosition;
    //        }
    //        else
    //        {
    //            GameController.GetInstance().towerDesTrans.localPosition = GameController.GetInstance().btnLeft.localPosition;
    //        }
    //    }
    //}

    /// <summary>
    /// 提示玩家此格子无法建塔
    /// </summary>
    //private void ShowCannotBuild()
    //{
    //    spriteRenderer.enabled = true;
    //    spriteRenderer.sprite = MapMakerTool.GetInstance().gridCannotBuildSp;
    //    spriteRenderer.DOFade(0, 0.5f).SetLoops(2).OnComplete(() =>
    //    {
    //        spriteRenderer.enabled = false;
    //        Color color = spriteRenderer.color;
    //        color.a = 1;
    //        spriteRenderer.color = color;
    //    });
    //}

    //public void HideGrid()
    //{
    //    spriteRenderer.enabled = false;
    //    if (towerGo == false)
    //    {
    //        GameController.GetInstance().towerList.SetActive(false);
    //    }
    //    else
    //    {
    //        GameController.GetInstance().towerShow.SetActive(false);
    //        towerGo.GetComponent<Tower>().attackRender.enabled = false;
    //    }

    //}


}
