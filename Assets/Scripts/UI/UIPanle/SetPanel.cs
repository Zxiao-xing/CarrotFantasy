using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class SetPanel : BasePanel
{
    public Sprite[] sprites;//放置音效按钮图片 0-音效开 1-音效关 2-BG开 3- BG关
    Image bgImg, effAudioImg;
    GameObject[] pages;
    GameObject resetPage;
    public Text[] staticTexts;

    float exitPosX = -1024;

    public override void Init()
    {
        transform.localPosition = new Vector3(-1024, 0, 0);
    }

    protected void Awake()
    {
        transform.Find("Btns").Find("Btn_Return").GetComponent<Button>().onClick.AddListener(ReturnMainMenu);
        Transform btns = transform.Find("OptionPage");
        btns.Find("Btn_BGAudio").GetComponent<Button>().onClick.AddListener(BGSwitch);
        btns.Find("Btn_EffectAudio").GetComponent<Button>().onClick.AddListener(EffAudioSwitch);
        btns.Find("Btn_ResetGame").GetComponent<Button>().onClick.AddListener(ShowResetTip);

        bgImg = btns.Find("Btn_BGAudio").GetComponent<Image>();
        effAudioImg = btns.Find("Btn_EffectAudio").GetComponent<Image>();

        pages = new GameObject[3];
        pages[0] = transform.Find("OptionPage").gameObject;
        pages[1] = transform.Find("StaticPage").gameObject;
        pages[2] = transform.Find("ProducerPage").gameObject;

        resetPage = transform.Find("OptionPage/ResetPage").gameObject;
    }

    protected override void Start()
    {
        base.Start();
        UpdateStatic();
    }

    public void ReturnMainMenu()
    {
        uIFacade.PlayButtonAudio();
        uIFacade.EnterPanel(StringManager.MainPanel);
        Exit();
    }

    #region OptionPage函数

    public void BGSwitch()
    {
        bool isPlayBG = uIFacade.BGSwitch();
        if (isPlayBG)
            bgImg.sprite = sprites[2];
        else
            bgImg.sprite = sprites[3];
    }

    public void EffAudioSwitch()
    {
        bool isPlayEffAudio = uIFacade.EffAudioSwitch();
        if (isPlayEffAudio)
            effAudioImg.sprite = sprites[0];
        else
            effAudioImg.sprite = sprites[1];
    }

    public void ChoosePage(int i)
    {
        if (uIFacade != null)//第一次进入时防止空指针异常
            uIFacade.PlayButtonAudio();
        for (int j = 0; j < pages.Length; ++j)
        {
            if (j == i)
            {
                pages[j].SetActive(true);
            }
            else
                pages[j].SetActive(false);
        }
    }

    public void ShowResetTip()
    {
        resetPage.SetActive(true);
    }
    #endregion

    private void UpdateStatic()
    {
        staticTexts[0].text = PlayerManager.GetInstance().GetPlayerInfo().adventureMaps.ToString();
        staticTexts[1].text = PlayerManager.GetInstance().GetPlayerInfo().hideLevels.ToString();
        staticTexts[2].text = PlayerManager.GetInstance().GetPlayerInfo().bossMaps.ToString();
        staticTexts[3].text = PlayerManager.GetInstance().GetPlayerInfo().coins.ToString();
        staticTexts[4].text = PlayerManager.GetInstance().GetPlayerInfo().monsterDefeated.ToString();
        staticTexts[5].text = PlayerManager.GetInstance().GetPlayerInfo().bossDefeated.ToString();
        staticTexts[6].text = PlayerManager.GetInstance().GetPlayerInfo().destroyItems.ToString();
    }

    public override void Enter()
    {
        base.Enter();
        transform.DOLocalMoveX(0, 0.5f);
    }

    public override void Exit()
    {
        transform.DOLocalMoveX(exitPosX, 0.5f).OnComplete(() => gameObject.SetActive(false));
    }
}
