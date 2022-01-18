using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoSingleton<GameManager>
{
    private void Update()
    {
        if (ScreenCache.Cache())
        {
            UIManager.GetInstance().MatchScreen();
        }
    }

    // 退出游戏时保存数据
    private void OnApplicationQuit()
    {
        SaveData();
    }
    protected override void Init()
    {
        //保证 Canvas、Camera 不被销毁
        Canvas canvas = FindObjectOfType<Canvas>();
        DontDestroyOnLoad(canvas.gameObject);
        DontDestroyOnLoad(Camera.main.gameObject);

        // 打开开始面板
        UIManager.GetInstance().OpenStartPanel();
    }

    public void SaveData()
    {
        PlayerManager.GetInstance().SaveData();
    }

    public void LoadData()
    {
        PlayerManager.GetInstance().LoadData();
    }




}
