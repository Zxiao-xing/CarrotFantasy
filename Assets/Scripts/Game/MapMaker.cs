using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;
using System.IO;

public class MapMaker : MonoBehaviour
{
    private static MapMaker _ins;
    public static MapMaker _Ins { get { return _ins; } }

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
    private Vector2 m_mapDefaultSize = new Vector2();
    // 路和背景
    public SpriteRenderer m_bgSpriteRenderer;
    public SpriteRenderer m_roadSpriteRenderer;

    [SerializeField] GameObject grid;
    [SerializeField] bool isDraw;
    [SerializeField] int currentBigLevel;
    [SerializeField] int currentLevel;
    [HideInInspector] public GameObject[] itemPrefabs;
    public List<Grids.GridPosIndex> monsterPos { get; private set; } = new List<Grids.GridPosIndex>();
    //每一波怪物的ID列表  如 1，1，1，2，2，2，3，3(出现几次代表有几个)
    public List<Round.RoundInfo> roundInfo = new List<Round.RoundInfo>();
    public Grids[,] grids = new Grids[m_column, m_row];
    [HideInInspector] public Sprite monsterPosSp;
    [HideInInspector] public Sprite gridSp, gridStartSp, gridCannotBuildSp;//点击格子后显示格子三种状态资源
    Sprite roadSp, bgSp;
    SpriteRenderer bgSR, roadSR;

    //产怪起点与终点的萝卜
    GameObject startPoint;
    public GameObject carrot { get; private set; }

    private void Awake()
    {
        //gameController = GetComponent<GameController>();
        bgSR = transform.Find("BG").GetComponent<SpriteRenderer>();
        roadSR = transform.Find("Road").GetComponent<SpriteRenderer>();
        CalculateSize();
#if Game
        gridSp = GameController.GetInstance().GetSprite("NormalMordel/Game/Grid");
        gridStartSp = GameController.GetInstance().GetSprite("NormalMordel/Game/StartSprite");
        gridCannotBuildSp = GameController.GetInstance().GetSprite("NormalMordel/Game/cantBuild");
#endif
#if Tool
         _ins = this;
        LoadGridRes();
#endif
        AdaptSpriteRender();
        InitMap();
    }

    void LoadGridRes()
    {
        monsterPosSp = Resources.Load<Sprite>("Pictures/NormalMordel/Game/1/Monster/3-2");
        gridSp = Resources.Load<Sprite>("Pictures/NormalMordel/Game/Grid");
        itemPrefabs = new GameObject[8];
        for (int i = 0; i < 8; ++i)
        {
            itemPrefabs[i] = Resources.Load<GameObject>("Prefabs/Game/1/Items/" + i);
            if (itemPrefabs[i] == null)
            {
                Debug.Log("错路径:" + "Prefabs/Game/1/Items/" + i);
            }
        }
    }

    void CalculateSize()
    {
        /*相机视口坐标转为世界坐标  视口坐标:从左下角为(0,0,0)开始 到右上角为(1,1,1)结束*/
        Camera camer = Camera.main;
        Vector3 leftDown = camer.ViewportToWorldPoint(Vector3.zero);
        Vector3 rightUp = camer.ViewportToWorldPoint(Vector3.one);
        m_mapWidth = rightUp.x - leftDown.x;
        m_mapHeight = rightUp.y - leftDown.y;
        // 

        m_gridHeight = m_mapHeight / m_row;
        m_gridWidth = m_mapWidth / m_column;
    }

    private void InitMap()
    {
        Vector3 pos;
        GameObject gridGO;
        for (int i = 0; i < m_column; ++i)
        {
            for (int j = 0; j < m_row; ++j)
            {
                pos = CorrectPos(new Vector3(i * m_gridWidth, j * m_gridHeight, 0));
                //地图编辑时没有工厂就直接实例化
#if Tool
                GameObject gridGO= Instantiate(grid, pos, Quaternion.identity, transform);
#endif
#if Game
                gridGO = GameController.GetInstance().GetObject(ObjectFactoryType.GameFactory, "Grid");
                gridGO.transform.SetParent(transform);
                gridGO.transform.position = pos;
#endif
                grids[i, j] = gridGO.GetComponent<Grids>();
                grids[i, j].pos.xIndex = i;
                grids[i, j].pos.yIndex = j;
            }
        }
    }

    Vector3 CorrectPos(Vector3 pos)
    {
        pos = new Vector3(pos.x - m_mapWidth / 2 + m_gridWidth / 2, pos.y - m_mapHeight / 2 + m_gridHeight / 2);
        return pos;
    }

