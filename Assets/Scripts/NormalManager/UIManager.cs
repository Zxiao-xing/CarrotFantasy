using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager
{
    public UIFacade mUIFacade { get; private set; }
    Transform canvasTrans;
    /*当前场景下所需要使用到的所有Panel*/
    Dictionary<string, GameObject> currentScenePanelDict = new Dictionary<string, GameObject>();
    Stack<BasePanel> panelStack = new Stack<BasePanel>();
    public UIManager()
    {
        canvasTrans = GameObject.FindObjectOfType<Canvas>().transform;
        mUIFacade = new UIFacade(this);
        //不能在uiFacade里设置开始场景,否则会报空指针
        mUIFacade.currentScene = new StartLoadState(mUIFacade);
        //mUIFacade.currentScene = new GameNormalState(mUIFacade);
        mUIFacade.currentScene.DoBeforeEntering();
    }

    /// <summary>
    /// 增加当前场景中的面板
    /// </summary>
    public void AddScenePanel(string panelName)
    {
        GameObject panel = mUIFacade.GetObject(ObjectFactoryType.UIPanelFactory, panelName);
        /*false:不保存相对于原来父物体的坐标,坐标自动转化为现在父物体的正中心*/
        panel.transform.SetParent(canvasTrans, false);
        panel.GetComponent<BasePanel>().Init();
        if (panel.GetComponent<BasePanel>() == null)
            Debug.LogWarning(panelName + "没有BasePanel");
        currentScenePanelDict.Add(panelName, panel);
    }

    public GameObject GetScenePanel(string panelName)
    {
        if (currentScenePanelDict.ContainsKey(panelName) == false)
        {
            Debug.LogError("当前场景不存在Panle " + panelName);
            return null;
        }
        return currentScenePanelDict[panelName];
    }

    public void ClearScenePanelDict()
    {
        foreach (var item in currentScenePanelDict)
        {
            if (item.Value.activeSelf)//防止有面板没有禁用
                item.Value.GetComponent<BasePanel>().Exit();
            mUIFacade.PushObject(ObjectFactoryType.UIPanelFactory, item.Key, item.Value.gameObject);
        }
        currentScenePanelDict.Clear();
    }

    public void EnterPanel(string panelName)
    {
        if (!currentScenePanelDict.ContainsKey(panelName))
        {
            Debug.LogWarning("当前场景不存在Panel " + panelName);
            return;
        }
        currentScenePanelDict[panelName].GetComponent<BasePanel>().Enter();
    }

    public void ExitPanel(string panelName)
    {
        if (!currentScenePanelDict.ContainsKey(panelName))
        {
            Debug.LogWarning("当前场景不存在Panel " + panelName);
            return;
        }
        currentScenePanelDict[panelName].GetComponent<BasePanel>().Exit();
    }

    public void IntoSmallLevel()
    {
        currentScenePanelDict[StringManager.GameNormalOptionPanel].GetComponent<GameNormalOptionPanel>().isInBigLevel = false;
    }
}
