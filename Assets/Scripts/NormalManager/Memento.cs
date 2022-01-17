using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;
using System.IO;
public class Memento
{
    string filePath = Application.dataPath + "/Resources/Json/PlayerData.json";

    public void SaveData(PlayerInfo playerManager)
    {
        string jsonStr = JsonMapper.ToJson(playerManager);
        File.WriteAllText(filePath, jsonStr);
    }

    public PlayerInfo LoadData()
    {
        if (File.Exists(filePath) == false)
        {
            Debug.LogError("不存在文本文件 " + filePath);
            return null;
        }
        string jsonStr = File.ReadAllText(filePath);
        return JsonMapper.ToObject<PlayerInfo>(jsonStr);
    }

}