    private void OnDrawGizmos()
    {
        if (isDraw == false)
        {
            return;
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
        AdaptSpriteRender();
    }

    private void AdaptSpriteRender()
    {
        if(m_bgSpriteRenderer != null)
        {
            Vector3 scale = m_bgSpriteRenderer.transform.localScale;
            scale.y *= m_mapHeight / m_bgSpriteRenderer.bounds.size.y;
            scale.x *= m_mapWidth / m_bgSpriteRenderer.bounds.size.x;
            m_bgSpriteRenderer.transform.localScale = scale;
        }

        if(m_roadSpriteRenderer != null)
        {
            // 路的图要比地图少两个格子的高度
            Vector3 scale = m_roadSpriteRenderer.transform.localScale;
            scale.y *= (m_mapHeight - 2 * m_gridHeight) / m_roadSpriteRenderer.bounds.size.y;
            scale.x *= m_mapWidth / m_roadSpriteRenderer.bounds.size.x;
            m_roadSpriteRenderer.transform.localScale = scale;
        }
    }

    /// <summary>
    /// 给MapEditor调用的函数
    /// </summary>
    public void SaveLevel()
    {
        MapInfo levelInfo = new MapInfo
        {
            bigLevelID = this.currentBigLevel,
            levelID = this.currentLevel,
            monsterPath = this.monsterPos,
            roundInfo = this.roundInfo
        };

        for (int i = 0; i < m_column; ++i)
        {
            for (int j = 0; j < m_row; ++j)
            {
                levelInfo.gridPoints.Add(grids[i, j].state);
            }
        }
        string jsonStr = JsonMapper.ToJson(levelInfo);
        StreamWriter sw = new StreamWriter(Application.dataPath + "/Resources/Json/Maps/" + currentBigLevel + "-" + currentLevel + ".json");
        sw.Write(jsonStr);
        sw.Close();
    }

    public void ClearMonsterPos()
    {
        if (monsterPos.Count == 0)
            return;
        monsterPos.Clear();
        for (int i = 0; i < m_column; ++i)
        {
            for (int j = 0; j < m_row; ++j)
            {
                grids[i, j].ClaerMonsterPos();
            }
        }
    }

    public void ClearAll()
    {
        if (monsterPos.Count > 0)
            monsterPos.Clear();
        for (int i = 0; i < m_column; ++i)
        {
            for (int j = 0; j < m_row; ++j)
            {
                grids[i, j].Init();
            }
        }
    }

    //加载关卡-按照文件名称
    public void LoadLevel(string fileName)
    {
        StreamReader sr = new StreamReader(Application.dataPath + "/Resources/Json/Maps/" + fileName);
        string jsonStr = sr.ReadToEnd();
        sr.Close();
        MapInfo levelInfo = JsonMapper.ToObject<MapInfo>(jsonStr);
        currentBigLevel = levelInfo.bigLevelID;
        currentLevel = levelInfo.levelID;
        LoadMapAndRoad();
        monsterPos = levelInfo.monsterPath;
        roundInfo = levelInfo.roundInfo;
        for (int i = 0; i < m_column; ++i)
        {
            for (int j = 0; j < m_row; ++j)
            {
                grids[i, j].UpdateState(levelInfo.gridPoints[i * m_row + j]);
            }
        }
#if Game
        startPoint = GameController.GetInstance().GetObject(ObjectFactoryType.GameFactory, "startPoint");
        startPoint.transform.SetParent(transform);
        startPoint.transform.position = grids[monsterPos[0].xIndex, monsterPos[0].yIndex].transform.position;
        startPoint.transform.localScale = Vector3.one;
        carrot = GameController.GetInstance().GetObject(ObjectFactoryType.GameFactory, "Carrot");
        carrot.transform.SetParent(transform);
        carrot.transform.position = grids[monsterPos[monsterPos.Count - 1].xIndex, monsterPos[monsterPos.Count - 1].yIndex].transform.position;
        carrot.transform.localScale = Vector3.one;
#endif
    }

    //加载关卡-按大关卡数加小关卡数
    public void LoadLevel(int bigLevel, int level)
    {
        currentBigLevel = bigLevel;
        currentLevel = level;
        LoadLevel(currentBigLevel + "-" + currentLevel + ".json");
    }

    public void LoadMapAndRoad()
    {
#if Tool
        roadSp = Resources.Load<Sprite>("Pictures/NormalMordel/Game/" + currentBigLevel + "/Road" + currentLevel);
        bgSp = Resources.Load<Sprite>("Pictures/NormalMordel/Game/" + currentBigLevel + "/BG" + currentLevel / 4);      
#endif
#if Game
        roadSp = GameController.GetInstance().GetSprite("NormalMordel/Game/" + currentBigLevel + "/Road" + currentLevel);
        bgSp = GameController.GetInstance().GetSprite("NormalMordel/Game/" + currentBigLevel + "/BG" + currentLevel / 4);
#endif
        roadSR.sprite = roadSp;
        bgSR.sprite = bgSp;
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
