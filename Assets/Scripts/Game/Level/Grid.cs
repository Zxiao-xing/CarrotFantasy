using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using System;

public struct GridState
{
    public bool isMonsterPoint;
    public int itemID;
    public bool canBuild;
    public bool hasItem;
}

public struct GridPosIndex : IEquatable<GridPosIndex>
{
    public int xIndex, yIndex;

    public bool Equals(GridPosIndex other)
    {
        if(other.xIndex == xIndex && other.yIndex == yIndex)
        {
            return true;
        }
        return false;
    }
}

public class Grid : MonoBehaviour
{

    SpriteRenderer m_spriteRenderer;

    public GridState m_state = new GridState();
    public GridPosIndex pos = new GridPosIndex();
    GameObject m_item;
    [HideInInspector]
    public GameObject towerGo = null;//格子上建的塔
    [HideInInspector]
    public GameObject levelUPsign;

    private void Awake()
    {
        m_spriteRenderer = GetComponent<SpriteRenderer>();
        levelUPsign = transform.Find("LevelUP").gameObject;
        levelUPsign.SetActive(false);
    }

    private void Start()
    {
        ShowGridShape();
    }

    private void Update()
    {
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
    }

    private void OnMouseDown()
    {
        //选择的UI就不执行
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }
        GameController.GetInstance().ClickGrid(this);
    }

    /// <summary>
    /// 在开始游戏时让可以建塔的格子闪烁一下,提示玩家此处可建塔
    /// </summary>
    private void ShowGridShape()
    {
        if (m_state.canBuild)
        {
            m_spriteRenderer.sprite = MapMaker.GetInstance().gridStartSp;
            m_spriteRenderer.DOFade(0, 2.3f).OnComplete(() =>
             {
                 m_spriteRenderer.enabled = false;
                 Color color = m_spriteRenderer.color;
                 color.a = 1;
                 m_spriteRenderer.color = color;
             });
        }
    }

    public void ShowGrid()
    {
        if (m_state.canBuild == false)
        {
            ShowCannotBuild();
        }
        else
        {
            m_spriteRenderer.enabled = true;
            m_spriteRenderer.sprite = MapMaker.GetInstance().gridSp;
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
    private void SetTowerListToRightPos()
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
    private void SetTowerShowToRightPos()
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
        m_spriteRenderer.enabled = true;
        m_spriteRenderer.sprite = MapMaker.GetInstance().gridCannotBuildSp;
        m_spriteRenderer.DOFade(0, 0.5f).SetLoops(2).OnComplete(() =>
        {
            m_spriteRenderer.enabled = false;
            Color color = m_spriteRenderer.color;
            color.a = 1;
            m_spriteRenderer.color = color;
        });
    }

    public void HideGrid()
    {
        m_spriteRenderer.enabled = false;
        if (towerGo == false)
            GameController.GetInstance().towerList.SetActive(false);
        else
        {
            GameController.GetInstance().towerShow.SetActive(false);
            towerGo.GetComponent<Tower>().attackRender.enabled = false;
        }

    }

    private void CreatItem()
    {
        m_item = FactoryManager.GetInstance().GetObject(ObjectFactoryType.GameFactory, $"{LevelManager.GetInstance().LevelGroupId}/Items/{PlayerManager.GetInstance().ItemInfoDict[m_state.itemID].Name}");

        m_item.transform.SetParent(GameController.GetInstance().transform);
        m_item.GetComponent<Item>().ID = m_state.itemID;
        ItemInfo itemInfo = PlayerManager.GetInstance().ItemInfoDict[m_state.itemID];
        // 适配格子大小，占多个格子的以左下角为原点向右上移动
        SpriteRenderer itemSpriteRenderer = m_item.GetComponent<SpriteRenderer>();
        switch (itemInfo.Size)
        {
            case EnItemSize.OneMOne:
                MapMaker.GetInstance().AdaptGridObjSize(itemSpriteRenderer);
                m_item.transform.position = transform.position;
                break;
            case EnItemSize.OneMTow:
                {
                    MapMaker.GetInstance().AdaptGridObjSize(itemSpriteRenderer, 2, 1);
                    Vector3 pos = transform.position;
                    pos.x += MapMaker.GetInstance().m_gridWidth / 2;
                    m_item.transform.position = pos;
                    break;
                }
            case EnItemSize.TowMTow:
                {
                    MapMaker.GetInstance().AdaptGridObjSize(itemSpriteRenderer, 2, 2);
                    Vector3 pos = transform.position;
                    pos.x += MapMaker.GetInstance().m_gridWidth / 2;
                    pos.y += MapMaker.GetInstance().m_gridHeight / 2;
                    m_item.transform.position = pos;
                    break;
                }
        }

        //让物品在z轴更靠近摄像机,做碰撞检测时就可以挡住格子的碰撞器
        m_item.transform.localPosition += Vector3.forward * -2;
    }

    // 更新格子状态
    public void UpdateState(GridState state)
    {
        this.m_state = state;
        if (m_item != null)
        {
            Destroy(m_item);
        }
        m_spriteRenderer.enabled = true;
        if (state.canBuild == true)
        {
            m_spriteRenderer.sprite = MapMaker.GetInstance().gridSp;
            if (state.hasItem && state.itemID != -1)
            {
                CreatItem();
            }
            ShowGridShape();
        }
        else
        {
            m_spriteRenderer.enabled = false;
        }
    }

}
