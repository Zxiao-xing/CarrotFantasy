using System.Collections;
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
        shopDiamandsTxt.text = GameManager._Ins.playerManager.GetPlayerInfo().diamands.ToString();
        shopGo = transform.Find("ShopPage").gameObject;

    }

    public override void Enter()
    {
        base.Enter();
        GameManager._Ins.audioManager.PlayBG("MonsterNest/BGMusic02");
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
        PlayerManager gm = GameManager._Ins.playerManager;
        switch (itemType)
        {
            case 1:
                gm.GetPlayerInfo().diamands -= milkPrice;
                gm.GetPlayerInfo().milk += 10;
                break;
            case 2:
                gm.GetPlayerInfo().diamands -= cookiesPrice;
                gm.GetPlayerInfo().cookies += 1;
                break;
            case 3:
                gm.GetPlayerInfo().diamands -= nestPrice;
                gm.GetPlayerInfo().nest += 1;
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
        int diamands = GameManager._Ins.playerManager.GetPlayerInfo().diamands;
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
        PlayerManager gm = GameManager._Ins.playerManager;
        milkTxt.text = gm.GetPlayerInfo().milk.ToString();
        nestTxt.text = gm.GetPlayerInfo().nest.ToString();
        cookiesTxt.text = gm.GetPlayerInfo().cookies.ToString();
        shopDiamandsTxt.text = gm.GetPlayerInfo().diamands.ToString();
    }

    void CreateBabies()
    {
        babyGroupTrans = transform.Find("Emp_Monsters");
        List<MonsterPetData> datas = GameManager._Ins.playerManager.GetPlayerInfo().monsterPetDatasList;
        GameObject babyGo = null;
        int i = babyGroupTrans.childCount;
        for (; i < datas.Count; i++)
        {
            babyGo = GameManager._Ins.factoryManager.GetObject(ObjectFactoryType.UIFactory, "Emp_Monster");
            babyGo.transform.SetParent(babyGroupTrans);
            babyGo.GetComponent<MonsterBaby>().Init(datas[i]);
        }
    }

}
