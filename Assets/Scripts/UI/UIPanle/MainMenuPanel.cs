using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class MainMenuPanel : BasePanel
{
    //Panel分别向左退出 向右退出
    float leftExitPosX = -1024, rightExitPosX = 1024;
    float exitPosX;

    Transform birdTrans, cloudeTrans;

    protected virtual void Awake()
    {
        cloudeTrans = transform.Find("Img_Cloud");
        birdTrans = transform.Find("Img_Bird");
        birdTrans.DOLocalMoveY(165,1.5f).SetLoops(-1,LoopType.Yoyo);
        cloudeTrans.DOLocalMoveX(640,6).SetLoops(-1, LoopType.Restart);

        Transform btns = transform.Find("Btn_Mid");
        btns.Find("Btn_Set").GetComponent<Button>().onClick.AddListener(SetBtnClick);
        btns.Find("Btn_Help").GetComponent<Button>().onClick.AddListener(HelpBtnClick);
    }

    /// <summary>
    /// 设置按钮点击后的响应函数
    /// </summary>
    public void SetBtnClick()
    {
        uIFacade.PlayButtonAudio();
        exitPosX = rightExitPosX;
        uIFacade.EnterPanel(StringManager.SetPanel);
        Exit();
    }

    public override void Exit()
    {
        transform.DOLocalMoveX(exitPosX, 0.5f);
        birdTrans.gameObject.SetActive(false);
        cloudeTrans.gameObject.SetActive(false);       
    }

    public override void Enter()
    {
        base.Enter();
        transform.DOLocalMoveX(0, 0.5f);
        birdTrans.gameObject.SetActive(true);
        cloudeTrans.gameObject.SetActive(true);
        AudioManager.GetInstance().PlayBG("Main/BGMusic");
    }

    public void HelpBtnClick()
    {
        uIFacade.PlayButtonAudio();
        exitPosX = leftExitPosX;
        Exit();
        uIFacade.EnterPanel(StringManager.HelpPanel);
    }

    /// <summary>
    /// 选择游戏模式按钮点击响应函数
    /// </summary>
    public void ChooseGameModel(int i)
    {
        uIFacade.PlayButtonAudio();
        switch (i)
        {
            case 0:
                uIFacade.ChangeScene(new GameNormalOptionState(uIFacade));
                break;
            case 1:
                uIFacade.ChangeScene(new GameBossOptionState(uIFacade));
                break;
            case 2:
                uIFacade.ChangeScene(new MonsterNestState(uIFacade));
                break;
            default:
                break;
        }
        uIFacade.EnterPanel(StringManager.GameLoadPanel);
    }

}
