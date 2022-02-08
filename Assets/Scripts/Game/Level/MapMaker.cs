using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;
using System.IO;

public class MapMaker : MonoSingleton<MapMaker>
{
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

    [SerializeField] GameObject grid;
    [SerializeField] bool isDraw;
    [SerializeField]
    private int m_curLevelGroupId;
    [SerializeField]
    private int m_curLevelId;
    [HideInInspector]
    public GameObject[] itemPrefabs;
    public List<GridPosIndex> monsterPos { get; private set; } = new List<GridPosIndex>();
    //每一波怪物的ID列表  如 1，1，1，2，2，2，3，3(出现几次代表有几个)
    public List<Round.RoundInfo> roundInfo = new List<Round.RoundInfo>();
    public Grid[,] grids = new Grid[m_column, m_row];
    [HideInInspector] public Sprite monsterPosSp;
    [HideInInspector] public Sprite gridSp, gridStartSp, gridCannotBuildSp;//点击格子后显示格子三种状态资源

    // 路和背景
    Sprite m_roadSprite;
    Sprite m_bgSprite;
    SpriteRenderer m_bgSpriteRenderer;
    SpriteRenderer m_roadSpriteRenderer;

    private GameObject m_startPoint;                    // 产怪起点
    public GameObject m_carrot { get; private set; }    // 终点萝卜

    protected override void Init()
    {
        GameObject bgGo = new GameObject("BG");
        bgGo.transform.parent = transform;
        m_bgSpriteRenderer = bgGo.AddComponent<SpriteRenderer>();

        GameObject roadGo = new GameObject("Road");
        roadGo.transform.parent = transform;
        Vector3 pos = roadGo.transform.position;
        pos.y -= 1.25f;
        roadGo.transform.position = pos;
        m_roadSpriteRenderer = roadGo.AddComponent<SpriteRenderer>();
        // 路显示在地图前面
        m_roadSpriteRenderer.sortingOrder++;
        gridSp = FactoryManager.GetInstance().GetSprite("NormalMordel/Game/Grid");
        gridStartSp = FactoryManager.GetInstance().GetSprite("NormalMordel/Game/StartSprite");
        gridCannotBuildSp = FactoryManager.GetInstance().GetSprite("NormalMordel/Game/cantBuild");

        CalculateSize();

        InitMap();
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

    // 初始化地图
    private void InitMap()
    {
        // 创建格子
        Vector3 pos;
        GameObject gridGO;
        for (int i = 0; i < m_column; ++i)
        {
            for (int j = 0; j < m_row; ++j)
            {
                pos = CorrectPos(new Vector3(i * m_gridWidth, j * m_gridHeight, 0));
                gridGO = FactoryManager.GetInstance().GetObject(ObjectFactoryType.GameFactory, "Grid");
                gridGO.transform.SetParent(transform);
                gridGO.transform.position = pos;
                // 适配格子大小
                AdaptGridObjSize(gridGO.GetComponent<SpriteRenderer>());

                grids[i, j] = gridGO.GetComponent<Grid>();
                grids[i, j].pos.xIndex = i;
                grids[i, j].pos.yIndex = j;
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


    Vector3 CorrectPos(Vector3 pos)
    {
        pos = new Vector3(pos.x - m_mapWidth / 2 + m_gridWidth / 2, pos.y - m_mapHeight / 2 + m_gridHeight / 2);
        return pos;
    }

    //加载关卡-按照文件名称
    public void LoadLevel(string fileName)
    {
        MapInfo mapInfo = FactoryManager.GetInstance().GetJsonObject<MapInfo>("Maps/" + fileName);
        m_curLevelGroupId = mapInfo.LevelGroupId;
        m_curLevelId = mapInfo.LevelId;
        LoadMapAndRoad();
        monsterPos = mapInfo.monsterPath;
        roundInfo = mapInfo.roundInfo;
        for (int i = 0; i < m_column; ++i)
        {
            for (int j = 0; j < m_row; ++j)
            {
                grids[i, j].UpdateState(mapInfo.gridPoints[i * m_row + j]);
            }
        }
        // 初始化怪物生成点，位置在怪物路径的第一个
        m_startPoint = FactoryManager.GetInstance().GetObject(ObjectFactoryType.GameFactory, "startPoint");
        m_startPoint.transform.SetParent(transform);
        m_startPoint.transform.position = grids[monsterPos[0].xIndex, monsterPos[0].yIndex].transform.position;
        AdaptGridObjSize(m_startPoint.GetComponent<SpriteRenderer>(), 0.5f, 1);
        // 初始化萝卜，位置在怪物路径的最后一个
        m_carrot = FactoryManager.GetInstance().GetObject(ObjectFactoryType.GameFactory, "Carrot");
        m_carrot.transform.SetParent(transform);
        m_carrot.transform.position = grids[monsterPos[monsterPos.Count - 1].xIndex, monsterPos[monsterPos.Count - 1].yIndex].transform.position;
        AdaptGridObjSize(m_carrot.GetComponent<SpriteRenderer>(), 0.5f, 1);
    }

    //加载关卡-按大关卡数加小关卡数
    public void LoadLevel(int levelGroupId, int levelId)
    {
        m_curLevelGroupId = levelGroupId;
        m_curLevelId = levelId;
        LoadLevel(m_curLevelGroupId + "-" + m_curLevelId);
    }

    // 加载地图和路
    public void LoadMapAndRoad()
    {
        m_roadSprite = FactoryManager.GetInstance().GetSprite("NormalMordel/Game/" + m_curLevelGroupId + "/Road" + m_curLevelId);
        m_bgSprite = FactoryManager.GetInstance().GetSprite("NormalMordel/Game/" + m_curLevelGroupId + "/BG" + m_curLevelId / 4);

        m_roadSpriteRenderer.sprite = m_roadSprite;
        m_bgSpriteRenderer.sprite = m_bgSprite;

        // 路和地图适配格子大小
        AdaptGridObjSize(m_bgSpriteRenderer, m_column, m_row);
        AdaptGridObjSize(m_roadSpriteRenderer, m_column, m_row - 2);
    }

    public List<Vector3> GetMonsterPosVect()
    {
        List<Vector3> pos = new List<Vector3>(monsterPos.Count);
        foreach (var item in monsterPos)
        {
            pos.Add(grids[item.xIndex, item.yIndex].transform.position);
        }
        return pos;
    }

}
