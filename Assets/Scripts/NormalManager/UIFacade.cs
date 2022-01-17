using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIFacade 
{
   public UIManager uIManager { get; private set; }
    FactoryManager factoryManager;
    public PlayerManager playerManager { get; private set; }
  //  public AudioManager audioManager { get; private set; }
    //场景切换
  public BaseSceneState currentScene, lastScene;
     //跳转场景使用的遮罩
    Image maskImg;
    Transform canvasTrans;

    public UIFacade(UIManager uimanager)
    {
        GameManager gameManager = GameManager._Ins;
        //uimanager的赋值必须这样,否则uimanager的初始化未完成,得到的uimanager是null
        playerManager = gameManager.playerManager;
        uIManager = uimanager;
        factoryManager = gameManager.factoryManager;
        canvasTrans = GameObject.FindObjectOfType<Canvas>().transform;
        InitMask();
    }

    void InitMask()
    {
        GameObject res = factoryManager.GetObject(ObjectFactoryType.UIFactory, "Img_Mask");
        maskImg = res.GetComponent<Image>();
        res.transform.SetParent(canvasTrans,false);
    }

    #region 场景状态切换
    public void ChangeScene(BaseSceneState newScene)
    {
        lastScene = currentScene;
        currentScene = newScene;
       ExitLastScene();
    }

     void ExitLastScene()
    {      
        ShowMask();
    }

    void ShowMask()
    {
        //设置物体的渲染顺序 越大越后渲染
        maskImg.transform.SetSiblingIndex(20);
        maskImg.gameObject.SetActive(true);      
        maskImg.DOFade(1, 1).OnComplete(() =>
        {
            lastScene.DoBeforeLeaving();            
        });   
    }

    //遮罩完全显示后,在SceneState的DoBeforeleaving中调用
   public  void EnterNewScene()
    {
       currentScene.DoBeforeEntering();
        HideMask();
    }

    void HideMask()
    {
        //设置物体的渲染顺序 越大越后渲染
        maskImg.transform.SetSiblingIndex(20);
        
        maskImg.DOFade(0, 1).OnComplete(() => maskImg.gameObject.SetActive(false));
    }
    #endregion

    #region factoryManager的函数封装
    public GameObject GetObject(ObjectFactoryType factoryType, string itemName)
    {
        return factoryManager.GetObject(factoryType, itemName);
    }

    public void PushObject(ObjectFactoryType factoryType, string itemName, GameObject go)
    {
        factoryManager.PushObject(factoryType, itemName, go);
    }

    public Sprite GetSprite(string spriteName)
    {
       
        return factoryManager.GetSprite(spriteName);
    }

    public AudioClip GetAudioClip(string clipName)
    {
        return factoryManager.GetAudioClip(clipName);
    }
    #endregion

    #region AudioManager的部分函数封装

    public bool BGSwitch()
    {
       return GameManager._Ins.audioManager.BGSwitch();
    }

    public bool EffAudioSwitch()
    {
       return GameManager._Ins.audioManager.EffAudioSwitch();
    }

    public void PlayButtonAudio()
    {
        GameManager._Ins.audioManager.PlayButtonAudio();
    }
    #endregion

    public void AddScenePanel(string  panelName)
    {
        uIManager.AddScenePanel(panelName);
    }

    public void ClearScenePanelDict()
    {
        uIManager.ClearScenePanelDict();
    }

    public void EnterPanel(string panelName)
    {
        uIManager.EnterPanel(panelName);
    }

    public void ExitPanel(string panelName)
    {
        uIManager.ExitPanel(panelName);
    }

    public void IntoSmallLevel()
    {
        uIManager.IntoSmallLevel();
    }

}
