using UnityEngine;
using LitJson;
using System.IO;

public class JsonFactory : IResourceFactory<string>
{
    // json 文件所在的根路径
    static string m_jsonFilePath = Application.dataPath + "/Resources/Json/";

    public string GetResource(string fileName)
    {
        string path = m_jsonFilePath + fileName + ".json";
        StreamReader sr = new StreamReader(path);
        string jsonText = sr.ReadToEnd();
        sr.Close();
        if (jsonText != null)
        {
            return jsonText;
        }
        else
        {
            Debug.LogError($"Cant find the json file. file name: {fileName}");
            return "";
        }
    }

    public T GetJsonObject<T>(string fileName)
    {
        string jsonText = GetResource(fileName);
        return JsonMapper.ToObject<T>(jsonText);
    }
}
