using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/*管理所有管理者的最终端  (是一个单例模式)*/
public class GameManager : MonoBehaviour
{
    /*所有管理者只在这里生成一次,是一种伪单例*/
    public FactoryManager factoryManager { get; private set; }
    public PlayerManager playerManager { get; private set; }
    public UIManager uIManager { get; private set; }
    public AudioManager audioManager { get; private set; }

    private static GameManager _ins;
    public static GameManager _Ins { get => _ins; }

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        //保证Canvas不被销毁
        Canvas canvas = FindObjectOfType<Canvas>();
        DontDestroyOnLoad(canvas.gameObject);
        DontDestroyOnLoad(Camera.main.gameObject);
        _ins = this;
        playerManager = new PlayerManager();
        factoryManager = new FactoryManager();
        audioManager = new AudioManager();
        uIManager = new UIManager();
    }

    public void SaveData()
    {
        playerManager.SaveData();
    }

    public void LoadData()
    {
        playerManager.LoadData();
    }

    // 退出游戏时保存数据
    private void OnApplicationQuit()
    {
        SaveData();
    }
}
