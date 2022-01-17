using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
public class HelpPanel : BasePanel
{
    float exitXPos = 1024;
    GameObject[] pages = new GameObject[3];
    protected void Awake()
    {
        transform.Find("Btns/Btn_Return").GetComponent<Button>().onClick.AddListener(ReturnToMainMenu);

        pages[0] = transform.Find("HelpPage").gameObject;
        pages[1] = transform.Find("TowerPage").gameObject;
        pages[2] = transform.Find("MonsterPage").gameObject;
    }
    /// <summary>
    /// 该物体被创建出来时,外界调用此函数初始化某些数据
    /// </summary>
    public override void Init()
    {
        transform.localPosition = new Vector3(1024, 0, 0);
        transform.SetSiblingIndex(10);
        ChoosePage(0);
    }

    void ReturnToMainMenu()
    {
        uIFacade.PlayButtonAudio();
        Exit();

        if (uIFacade.currentScene is MainMenuState)
            uIFacade.EnterPanel(StringManager.MainPanel);
        else if (uIFacade.currentScene is GameNormalOptionState)
        {
            uIFacade.EnterPanel(StringManager.GameNormalOptionPanel);
        }
    }

    public void ChoosePage(int i)
    {
        if (uIFacade != null)//第一次进入时防止空指针异常
            uIFacade.PlayButtonAudio();
        for (int j = 0; j < pages.Length; ++j)
        {
            if (j == i)
                pages[j].SetActive(true);
            else
                pages[j].SetActive(false);
        }
    }

    public override void Enter()
    {
        base.Enter();
        transform.DOLocalMoveX(0, 0.5f);
    }

    public override void Exit()
    {
        transform.DOLocalMoveX(exitXPos, 0.5f).OnComplete(() => gameObject.SetActive(false));
    }
}
