using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public struct GridState
{
    public bool isMonsterPoint;
    public int itemID;
    public bool canBuild;
    public bool hasItem;
}

public struct GridPosIndex
{
    public int xIndex, yIndex;
}

public class Grid : MonoBehaviour
{

    SpriteRenderer spriteRenderer;

    public GridState state = new GridState();
    public GridPosIndex pos = new GridPosIndex();
    GameObject curretnItem;
    [HideInInspector]
    public GameObject towerGo = null;//格子上建的塔
    [HideInInspector]
    public GameObject levelUPsign;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        levelUPsign = transform.Find("LevelUP").gameObject;
        levelUPsign.SetActive(false);
#if Tool
        //mapMaker = MapMaker.GetInstance();
#endif     
    }

    private void Start()
    {
#if Tool
        Init();
#endif
#if Game
        // ShowGridShape();
#endif
    }

    private void Update()
    {
#if Game
        if (towerGo != null)
        {
            if (towerGo.GetComponent<TowerPersonalProperty>().towerLevel < 3 &&
               GameController.GetInstance().coins >= towerGo.GetComponent<TowerPersonalProperty>().upPrice)
            {
                if (levelUPsign.activeSelf == false)
                    levelUPsign.SetActive(true);
            }
            else
            {
                if (levelUPsign.activeSelf == true)
                    levelUPsign.SetActive(false);
            }

        }
#endif
    }

    public void Init()
    {
        if (curretnItem != null)
        {
            Destroy(curretnItem);
        }
        spriteRenderer.enabled = true;
        spriteRenderer.sprite = MapMaker.GetInstance().gridSp;
        state.isMonsterPoint = false;
        state.itemID = -1;
        state.canBuild = true;
    }
#if Tool
    private void OnMouseDown()
    {
        if (Input.GetKey(KeyCode.M))
        {
            state.isMonsterPoint = !state.isMonsterPoint;
            if(state.isMonsterPoint)
            {
                spriteRenderer.sprite = MapMaker.GetInstance().monsterPosSp;
                state.canBuild = false;
                MapMaker.GetInstance().monsterPos.Add(pos);
            }
            else
            {
                state.canBuild = true;
                MapMaker.GetInstance().monsterPos.Remove(pos);
                spriteRenderer.sprite = MapMaker.GetInstance().gridSp;
            }
        }
        else if (Input.GetKey(KeyCode.I))
        {
            if (curretnItem != null)
                Destroy(curretnItem);
            state.itemID++;
            if(state.itemID > 7)
            {
                state.itemID = -1;
            }
            else
            {
                CreatItem();                 
            }
        }

        else
        {
            if(state.itemID != -1)
            {
                state.itemID = -1;
                Destroy(curretnItem);
            }
            else if(state.isMonsterPoint)
            {
                state.canBuild = true;
                MapMaker.GetInstance().monsterPos.Remove(pos);
                spriteRenderer.sprite = MapMaker.GetInstance().gridSp;
                state.isMonsterPoint = false;
            }
            else
            {
                state.canBuild = !state.canBuild;
            }
        }
        spriteRenderer.enabled = state.canBuild;
        if (state.isMonsterPoint)
            spriteRenderer.enabled = true;
    }
#endif
#if Game
    private void OnMouseDown()
    {
        //选择的UI就不执行
        if (EventSystem.current.IsPointerOverGameObject())
            return;
        GameController.GetInstance().ClickGrid(this);
    }
