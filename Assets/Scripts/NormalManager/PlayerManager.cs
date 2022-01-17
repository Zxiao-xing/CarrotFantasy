using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelInfo
{
    public bool IsLocked;               // 是否被锁住
    public bool IsAllClear;             // 建筑是否全部清理
    public int CarrotState;             // 获取到了啥萝卜
}

public class LevelGroupInfo
{
    public bool IsLocked;           // 是否被锁住
    public List<LevelInfo> LevelInfoList = new List<LevelInfo>();           // 包含的关卡信息
}

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

/// <summary>
/// 储存玩家所有的数据信息
/// </summary>
public class PlayerManager
{
    private PlayerInfo m_playerInfo;

    // 判断玩家是否存有数据的标签
    private static string s_hasPlayerInfoFlag = "HasPlayerInfo";

    public PlayerManager()
    {
        // 玩家不存在数据就造数据，存在数据则从文件中读取
        if (PlayerPrefs.GetInt(s_hasPlayerInfoFlag) == 0)
        {

            InitPlayerData();
            SaveData();
            PlayerPrefs.SetInt(s_hasPlayerInfoFlag, 1);
        }
        else
        {
            LoadData();
        }
    }

    /// <summary>
    /// 初始化所有数据
    /// </summary>
    public void InitPlayerData()
    {
        m_playerInfo = new PlayerInfo();
        m_playerInfo.adventureMaps = 0;
        m_playerInfo.hideLevels = 0;
        m_playerInfo.bossMaps = 0;
        m_playerInfo.coins = 1000;
        m_playerInfo.bossDefeated = 0;
        m_playerInfo.monsterDefeated = 0;
        m_playerInfo.destroyItems = 0;

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
            m_playerInfo.LevelGroupInfoList.Add(levelGroupInfo);
        }

        m_playerInfo.monsterPetDatasList = new List<MonsterPetData>();
        m_playerInfo.cookies = 33;
        m_playerInfo.milk = 22;
        m_playerInfo.nest = 0;
        m_playerInfo.diamands = 25;
    }

    // 获取玩家数据
    public PlayerInfo GetPlayerInfo()
    {
        return m_playerInfo;
    }

    // 获取玩家关卡组数据链表
    public List<LevelGroupInfo> GetPlayerLevelGroupInfoList()
    {
        return m_playerInfo.LevelGroupInfoList;
    }

    // 通过 levelGroupId 获取玩家关卡组数据
    public LevelGroupInfo GetPlayerLevelGroupInfo(uint levelGroupId)
    {
        // 潜规则，id 和链表下标是对应的
        return m_playerInfo.LevelGroupInfoList[(int)levelGroupId];
    }

    // 通过 levelGroupId 获取玩家该关卡组中关卡数据链表
    public List<LevelInfo> GetPlayerLevelInfoList(uint levelGroupId)
    {
        LevelGroupInfo levelGroupInfo = GetPlayerLevelGroupInfo(levelGroupId);
        return levelGroupInfo.LevelInfoList;
    }

    // 通过 levelGroupId 和 levelId 获取具体关卡数据
    public LevelInfo GetPlayerLevelInfo(uint levelGroupId, uint levelId)
    {
        // 潜规则，关卡组 id 以及关卡 id 都是和链表下标对应的
        List<LevelInfo> levelInfoList = GetPlayerLevelInfoList(levelGroupId);
        return levelInfoList[(int)levelId];
    }

    // 获取关卡组中已经解锁的关卡数量
    public int GetUnlockedLevelCountInLevelGroup(uint levelGroupId)
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
            // 因为解锁的都是挨着的
            break;
        }
        return count;
    }

    // 保存玩家数据
    public void SaveData()
    {
        Memento memento = new Memento();
        memento.SaveData(m_playerInfo);
    }

    // 加载玩家数据
    public void LoadData()
    {
        Memento memento = new Memento();
        m_playerInfo = memento.LoadData();
    }

}
