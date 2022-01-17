﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

/// <summary>
/// 正常模式中战斗场景Panel
/// </summary>
public class NormalModelPanel : BasePanel
{
    //这个面板实例化是在场景跳转之前的,所以不能去引用GameController
    //直接使用._Ins就好了 获取资源也是使用的GameManager
    // GameController gameController;
    Sprite[] playSpeedSprites = new Sprite[2];
    Sprite[] stopSprites = new Sprite[2];
    Image speedImg, stopImg;
    GameObject FinalWaveGo;

    public Text coinTxt, waveTxt, allWaveTxt;
    [SerializeField]
    GameObject winUIGo, defeatUIGo;

    [SerializeField]
    Text winAllWaveTxt, winwaveTxt,
        defAllWaveTxt, defwaveTxt;
    [SerializeField] Image winCarrotImg, winModeImg, defModeImg;

    private GameObject prizeUIGo;
    [SerializeField] Image prizeDesImg, prizeIconImg;
    [SerializeField] Text prizeNameTxt;

    protected override void Start()
    {
        base.Start();
        playSpeedSprites[0] = GameManager._Ins.factoryManager.GetSprite("NormalMordel/touming-hd.pvr_7");
        playSpeedSprites[1] = GameManager._Ins.factoryManager.GetSprite("NormalMordel/touming-hd.pvr_9");
        speedImg = transform.Find("Img_Top/Btn_Speed").GetComponent<Image>();

        stopSprites[0] = GameManager._Ins.factoryManager.GetSprite("NormalMordel/touming-hd.pvr_11");
        stopSprites[1] = GameManager._Ins.factoryManager.GetSprite("NormalMordel/touming-hd.pvr_12");
        stopImg = transform.Find("Img_Top/Btn_Pause").GetComponent<Image>();

        prizeUIGo = transform.Find("PrizeUI").gameObject;
        FinalWaveGo = transform.Find("Img_FinalWave").gameObject;

    }

    public void ChangePlaySpeed()
    {
        int index = GameController._Ins.ChangePlaySpeed();
        speedImg.sprite = playSpeedSprites[index];
    }

    //暂停或者继续游戏 类似于开关(按钮响应)
    public void StopGoOnGame()
    {
        StopGoOnGame(!GameController._Ins.isStop);
    }

    //暂停或者继续游戏(按钮响应)
    public void StopGoOnGame(bool isStop)
    {
        GameController._Ins.StopGoOnGame(isStop);
        if (isStop)
            stopImg.sprite = stopSprites[1];
        else
            stopImg.sprite = stopSprites[0];
    }

    public void ShowPrize(int prizeType)
    {
        StopGoOnGame(true);
        prizeDesImg.sprite = GameController._Ins.GetSprite("MonsterNest/Prize/Instruction" + prizeType);
        prizeIconImg.sprite = GameController._Ins.GetSprite("MonsterNest/Prize/Prize" + prizeType);
        string _text = "";
        switch (prizeType)
        {
            case 1:
                _text = "牛 奶";
                GameManager._Ins.playerManager.GetPlayerInfo().milk++;
                break;
            case 2:
                _text = "饼 干";
                GameManager._Ins.playerManager.GetPlayerInfo().cookies++;
                break;
            case 3:
                GameManager._Ins.playerManager.GetPlayerInfo().nest++;
                _text = "怪物窝";
                break;
            case 4:
                _text = "神秘蛋";
                GameManager._Ins.playerManager.GetPlayerInfo().monsterPetDatasList.
                    Add(new MonsterPetData
                    {
                        monsterID = prizeType,
                        remainMilk = 0,
                        monsterLevel = 0,
                        remainCookies = 0
                    });
                break;
            default:
                break;
        }
        prizeNameTxt.text = _text;
        prizeUIGo.SetActive(true);
    }

    /// <summary>
    /// 最后一波怪物时的过场动画
    /// </summary>
    public void FinalWave()
    {
        GameManager._Ins.audioManager.PlayEffAudio("NormalMordel/Tower/Finalwave");
        GameController._Ins.StopGoOnGame(true);
        FinalWaveGo.SetActive(true);
        FinalWaveGo.transform.localPosition = new Vector3(-960, 0, 0);
        FinalWaveGo.transform.DOLocalMoveX(0, 1.5f).OnComplete(HideFinalWave);
    }

    void HideFinalWave()
    {
        FinalWaveGo.GetComponent<Image>().DOFade(0, 0.8f).OnComplete(() => GameController._Ins.StopGoOnGame(false));
    }

    //重新开始本关卡(按钮响应)
    public void Restart()
    {
        uIFacade.ChangeScene(new GameNormalState(uIFacade));
    }

    //去往选择关卡场景(按钮响应)
    public void ToChooseLevel()
    {
        uIFacade.ChangeScene(new GameNormalOptionState(uIFacade));
    }

    public void GameOverUI(bool isVictory, int waves, int allwaves, Sprite gameModeSp, Sprite carrotSp = null)
    {
        if (isVictory)
        {
            winUIGo.SetActive(true);
            if (waves > 9)
                winwaveTxt.text = (waves / 10).ToString() + "    " + (waves % 10);
            else
                winwaveTxt.text = "0    " + waves.ToString();
            winAllWaveTxt.text = allwaves.ToString();
            winCarrotImg.sprite = carrotSp;
            winModeImg.sprite = gameModeSp;
        }
        else
        {
            defeatUIGo.SetActive(true);
            if (waves > 9)
                defwaveTxt.text = (waves / 10).ToString() + "    " + (waves % 10).ToString();
            else
                defwaveTxt.text = "0    " + waves.ToString();
            defAllWaveTxt.text = allwaves.ToString();
            defModeImg.sprite = gameModeSp;
        }
    }

}