#endif

    /// <summary>
    /// 在开始游戏时让可以建塔的格子闪烁一下,提示玩家此处可建塔
    /// </summary>
    void ShowGridShape()
    {
        if (state.canBuild)
        {
            spriteRenderer.sprite = MapMaker.GetInstance().gridStartSp;
            spriteRenderer.DOFade(0, 2.3f).OnComplete(() =>
             {
                 spriteRenderer.enabled = false;
                 Color color = spriteRenderer.color;
                 color.a = 1;
                 spriteRenderer.color = color;
             });
        }
    }

    public void ShowGrid()
    {
        if (state.canBuild == false)
        {
            ShowCannotBuild();
        }
        else
        {
            spriteRenderer.enabled = true;
            spriteRenderer.sprite = MapMaker.GetInstance().gridSp;
            if (towerGo == null)
            {
                GameController.GetInstance().towerList.SetActive(true);
                SetTowerListToRightPos();
            }
            else
            {
                GameController.GetInstance().towerShow.SetActive(true);
                towerGo.GetComponent<Tower>().attackRender.enabled = true;
                SetTowerShowToRightPos();
            }

        }
    }

    /// <summary>
    /// 如果玩家点击的格子是边缘位置,就要调整一下
    /// 建塔列表的显示位置
    /// </summary>
    void SetTowerListToRightPos()
    {
        int towerNums = GameController.GetInstance().towerList.transform.childCount;
        Vector3 pos = transform.position;
        float gridHeight = MapMaker.GetInstance().m_gridHeight;
        float gridWidth = MapMaker.GetInstance().m_gridWidth;
        pos.y += gridHeight;
        if (this.pos.yIndex >= MapMaker.m_row - 2)
            pos.y = transform.position.y - gridHeight;
        if (towerNums < 5)
        {
            if (this.pos.xIndex == 0)
                pos.x = transform.position.x + gridWidth * (towerNums - 1) * .5f;
            else if (this.pos.xIndex == MapMaker.m_column - 1)
                pos.x = transform.position.x - gridWidth * (towerNums - 1) * .5f;
        }
        else
        {
            if (this.pos.xIndex <= 1)
                pos.x = transform.position.x + gridWidth * (towerNums - 1) * .5f;
            else if (this.pos.xIndex >= MapMaker.m_column - 2)
                pos.x = transform.position.x - gridWidth * (towerNums - 1) * .5f;
        }
        GameController.GetInstance().towerList.transform.position = pos;

    }

    /// <summary>
    /// 如果玩家点击的格子是边缘位置,就要调整一下
    /// 升级/销毁塔列表的显示位置
    /// </summary>
    void SetTowerShowToRightPos()
    {
        GameController.GetInstance().towerShow.transform.position = transform.position;
        GameController.GetInstance().towerDesTrans.localPosition = GameController.GetInstance().btnDown.localPosition;
        GameController.GetInstance().towerUpTrans.localPosition = GameController.GetInstance().btnUp.localPosition;
        if (pos.yIndex == 7)
        {
            if (pos.xIndex <= 1)
                GameController.GetInstance().towerUpTrans.localPosition = GameController.GetInstance().btnRight.localPosition;
            else
                GameController.GetInstance().towerUpTrans.localPosition = GameController.GetInstance().btnLeft.localPosition;
        }
        else if (pos.yIndex == 0)
        {
            if (pos.xIndex <= 1)
                GameController.GetInstance().towerDesTrans.localPosition = GameController.GetInstance().btnRight.localPosition;
            else
                GameController.GetInstance().towerDesTrans.localPosition = GameController.GetInstance().btnLeft.localPosition;
        }
    }

    /// <summary>
    /// 提示玩家此格子无法建塔
    /// </summary>
    private void ShowCannotBuild()
    {
        spriteRenderer.enabled = true;
        spriteRenderer.sprite = MapMaker.GetInstance().gridCannotBuildSp;
        spriteRenderer.DOFade(0, 0.5f).SetLoops(2).OnComplete(() =>
        {
            spriteRenderer.enabled = false;
            Color color = spriteRenderer.color;
            color.a = 1;
            spriteRenderer.color = color;
        });
    }

    public void HideGrid()
    {
        spriteRenderer.enabled = false;
        if (towerGo == false)
            GameController.GetInstance().towerList.SetActive(false);
        else
        {
            GameController.GetInstance().towerShow.SetActive(false);
            towerGo.GetComponent<Tower>().attackRender.enabled = false;
        }

    }

    public void ClaerMonsterPos()
    {
        if (state.isMonsterPoint == false)
            return;
        spriteRenderer.enabled = true;
        spriteRenderer.sprite = MapMaker.GetInstance().gridSp;
    }

    void CreatItem()
    {
#if Game
        curretnItem = GameController.GetInstance().GetObject(ObjectFactoryType.GameFactory, LevelManager.GetInstance().LevelGroupId + "/Items/" + state.itemID);
#elif Tool
        curretnItem = Instantiate(MapMaker.GetInstance().itemPrefabs[state.itemID]);
#endif

        curretnItem.transform.SetParent(GameController.GetInstance().transform);
        // curretnItem.transform.localScale = Vector3.one;
        curretnItem.GetComponent<Item>().ID = state.itemID;
        if (state.itemID <= 2)
        {
            Vector3 pos = transform.position;
            pos.x += MapMaker.GetInstance().m_gridWidth / 2;
            pos.y += MapMaker.GetInstance().m_gridHeight / 2;
            curretnItem.transform.position = pos;
        }
        else if (state.itemID <= 4)
        {
            Vector3 pos = transform.position;
            pos.x -= MapMaker.GetInstance().m_gridWidth / 2;
            curretnItem.transform.position = pos;
        }
        else
        {
            curretnItem.transform.position = transform.position;
        }
        //让物品在z轴更靠近摄像机,做碰撞检测时就可以挡住格子的碰撞器
        curretnItem.transform.localPosition += Vector3.forward * -2;
    }

    // 更新格子状态
    public void UpdateState(GridState state)
    {
        this.state = state;
        if (curretnItem != null)
        {
            Destroy(curretnItem);
        }
        spriteRenderer.enabled = true;
        if (state.canBuild == true)
        {
            spriteRenderer.sprite = MapMaker.GetInstance().gridSp;
            if (state.hasItem && state.itemID != -1)
            {
                CreatItem();
            }
            ShowGridShape();
        }
        else
        {
            spriteRenderer.enabled = false;
#if Tool
            //在编辑地图的时候怪物路径点会显示出来,便于编辑 游戏时就该隐藏
            if (state.isMonsterPoint)
            {
                spriteRenderer.enabled = true;
                spriteRenderer.sprite = MapMaker.GetInstance().monsterPosSp;
            }         
#endif
        }
    }

}
