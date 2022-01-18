using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIFacade 
{
  //  public AudioManager audioManager { get; private set; }
    //场景切换
  public BaseSceneState currentScene, lastScene;
     //跳转场景使用的遮罩
    Image maskImg;
    Transform canvasTrans;

    public UIFacade()
    {
        canvasTrans = GameObject.FindObjectOfType<Canvas>().transform;
        InitMask();
    }

    void InitMask()
    {
        GameObject res = FactoryManager.GetInstance().GetObject(ObjectFactoryType.UIFactory, "Img_Mask");
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
        return FactoryManager.GetInstance().GetObject(factoryType, itemName);
    }

    public void PushObject(ObjectFactoryType factoryType, string itemName, GameObject go)
    {
        FactoryManager.GetInstance().PushObject(factoryType, itemName, go);
    }

    public Sprite GetSprite(string spriteName)
    {
       
        return FactoryManager.GetInstance().GetSprite(spriteName);
    }

    public AudioClip GetAudioClip(string clipName)
    {
        return FactoryManager.GetInstance().GetAudioClip(clipName);
    }
    #endregion

    #region AudioManager的部分函数封装

    public bool BGSwitch()
    {
       return AudioManager.GetInstance().BGSwitch();
    }

    public bool EffAudioSwitch()
    {
       return AudioManager.GetInstance().EffAudioSwitch();
    }

    public void PlayButtonAudio()
    {
        AudioManager.GetInstance().PlayButtonAudio();
    }
    #endregion

    public void AddScenePanel(string  panelName)
    {
        UIManager.GetInstance().AddScenePanel(panelName);
    }

    public void ClearScenePanelDict()
    {
        UIManager.GetInstance().ClearScenePanelDict();
    }

    public void EnterPanel(string panelName)
    {
        UIManager.GetInstance().EnterPanel(panelName);
    }

    public void ExitPanel(string panelName)
    {
        UIManager.GetInstance().ExitPanel(panelName);
    }

    public void IntoSmallLevel()
    {
        UIManager.GetInstance().IntoSmallLevel();
    }

}
