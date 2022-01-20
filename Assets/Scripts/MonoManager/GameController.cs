using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoSingleton<GameController>
{
    public int CurLevelGroup { get; private set; }
    public int CurLevel { get; private set; }
    public MapMaker mapMaker { get; private set; }
    public bool isStop { get; private set; } = false;

    public int playSpeed { get; private set; } = 3;
    [HideInInspector]
    public Round.RoundInfo nowRoundInfo;
    private UI_LevelData m_curLevelData;
    public int coins;

    GameObject monster, bornEff;
    int monsterNums;
    Vector3 startPos;
    Level level;
    private int diedMonsterCount;//死亡怪物数目 用于回合数的切换
    [HideInInspector]
    public Grid selectedGrid;

    //可以建造的炮塔面板  炮塔升级面板
    public GameObject towerList, towerShow;
    //显示塔升级还是拆除面板上按钮的四个位置
    public Transform btnUp, btnDown, btnLeft, btnRight;
    public Transform towerUpTrans, towerDesTrans;
    //每一类型防御塔的价钱,todo：用塔的id来对应塔的信息
    public int[] towerPrice = { 100, 120, 140, 160, 160 };

    [SerializeField] GameObject firePoint;//集火点物品图标
    [HideInInspector]
    public Transform fireTrans;//集火目标的transform

    MonsterBuilder monsterBuilder;

    NormalModelPanel normalModelPanel;
    int nowRoundIndex;
    #region 生命周期函数
    protected override void Init()
    {
        mapMaker = MapMaker.GetInstance();
        //下标从0开始但是json文件存储的关卡名字从1开始编号
        CurLevelGroup = (int)LevelManager.GetInstance().LevelGroupId + 1;
        CurLevel = (int)LevelManager.GetInstance().LevelId + 1;

        m_curLevelData = LevelManager.GetInstance().GetLevelInfoByLevelGroupId(CurLevelGroup - 1)[CurLevel - 1];
        monsterBuilder = new MonsterBuilder();
    }

    private void Start()
    {
        coins = PlayerManager.GetInstance().PlayerInfo.coins;
        mapMaker.LoadLevel(CurLevelGroup, CurLevel);
        Invoke("DelayToStart", 3);
        LoadTowerButtons();

        normalModelPanel = UIManager.GetInstance().GetScenePanel(StringManager.NormalModelPanel).GetComponent<NormalModelPanel>();

        normalModelPanel.coinTxt.text = coins.ToString();
        normalModelPanel.waveTxt.text = "0   " + 1.ToString();
        normalModelPanel.allWaveTxt.text = "/" + m_curLevelData.TotalWave.ToString();
    }

    /// <summary>
    /// 开始游戏钱要播放一个三秒动画 所以延迟调用
    /// </summary>
    void DelayToStart()
    {
        level = new Level(mapMaker.roundInfo);
        AudioManager.GetInstance().PlayBG("NormalMordel/");
    }

    private void Update()
    {
        if (IsInvoking("Creat") == false && isStop == false && monsterNums != nowRoundInfo.mMonsterIDList.Count)
            InvokeRepeating("Creat", 1, 1);
        if (diedMonsterCount != 0 && diedMonsterCount == monsterNums)
            NextRound();
    }
    #endregion

    /// <summary>
    /// 生成本回合可以建造的炮塔按钮
    /// </summary>
    void LoadTowerButtons()
    {
        GameObject go;
        for (int i = 0; i < m_curLevelData.TowerIdList.Count; i++)
        {
            go = GetObject(ObjectFactoryType.UIFactory, "Btn_Tower");
            go.GetComponent<ButtonTower>().towerID = (int)m_curLevelData.TowerIdList[i];
            go.transform.SetParent(towerList.transform);
            //必须要再次设置一下物体的位置与大小
            go.transform.localPosition = Vector3.zero;
            go.transform.localScale = Vector3.one;
        }
    }

    // todo：移到 MapMaker 里面去把
    public void CreatMonster(Round.RoundInfo roundInfo)
    {
        startPos = mapMaker.grids[mapMaker.monsterPos[0].xIndex, mapMaker.monsterPos[0].yIndex].transform.position;
        monsterNums = 0;
        diedMonsterCount = 0;
        nowRoundInfo = roundInfo;
        // 出生特效
        bornEff = GetObject(ObjectFactoryType.GameFactory, "BornEff");
        bornEff.transform.SetParent(transform);
        bornEff.transform.position = startPos;
        InvokeRepeating("Creat", (float)1 / playSpeed, (float)1 / playSpeed);
    }

    //两种状态下杀死怪物:1.没到终点   2.到了终点
    public void KillMonster(bool isReach, Vector3 monsterPosition)
    {
        diedMonsterCount++;
        if (isReach)
            mapMaker.m_carrot.GetComponent<Carrot>().SubtractHP();
        else//怪物是被炮塔击杀 随机奖励物品
        {
            int temp = Random.Range(1, 100);
            if (temp < 10)
            {
                GameObject prizeGo = GetObject(ObjectFactoryType.GameFactory, "Prize");
                prizeGo.transform.position = monsterPosition;
            }
        }
    }

    public void GivePrize()
    {
        int temp;
        do//如果随机出来的奖励是怪物就检查
        {//玩家已有怪物数量是否已经满了
            temp = Random.Range(1, 4);
        }
        while (temp == 4 && PlayerManager.GetInstance().PlayerInfo.monsterPetDatasList.Count >= 3);
        normalModelPanel.ShowPrize(temp);
    }

    void Creat()
    {
        if (isStop == true)
        {
            CancelInvoke();
            return;
        }
        if (monsterNums == nowRoundInfo.mMonsterIDList.Count)
        {
            PushObject(ObjectFactoryType.GameFactory, "BornEff", bornEff);
            CancelInvoke();
            return;
        }

        monsterBuilder.monsterID = nowRoundInfo.mMonsterIDList[monsterNums];
        monster = monsterBuilder.GetProducts();
        monster.transform.SetParent(transform);
        monster.transform.position = startPos;
        monsterNums++;
    }

    #region 游戏逻辑相关
    public void ClickGrid(Grid grid)
    {
        if (grid == selectedGrid)
        {
            selectedGrid.HideGrid();
            selectedGrid = null;
        }
        else
        {
            if (selectedGrid != null)
                selectedGrid.HideGrid();
            selectedGrid = grid;
            selectedGrid.ShowGrid();
        }
    }

    public void SetFirePoint(Transform fireTransform)
    {
        if (firePoint.activeSelf == false)
            firePoint.SetActive(true);
        fireTrans = fireTransform;
        firePoint.transform.SetParent(fireTransform);
        firePoint.transform.position = fireTransform.position;
        Vector3 pos = firePoint.transform.position;
        pos.y += mapMaker.m_gridHeight / 2;
        firePoint.transform.position = pos;
    }

    public void HideFirePoint()
    {
        firePoint.transform.SetParent(transform);
        firePoint.SetActive(false);

    }

    public void ChangeCoin(int money)
    {
        coins += money;
        normalModelPanel.coinTxt.text = coins.ToString();
    }

    public int ChangePlaySpeed()
    {
        if (playSpeed == 1)
            playSpeed = 2;
        else
            playSpeed = 1;
        return playSpeed - 1;
    }

    public void StopGoOnGame(bool isStop)
    {
        this.isStop = isStop;
    }

    void NextRound()
    {
        diedMonsterCount = 0;
        nowRoundIndex = level.NextRound();//进入下一关
        if (nowRoundIndex >= m_curLevelData.TotalWave)//已经游戏结束 不更新UI
        {
            return;
        }
        if (nowRoundIndex + 1 < 10)
        {
            normalModelPanel.waveTxt.text = "0   " + (nowRoundIndex + 1).ToString();
        }
        else
        {
            normalModelPanel.waveTxt.text = ((nowRoundIndex + 1) / 10).ToString() + "   " + ((nowRoundIndex + 1) % 10).ToString();
        }
        if (nowRoundIndex == m_curLevelData.TotalWave - 1)
        {
            normalModelPanel.FinalWave();
        }
    }

    public void GameOver(bool isVictory)
    {
        normalModelPanel.StopGoOnGame(true);
        int waves = nowRoundIndex + 1;
        int allwaves = (int)m_curLevelData.TotalWave;
        Sprite carrotSp = null, gameModeSp;
        if (UIManager.GetInstance().mUIFacade.currentScene.GetType() == typeof(GameNormalState))
            gameModeSp = GetSprite("NormalMordel/GameOverAndWin/gameover0-hd_10");
        else//没有图片资源了 所以就这样吧
            gameModeSp = GetSprite("NormalMordel/GameOverAndWin/gameover0-hd_10");
        if (isVictory)//只有是胜利结局才去加载萝卜奖励图标资源
        {
            int hp = mapMaker.m_carrot.GetComponent<Carrot>().nowHP;
            if (hp >= 8)
                carrotSp = GetSprite("GameOption/Normal/Level/Carrot_1");
            else if (hp >= 5)
                carrotSp = GetSprite("GameOption/Normal/Level/Carrot_2");
            else
                carrotSp = GetSprite("GameOption/Normal/Level/Carrot_3");
        }
        normalModelPanel.GameOverUI(isVictory, waves, allwaves, gameModeSp, carrotSp);
    }

    #endregion

    #region factoryManager的函数封装
    public GameObject GetObject(ObjectFactoryType factoryType, string itemPath)
    {
        return FactoryManager.GetInstance().GetObject(factoryType, itemPath);
    }

    public void PushObject(ObjectFactoryType factoryType, string itemPath, GameObject go)
    {
        FactoryManager.GetInstance().PushObject(factoryType, itemPath, go);
    }

    public Sprite GetSprite(string spritePath)
    {
        return FactoryManager.GetInstance().GetSprite(spritePath);
    }

    public AudioClip GetAudioClip(string clipPath)
    {
        return FactoryManager.GetInstance().GetAudioClip(clipPath);
    }

    public RuntimeAnimatorController GetRunTimeController(string controllerPath)
    {
        return FactoryManager.GetInstance().GetRuntimeAnimatorController(controllerPath);
    }
    #endregion

}

