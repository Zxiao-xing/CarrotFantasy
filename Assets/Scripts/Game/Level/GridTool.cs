using UnityEngine;
using DG.Tweening;

public class GridTool : MonoBehaviour
{
    private SpriteRenderer m_spriteRenderer;

    public GridState m_state = new GridState();
    public GridPosIndex pos = new GridPosIndex();
    GameObject m_item;
    //[HideInInspector]
    //public GameObject towerGo = null;//格子上建的塔

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
        if (m_item != null)
        {
            Destroy(m_item);
        }
        m_spriteRenderer.enabled = true;
        m_spriteRenderer.sprite = MapMakerTool.GetInstance().m_gridSprite;
        m_state.isMonsterPoint = false;
        m_state.itemID = -1;
        m_state.canBuild = true;

        ShowGridShape();
    }

    // 清除怪物路径
    public void ClearMonsterPos()
    {
        if (m_state.isMonsterPoint == false)
        {
            return;
        }
        m_spriteRenderer.enabled = true;
        m_spriteRenderer.sprite = MapMakerTool.GetInstance().m_gridSprite;
    }

    // 更新格子状态
    public void UpdateState(GridState state)
    {
        m_state = state;
        if (m_item != null)
        {
            Destroy(m_item);
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
        }
        else
        {
            // 不能建造显示那个不能建造的图标
            m_spriteRenderer.enabled = true;
            //m_spriteRenderer.enabled = false;
            if(state.isMonsterPoint == false)
            {
                m_spriteRenderer.sprite = MapMakerTool.GetInstance().m_gridCantBuildSprite;
            }
            //在编辑地图的时候怪物路径点会显示出来,便于编辑游戏时就该隐藏
            //if (state.isMonsterPoint)
            //{
            //    m_spriteRenderer.sprite = MapMakerTool.GetInstance().m_monsterPosSprite;
            //}
        }
    }

    // 创建物体
    private void CreatItem()
    {
        m_item = Instantiate(MapMakerTool.GetInstance().GetItemPrefab(m_state.itemID));

        m_item.transform.SetParent(MapMakerTool.GetInstance().transform);
        m_item.GetComponent<Item>().ID = m_state.itemID;
        m_item.GetComponent<Item>().m_gridTool = this;

        ItemInfo itemInfo = PlayerManager.GetInstance().ItemInfoDict[m_state.itemID];

        // 适配格子大小，占多个格子的以左下角为原点向右上移动
        SpriteRenderer itemSpriteRenderer = m_item.GetComponent<SpriteRenderer>();
        switch (itemInfo.Size)
        {
            case EnItemSize.OneMOne:
                MapMakerTool.GetInstance().AdaptGridObjSize(itemSpriteRenderer);
                m_item.transform.position = transform.position;
                break;
            case EnItemSize.OneMTow:
                {
                    MapMakerTool.GetInstance().AdaptGridObjSize(itemSpriteRenderer, 2, 1);
                    Vector3 pos = transform.position;
                    pos.x += MapMakerTool.GetInstance().m_gridWidth / 2;
                    m_item.transform.position = pos;
                    break;
                }
            case EnItemSize.TowMTow:
                {
                    MapMakerTool.GetInstance().AdaptGridObjSize(itemSpriteRenderer, 2, 2);
                    Vector3 pos = transform.position;
                    pos.x += MapMakerTool.GetInstance().m_gridWidth / 2;
                    pos.y += MapMakerTool.GetInstance().m_gridHeight / 2;
                    m_item.transform.position = pos;
                    break;
                }
        }

        //让物品在z轴更靠近摄像机,做碰撞检测时就可以挡住格子的碰撞器
        m_item.transform.localPosition += Vector3.forward * -2;

    }

    public void SetSprite(Sprite sprite, bool enabled)
    {
        m_spriteRenderer.enabled = enabled;
        m_spriteRenderer.sprite = sprite;
    }

    /// <summary>
    /// 在开始游戏时让可以建塔的格子闪烁一下,提示玩家此处可建塔
    /// </summary>
    private void ShowGridShape()
    {
        if (m_state.canBuild)
        {
            m_spriteRenderer.sprite = MapMakerTool.GetInstance().m_gridStartSprite;
            m_spriteRenderer.DOFade(0, 2.3f).OnComplete(() =>
            {
                m_spriteRenderer.enabled = false;
                Color color = m_spriteRenderer.color;
                color.a = 1;
                m_spriteRenderer.color = color;
            });
        }
    }
}
