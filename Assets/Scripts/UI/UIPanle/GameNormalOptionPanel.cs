using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameNormalOptionPanel : BasePanel
{
   public bool isInBigLevel = true;

    protected override void Start()
    {
        base.Start();
        transform.Find("Btn_Return").GetComponent<Button>().onClick.AddListener(ReturnBtnClick);
        transform.Find("Btn_Help").GetComponent<Button>().onClick.AddListener(HelpBtnClick);
    }

    public void ReturnBtnClick()
    {
        uIFacade.PlayButtonAudio();
        if (isInBigLevel)
            uIFacade.ChangeScene(new MainMenuState(uIFacade));
           
        else
        {
            uIFacade.EnterPanel(StringManager.NormalBigLevelPanel);
            uIFacade.ExitPanel(StringManager.NormalLevelPanel);
        }
        isInBigLevel = true;
    }

    public void HelpBtnClick()
    {
        uIFacade.PlayButtonAudio();
        Exit();
        if(isInBigLevel)
            uIFacade.ExitPanel(StringManager.NormalBigLevelPanel);
        else
            uIFacade.ExitPanel(StringManager.NormalLevelPanel);
        uIFacade.EnterPanel(StringManager.HelpPanel);
    }

    public override void Enter()
    {
        base.Enter();
        if (uIFacade == null)
            return;
        if (isInBigLevel)
            uIFacade.EnterPanel(StringManager.NormalBigLevelPanel);
        else
            uIFacade.EnterPanel(StringManager.NormalLevelPanel);
    }

    public override void Exit()
    {
        //这个面板已经离开 表示去往了另一场景 改变isInBigLevel值
        //防止回到该场景时2个Panel都被激活
        isInBigLevel = true;
        gameObject.SetActive(false);
    }
}
