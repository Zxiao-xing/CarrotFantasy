using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager>
{
    public UIFacade mUIFacade { get; private set; }

    Transform canvasTrans;

    // 适配
    public Vector2 m_referenceResolution;
    public bool m_isFullScreen;                     // 默认为 false，为 true 的话考虑将全屏 bg 和其他元素分开了
    private Canvas m_canvas;
    private CanvasScaler m_canvasScaler;
    private float m_matchedFormToScreenRation;      // 当前的比率，可用于从屏幕位置定位 UI 位置

    /*当前场景下所需要使用到的所有Panel*/
    Dictionary<string, GameObject> currentScenePanelDict = new Dictionary<string, GameObject>();
    Stack<BasePanel> panelStack = new Stack<BasePanel>();
    protected override void Init()
    {
        m_canvas = GameObject.FindObjectOfType<Canvas>();
        m_canvasScaler = GameObject.FindObjectOfType<CanvasScaler>();
        m_referenceResolution = new Vector2(1280, 760);
        MatchScreen();

        canvasTrans = m_canvas.transform;

        mUIFacade = new UIFacade();
    }

    // 适配屏幕
    public void MatchScreen()
    {

        if (m_canvasScaler == null)
        {
            return;
        }

        Vector2 screenSize = new Vector2(ScreenCache.Width, ScreenCache.Height);

        m_canvasScaler.referenceResolution = m_referenceResolution;

        Vector2 m_screenToCanvasRatio = new Vector2();

        m_screenToCanvasRatio.x = screenSize.x / m_referenceResolution.x;
        m_screenToCanvasRatio.y = screenSize.y / m_referenceResolution.y;

        m_canvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;

        // 通常按高高比和宽宽比中高的那个进行适配，可以防止出屏
        // 对于全屏背景，为保证缩放比例，需要裁剪一部分，所以就按大的那个进行
        if (m_screenToCanvasRatio.x > m_screenToCanvasRatio.y)
        {
            // 全屏 bg 用上面
            if (m_isFullScreen)
            {
                m_canvasScaler.matchWidthOrHeight = 0f;
                m_matchedFormToScreenRation = m_screenToCanvasRatio.x;
            }
            else
            {
                m_canvasScaler.matchWidthOrHeight = 1.0f;
                m_matchedFormToScreenRation = m_screenToCanvasRatio.y;
            }
        }
        else
        {
            if (m_isFullScreen)
            {
                m_canvasScaler.matchWidthOrHeight = 1.0f;
                m_matchedFormToScreenRation = m_screenToCanvasRatio.y;
            }
            else
            {
                m_canvasScaler.matchWidthOrHeight = 0f;
                m_matchedFormToScreenRation = m_screenToCanvasRatio.x;
            }
        }

        // 刷新一下 m_canvaScaler;
        m_canvasScaler.enabled = false;
        m_canvasScaler.enabled = true;
    }

    // 启动开始场景
    public void OpenStartPanel()
    {
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
