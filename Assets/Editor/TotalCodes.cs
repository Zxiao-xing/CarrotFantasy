using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System;
public class TotalCodes : MonoBehaviour
{
    private static int codesNum;
    private static List<string> pathList;
    [MenuItem("Tools/AllCodesNum")]
    public static void CalCodes()
    {
        codesNum = 0;
        InitPath();
        for(int i=0;i<pathList.Count;i++)
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(pathList[i]);
            HandelDirec(directoryInfo);
        }
        Debug.Log("总计 " + codesNum + " 行代码");
    }

    private static void HandelDirec(DirectoryInfo directoryInfo)
    {
        DirectoryInfo[]infos= directoryInfo.GetDirectories();
        for(int i=0;i<infos.Length;i++)
        {
            HandelDirec(infos[i]);
        }
        FileInfo[] fielInfos = directoryInfo.GetFiles();
        for (int i = 0; i < fielInfos.Length; i++)
            HandleFile(fielInfos[i]);
    }
    /*检查本行代码是否有效*/
    private static bool CheckCodes(string codeLine)
    { 
        if (codeLine == null)
            return false;
        string[] words = codeLine.Split(' ');
        //if (words.Length <=2)//小于二个单词数无效
        //    return false;
        if (codeLine.Contains("/*") || codeLine.Contains("//"))
            return false;
        return true;
    }

    private static void HandleFile(FileInfo fielInfos)
    {
        if(fielInfos.Name.EndsWith(".cs"))
        {
            int len = 0;
            StreamReader sr = new StreamReader(fielInfos.FullName);
            while (sr.EndOfStream==false)
            {
                string str = sr.ReadLine();
                
                if (CheckCodes(str))
                    len++;
            }
            Debug.Log(fielInfos.Name + "代码数目 :" + len);
            codesNum += len;
            sr.Close();
        }
    }

    private static void InitPath()
    {
        pathList = new List<string>();
        pathList.Add(Application.dataPath + "/Scripts");
    }
}
