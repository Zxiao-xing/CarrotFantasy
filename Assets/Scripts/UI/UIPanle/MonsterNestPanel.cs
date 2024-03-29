﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MonsterNestPanel : BasePanel
{
    int milkPrice = 1, nestPrice = 60, cookiesPrice = 10;
    Text milkTxt, nestTxt, cookiesTxt;
    [SerializeField] Text shopDiamandsTxt;
    [SerializeField] Button btnMilk, btnCookies, btnNest;
    GameObject shopGo;
    Transform babyGroupTrans;

    private void Awake()
    {
        Transform topTrans = transform.Find("Img_Top");
        milkTxt = topTrans.Find("Txt_Milk").GetComponent<Text>();
        nestTxt = topTrans.Find("Txt_Nest").GetComponent<Text>();
        cookiesTxt = topTrans.Find("Txt_Cookies").GetComponent<Text>();
        shopDiamandsTxt.text = PlayerManager.GetInstance().PlayerInfo.diamands.ToString();
        shopGo = transform.Find("ShopPage").gameObject;

    }

    public override void Enter()
    {
        base.Enter();
        AudioManager.GetInstance().PlayBG("MonsterNest/BGMusic02");
        UpdateText();
        CreateBabies();
    }

    public void ReturnToMainMenu()
    {
        uIFacade.PlayButtonAudio();
        uIFacade.ChangeScene(new MainMenuState(uIFacade));
    }

    public void BuyItems(int itemType)
    {
        uIFacade.PlayButtonAudio();
        switch (itemType)
        {
            case 1:
                PlayerManager.GetInstance().PlayerInfo.diamands -= milkPrice;
                PlayerManager.GetInstance().PlayerInfo.milk += 10;
                break;
            case 2:
                PlayerManager.GetInstance().PlayerInfo.diamands -= cookiesPrice;
                PlayerManager.GetInstance().PlayerInfo.cookies += 1;
                break;
            case 3:
                PlayerManager.GetInstance().PlayerInfo.diamands -= nestPrice;
                PlayerManager.GetInstance().PlayerInfo.nest += 1;
                break;
            default:
                break;
        }
        UpdateText();
    }

    public void ShowShop()
    {
        uIFacade.PlayButtonAudio();
        shopGo.SetActive(true);
        int diamands = PlayerManager.GetInstance().PlayerInfo.diamands;
        btnMilk.interactable = false;
        btnNest.interactable = false;
        btnCookies.interactable = false;
        if (diamands >= nestPrice)
        {
            btnMilk.interactable = true;
            btnNest.interactable = true;
            btnCookies.interactable = true;
        }
        else if (diamands >= cookiesPrice)
        {
            btnMilk.interactable = true;
            btnCookies.interactable = true;
        }
        else if (diamands >= milkPrice)
            btnMilk.interactable = true;
    }

    public void UpdateText()
    {
        milkTxt.text = PlayerManager.GetInstance().PlayerInfo.milk.ToString();
        nestTxt.text = PlayerManager.GetInstance().PlayerInfo.nest.ToString();
        cookiesTxt.text = PlayerManager.GetInstance().PlayerInfo.cookies.ToString();
        shopDiamandsTxt.text = PlayerManager.GetInstance().PlayerInfo.diamands.ToString();
    }

    void CreateBabies()
    {
        babyGroupTrans = transform.Find("Emp_Monsters");
        List<MonsterPetData> datas = PlayerManager.GetInstance().PlayerInfo.monsterPetDatasList;
        GameObject babyGo = null;
        int i = babyGroupTrans.childCount;
        for (; i < datas.Count; i++)
        {
            babyGo = FactoryManager.GetInstance().GetObject(ObjectFactoryType.UIFactory, "Emp_Monster");
            babyGo.transform.SetParent(babyGroupTrans);
            babyGo.GetComponent<MonsterBaby>().Init(datas[i]);
        }
    }

}
