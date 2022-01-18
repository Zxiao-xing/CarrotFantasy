using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;

public class LevelManager : Singleton<LevelManager>
{
    // 当前 levelGroup 和 level 的 id
    public uint LevelGroupId { get; set; }
    public uint LevelId { get; set; }
    // 第一层关卡组 id
    private static string s_startLevelGroupFileName = "StartLevelGroup";
    private List<uint> m_startLevelGroupIdList = new List<uint>();

    private static string s_levelGroupFile = "LevelGroups/";
    private static string s_levelFile = "Levels/";

    // LevelGroup 命名规则，LevelGroup_{id}，比如 id 为 0，则 LevelGroup_0。
    // Level 同理

    // 通过 id 获取 LevelGroup 的 json 文件名
    public string GetLevelGroupFileName(uint id)
    {
        return $"LevelGroup_{id}";
    }

    // 通过 id 获取 Level 的 json 文件名
    public string GetLevelFileName(uint id)
    {
        return $"Level_{id}";
    }

    // 获取第一层关卡组 id
    public List<uint> GetStartLevelGroupList()
    {
        if (m_startLevelGroupIdList.Count == 0)
        {
            string jsonText = FactoryManager.GetInstance().GetJsonTextString(s_startLevelGroupFileName);
            m_startLevelGroupIdList = JsonMapper.ToObject<List<uint>>(jsonText);
        }
        return m_startLevelGroupIdList;
    }

    // 获取第一层关卡组信息
    public List<UI_LevelGroupData> GetStartLevelGroupInfoList()
    {
        List<uint> startLevelGroupIdList = GetStartLevelGroupList();
        List<UI_LevelGroupData> levelGroupInfoList = new List<UI_LevelGroupData>();

        foreach (uint id in startLevelGroupIdList)
        {
            string jsonText = FactoryManager.GetInstance().GetJsonTextString(s_levelGroupFile + GetLevelGroupFileName(id));
            levelGroupInfoList.Add(JsonMapper.ToObject<UI_LevelGroupData>(jsonText));
        }
        return levelGroupInfoList;
    }

    // 通过关卡组 id 获取其中包含的关卡组信息
    public UI_LevelGroupData GetLevelGroupInfoByLevelGroupId(uint levelGroupId)
    {
        string jsonText = FactoryManager.GetInstance().GetJsonTextString(s_levelGroupFile + GetLevelGroupFileName(levelGroupId));
        return JsonMapper.ToObject<UI_LevelGroupData>(jsonText);
    }

    // 根据关卡组 id 获取关卡组中的关卡信息
    public List<UI_LevelData> GetLevelInfoByLevelGroupId(uint levelGroupId)
    {
        UI_LevelGroupData levelGroupInfo = GetLevelGroupInfoByLevelGroupId(levelGroupId);
        List<UI_LevelData> levelInfoList = new List<UI_LevelData>();
        uint count = levelGroupInfo.BelongLevelCount;
        for (uint i = 0; i < count; i++)
        {
            // 文件所在位置为：Levels/关卡组id/关卡id，但是关卡 id 是从 0 开始的
            string jsonText = FactoryManager.GetInstance().GetJsonTextString($"{s_levelFile}{levelGroupId}/{GetLevelFileName(i)}");
            levelInfoList.Add(JsonMapper.ToObject<UI_LevelData>(jsonText));
        }
        return levelInfoList;
    }

    // 打开某个关卡组
    public void OpenLevelGroup(uint levelGroupId)
    {
        // 获取关卡组 文件数据和玩家数据，根据数据刷新 UI
    }

    // 进入关卡
    public void OpenLevel(uint levelId)
    {
        // 获取关卡文件数据和玩家数据，根据数据刷新 UI

    }
}
