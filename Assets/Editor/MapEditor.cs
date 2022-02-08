using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
[CustomEditor(typeof(MapMakerTool))]
public class MapEditor : Editor
{
    private MapMakerTool m_mapMakerTool;
    int selectedMap = -1;
    List<FileInfo> mapFiles = new List<FileInfo>();
    List<string> fileName = new List<string>();
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (!Application.isPlaying)
        {
            return;
        }
        m_mapMakerTool = MapMakerTool.GetInstance();
        LoadMapFiles();
        EditorGUILayout.BeginHorizontal();
        int currentIndex = EditorGUILayout.Popup(selectedMap, fileName.ToArray());
        selectedMap = currentIndex;
        if (GUILayout.Button("加载地图"))
        {
            m_mapMakerTool.LoadLevel(fileName[selectedMap]);
        }

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("恢复默认状态"))
        {
            m_mapMakerTool.ClearAll();
        }
        if (GUILayout.Button("清除怪物路点"))
        {
            m_mapMakerTool.ClearMonsterPos();
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("创建新地图"))
        {
            m_mapMakerTool.ClearAll();
            m_mapMakerTool.LoadMapAndRoad();
        }
        if (GUILayout.Button("保存地图数据"))
        {
            m_mapMakerTool.SaveLevel();
        }
        EditorGUILayout.EndHorizontal();
    }
    /// <summary>
    /// 读取所有的关卡信息文件以及它们的名字
    /// </summary>
    void LoadMapFiles()
    {
        //读取某一个文件夹下面所有的.json文件,放进一个string数组
        string[] files = Directory.GetFiles(Application.dataPath + "/Resources/Json/Maps/", "*.json");//*代表读取所有
        foreach (var item in files)
        {
            FileInfo fileInfo = new FileInfo(item);
            mapFiles.Add(fileInfo);
            string name = fileInfo.Name.TrimEnd(".json".ToCharArray());
            fileName.Add(name);
        }
    }
}
