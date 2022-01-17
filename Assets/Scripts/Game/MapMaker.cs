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
    public float mapHeight, mapWidth,
            gridHeight, gridWidth;
    public const int xColumn = 12, yRow = 8;

    [SerializeField] GameObject grid;
    [SerializeField] bool isDraw;
    [SerializeField] int currentBigLevel;
    [SerializeField] int currentLevel;
    [HideInInspector] public GameObject[] itemPrefabs;
    public List<Grids.GridPosIndex> monsterPos { get; private set; } = new List<Grids.GridPosIndex>();
    //每一波怪物的ID列表  如 1，1，1，2，2，2，3，3(出现几次代表有几个)
    public List<Round.RoundInfo> roundInfo = new List<Round.RoundInfo>();
    public Grids[,] grids = new Grids[xColumn, yRow];
    [HideInInspector] public Sprite monsterPosSp;
    [HideInInspector] public Sprite gridSp, gridStartSp, gridCannotBuildSp;//点击格子后显示格子三种状态资源
    Sprite roadSp, bgSp;
    SpriteRenderer bgSR, roadSR;
    GameController gameController;

    //产怪起点与终点的萝卜
    GameObject startPoint;
    public GameObject carrot { get; private set; }

    private void Awake()
    {
        gameController = GameController._Ins;
        //gameController = GetComponent<GameController>();
        bgSR = transform.Find("BG").GetComponent<SpriteRenderer>();
        roadSR = transform.Find("Road").GetComponent<SpriteRenderer>();
        CalculateSize();
#if Game
        gridSp = gameController.GetSprite("NormalMordel/Game/Grid");
        gridStartSp = gameController.GetSprite("NormalMordel/Game/StartSprite");
        gridCannotBuildSp = gameController.GetSprite("NormalMordel/Game/cantBuild");
#endif
#if Tool
         _ins = this;
        LoadGridRes();
#endif
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
                Debug.Log("错路径:" + "Prefabs/Game/1/Items/" + i);
        }
    }

    void CalculateSize()
    {
        /*相机视口坐标转为世界坐标  视口坐标:从左下角为(0,0,0)开始 到右上角为(1,1,1)结束*/
        Camera camer = Camera.main;
        Vector3 leftDown = camer.ViewportToWorldPoint(Vector3.zero);
        Vector3 rightUp = camer.ViewportToWorldPoint(Vector3.one);
        mapWidth = rightUp.x - leftDown.x;
        mapHeight = rightUp.y - leftDown.y;

        gridHeight = mapHeight / yRow;
        gridWidth = mapWidth / xColumn;
    }

    private void InitMap()
    {
        Vector3 pos;
        GameObject gridGO;
        for (int i = 0; i < xColumn; ++i)
        {
            for (int j = 0; j < yRow; ++j)
            {
                pos = CorrectPos(new Vector3(i * gridWidth, j * gridHeight, 0));
                //地图编辑时没有工厂就直接实例化
#if Tool
                GameObject gridGO= Instantiate(grid, pos, Quaternion.identity, transform);
#endif
#if Game
                gridGO = gameController.GetObject(ObjectFactoryType.GameFactory, "Grid");
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
        pos = new Vector3(pos.x - mapWidth / 2 + gridWidth / 2, pos.y - mapHeight / 2 + gridHeight / 2);
        return pos;
    }

    private void OnDrawGizmos()
    {
        if (isDraw == false)
            return;
        CalculateSize();
        Gizmos.color = Color.red;
        //画行
        for (int y = 0; y <= yRow; y++)
        {
            Vector3 startPos = new Vector3(-mapWidth / 2, -mapHeight / 2 + y * gridHeight);
            Vector3 endPos = new Vector3(mapWidth / 2, -mapHeight / 2 + y * gridHeight);
            Gizmos.DrawLine(startPos, endPos);
        }
        //画列
        for (int x = 0; x <= xColumn; x++)
        {
            Vector3 startPos = new Vector3(-mapWidth / 2 + gridWidth * x, mapHeight / 2);
            Vector3 endPos = new Vector3(-mapWidth / 2 + x * gridWidth, -mapHeight / 2);
            Gizmos.DrawLine(startPos, endPos);
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

        for (int i = 0; i < xColumn; ++i)
        {
            for (int j = 0; j < yRow; ++j)
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
        for (int i = 0; i < xColumn; ++i)
        {
            for (int j = 0; j < yRow; ++j)
            {
                grids[i, j].ClaerMonsterPos();
            }
        }
    }

    public void ClearAll()
    {
        if (monsterPos.Count > 0)
            monsterPos.Clear();
        for (int i = 0; i < xColumn; ++i)
        {
            for (int j = 0; j < yRow; ++j)
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
        for (int i = 0; i < xColumn; ++i)
        {
            for (int j = 0; j < yRow; ++j)
            {
                grids[i, j].UpdateState(levelInfo.gridPoints[i * yRow + j]);
            }
        }
#if Game
        startPoint = gameController.GetObject(ObjectFactoryType.GameFactory, "startPoint");
        startPoint.transform.SetParent(transform);
        startPoint.transform.position = grids[monsterPos[0].xIndex, monsterPos[0].yIndex].transform.position;
        startPoint.transform.localScale = Vector3.one;
        carrot = gameController.GetObject(ObjectFactoryType.GameFactory, "Carrot");
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
        roadSp = gameController.GetSprite("NormalMordel/Game/" + currentBigLevel + "/Road" + currentLevel);
        bgSp = gameController.GetSprite("NormalMordel/Game/" + currentBigLevel + "/BG" + currentLevel / 4);
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
