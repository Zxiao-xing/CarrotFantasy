using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 玩家数据包含得关卡信息
public class LevelInfo
{
    public bool IsLocked;               // 是否被锁住
    public bool IsAllClear;             // 建筑是否全部清理
    public int CarrotState;             // 获取到了啥萝卜
}

// 玩家数据包含得关卡组信息
public class LevelGroupInfo
{
    public bool IsLocked;           // 是否被锁住
    public List<LevelInfo> LevelInfoList = new List<LevelInfo>();           // 包含的关卡信息
}

// 玩家信息
public class PlayerInfo
{
    //帮助界面的数据表数据
    public int adventureMaps, hideLevels, bossMaps,
          coins, bossDefeated, monsterDefeated, destroyItems;

    public List<LevelGroupInfo> LevelGroupInfoList = new List<LevelGroupInfo>();

    /*怪物窝数据*/
    //玩家已经获得的宠物怪物
    public List<MonsterPetData> monsterPetDatasList = new List<MonsterPetData>();
    public int cookies, milk, nest, diamands;
}

// 怪物数据
public class MonsterInfo
{
    public int Id;                  // 怪物 id
    public string Name;             // 怪物名字
    public string AnimatorName;     // 怪物动画控制器名字
    public int Damage;              // 怪物对萝卜造成的伤害
    public int Hp;                  // 怪物 hp
    public float Speed;             // 怪物默认速度
    public int Coin;                // 击杀可获得的游戏机金币
}

public class TowerInfo
{
    public int Id;
    public string Name;
    public int BuyPrice;            // 购买金额，升级后就按统一的比例上调？
    public int SellPrice;           // 出售金额，升级后就按统一的比例上调？
    public int AttackRange;         // 攻击范围，
}

/// <summary>
/// 玩家所需的信息管理，包括玩家数据和其他需要从文件中读取的数据信息
/// </summary>
public class PlayerManager : Singleton<PlayerManager>
{
    // 玩家数据
    public PlayerInfo PlayerInfo { get; set; }
    // 怪物数据，id 和数据的字典
    public Dictionary<int, MonsterInfo> MonsterInfoDict { get; private set; }

    // 判断玩家是否存有数据的标签
    private static string s_hasPlayerInfoFlag = "HasPlayerInfo";

    private static string s_playerInfoPath = "PlayerData";                  // 玩家数据存储的路径
    private static string s_monsterInfoPath = "MonsterData";                // 怪物数据存储的路径

    public PlayerManager()
    {
        // 玩家不存在数据就造数据，存在数据则从文件中读取
        if (PlayerPrefs.GetInt(s_hasPlayerInfoFlag) == 0)
        {
            InitPlayerData();
            SavePlayerData();
            PlayerPrefs.SetInt(s_hasPlayerInfoFlag, 1);
        }
        else
        {
            LoadPlayerData();
        }

        // 从配置表中加载怪物数据
        LoadMonsterData();
    }

    /// <summary>
    /// 初始化玩家数据
    /// </summary>
    private void InitPlayerData()
    {
        PlayerInfo = new PlayerInfo();
        PlayerInfo.adventureMaps = 0;
        PlayerInfo.hideLevels = 0;
        PlayerInfo.bossMaps = 0;
        PlayerInfo.coins = 1000;
        PlayerInfo.bossDefeated = 0;
        PlayerInfo.monsterDefeated = 0;
        PlayerInfo.destroyItems = 0;

        // 初始化关卡组数据
        for (int i = 0; i < 3; i++)          // todo：关卡组总数从文件获取，这里写死了
        {
            LevelGroupInfo levelGroupInfo = new LevelGroupInfo();
            // 第一个关卡组开始就要解锁
            if (i == 0)
            {
                levelGroupInfo.IsLocked = false;
            }
            else
            {
                levelGroupInfo.IsLocked = true;
            }

            for (int j = 0; j < 5; j++)         // todo：关卡组总数从文件获取，这里写死了
            {
                LevelInfo levelInfo = new LevelInfo();
                // 第一个关卡开始就要解锁
                if (j == 0)
                {
                    levelInfo.IsLocked = false;
                }
                else
                {
                    levelInfo.IsLocked = true;
                }
                levelInfo.IsAllClear = false;
                levelInfo.CarrotState = 0;
                levelGroupInfo.LevelInfoList.Add(levelInfo);
            }
            PlayerInfo.LevelGroupInfoList.Add(levelGroupInfo);
        }

        PlayerInfo.monsterPetDatasList = new List<MonsterPetData>();
        PlayerInfo.cookies = 33;
        PlayerInfo.milk = 22;
        PlayerInfo.nest = 0;
        PlayerInfo.diamands = 25;
    }

    // 加载玩家数据
    private void LoadPlayerData()
    {
        Memento memento = new Memento();
        PlayerInfo = memento.LoadData();
    }

    // 保存玩家数据
    public void SavePlayerData()
    {
        Memento memento = new Memento();
        memento.SaveData(PlayerInfo);
    }

    // 加载怪物数据
    private void LoadMonsterData()
    {
        List<MonsterInfo> monsterInfoList = FactoryManager.GetInstance().GetJsonObject<List<MonsterInfo>>(s_monsterInfoPath);
        MonsterInfoDict = new Dictionary<int, MonsterInfo>();
        foreach (MonsterInfo info in monsterInfoList)
        {
            MonsterInfoDict.Add(info.Id, info);
        }
    }

    // 获取玩家关卡组数据链表
    public List<LevelGroupInfo> GetPlayerLevelGroupInfoList()
    {
        return PlayerInfo.LevelGroupInfoList;
    }

    // 通过 levelGroupId 获取玩家关卡组数据
    public LevelGroupInfo GetPlayerLevelGroupInfo(int levelGroupId)
    {
        // 潜规则，id 和链表下标是对应的
        return PlayerInfo.LevelGroupInfoList[levelGroupId];
    }

    // 通过 levelGroupId 获取玩家该关卡组中关卡数据链表
    public List<LevelInfo> GetPlayerLevelInfoList(int levelGroupId)
    {
        LevelGroupInfo levelGroupInfo = GetPlayerLevelGroupInfo(levelGroupId);
        return levelGroupInfo.LevelInfoList;
    }

    // 通过 levelGroupId 和 levelId 获取具体关卡数据
    public LevelInfo GetPlayerLevelInfo(int levelGroupId, int levelId)
    {
        // 潜规则，关卡组 id 以及关卡 id 都是和链表下标对应的
        List<LevelInfo> levelInfoList = GetPlayerLevelInfoList(levelGroupId);
        return levelInfoList[levelId];
    }

    // 获取关卡组中已经解锁的关卡数量
    public int GetUnlockedLevelCountInLevelGroup(int levelGroupId)
    {
        LevelGroupInfo levelGroupInfo = GetPlayerLevelGroupInfo(levelGroupId);
        int count = 0;
        // 关卡组未解锁说明里面的关卡没有一个解锁的
        if (levelGroupInfo.IsLocked)
        {
            return count;
        }
        for (int i = 0; i < levelGroupInfo.LevelInfoList.Count; i++)
        {
            if (levelGroupInfo.LevelInfoList[i].IsLocked == false)
            {
                count++;
            }
            // 因为解锁的都是挨着的，所以遇到没解锁的后面就都是没解锁的，可以直接跳过了
            break;
        }
        return count;
    }


}
