using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MonsterBaby : MonoBehaviour
{
    GameObject eggGo, normalGo, babyGo;
    MonsterPetData petData;
    [SerializeField] GameObject feedHeartGo;

    private void Awake()
    {
        eggGo = transform.Find("Emp_Egg").gameObject;
        normalGo = transform.Find("Emp_Normal").gameObject;
        babyGo = transform.Find("Emp_Baby").gameObject;
    }

    public void Init(MonsterPetData monsterPetData)
    {
        this.petData = monsterPetData;

        string path = "MonsterNest/Monster";
        eggGo.transform.Find("Img_Egg").GetComponent<Image>().sprite =
            FactoryManager.GetInstance().GetSprite(path + "/Egg/" + petData.monsterID);
        babyGo.transform.Find("Img_Baby").GetComponent<Image>().sprite =
            FactoryManager.GetInstance().GetSprite(path + "/Baby/" + petData.monsterID);
        normalGo.transform.Find("Img_Normal").GetComponent<Image>().sprite =
            FactoryManager.GetInstance().GetSprite(path + "/Normal/" + petData.monsterID);

        eggGo.SetActive(false);
        babyGo.SetActive(false);
        normalGo.SetActive(false);
        switch (petData.monsterLevel)
        {
            case 0:
                eggGo.SetActive(true);
                break;
            case 1:
                babyGo.SetActive(true);
                break;
            case 2:
                normalGo.SetActive(true);
                break;
            default:
                break;
        }
    }

    public void MonsterClicked()
    {
        switch (petData.monsterLevel)
        {
            case 0:
                ClickEgg();
                break;
            case 1:
                ClickBaby();
                break;
            case 2:
                ClickNormal();
                break;
            default:
                break;
        }
    }

    void ClickEgg()
    {
        if (PlayerManager.GetInstance().GetPlayerInfo().nest >= 1)
        {
            PlayerManager.GetInstance().GetPlayerInfo().nest--;
            petData.monsterLevel++;
            eggGo.SetActive(false);
            babyGo.SetActive(true);
            SendMessageUpwards("UpdateText");
            return;
        }
        GameObject go = eggGo.transform.Find("Img_Egg").Find("Img_Instruction").gameObject;
        go.SetActive(!go.activeSelf);
    }

    void ClickBaby()
    {
        GameObject go = babyGo.transform.Find("Emp_Btns").gameObject;
        go.SetActive(!go.activeSelf);
        if (go.activeSelf)
        {
            go.transform.Find("Btn_Milk").GetComponent<Button>().interactable = PlayerManager.GetInstance().GetPlayerInfo().milk > 0 && petData.remainMilk > 0;
            go.transform.Find("Btn_Cookies").GetComponent<Button>().interactable = PlayerManager.GetInstance().GetPlayerInfo().cookies > 0 && petData.remainCookies > 0;
        }
    }

    void ClickNormal()
    {
        GameObject goLeft = normalGo.transform.Find("Img_TalkLineRight").gameObject;
        if (goLeft.activeSelf)
        {
            goLeft.SetActive(false);
            return;
        }
        GameObject goRight = normalGo.transform.Find("Img_TalkLineLeft").gameObject;
        if (goRight.activeSelf)
        {
            goRight.SetActive(false);
            return;
        }
        if (Random.Range(1, 3) == 1)
            goLeft.SetActive(true);
        else
            goRight.SetActive(true);
    }

    public void FeedMilk()
    {
        feedHeartGo.SetActive(true);
        Invoke("HideHeart", 0.5f);
        if (PlayerManager.GetInstance().GetPlayerInfo().milk >= petData.remainMilk)
        {
            PlayerManager.GetInstance().GetPlayerInfo().milk -= petData.remainMilk;
            petData.remainMilk = 0;
            babyGo.transform.Find("Emp_Btns").Find("Btn_Milk").GetComponent<Button>().interactable = false;
            if (petData.remainCookies == 0)
            {
                LevelUp();
            }
        }
        else
        {
            petData.remainMilk -= PlayerManager.GetInstance().GetPlayerInfo().milk;
            PlayerManager.GetInstance().GetPlayerInfo().milk = 0;
        }
        SendMessageUpwards("UpdateText");
    }

    public void FeedCookies()
    {
        feedHeartGo.SetActive(true);
        Invoke("HideHeart", 0.5f);
        if (PlayerManager.GetInstance().GetPlayerInfo().cookies >= petData.remainCookies)
        {
            PlayerManager.GetInstance().GetPlayerInfo().cookies -= petData.remainCookies;
            petData.remainCookies = 0;
            babyGo.transform.Find("Emp_Btns").Find("Btn_Cookies").GetComponent<Button>().interactable = false;
            if (petData.remainMilk == 0)
                LevelUp();
        }
        else
        {
            petData.remainMilk -= PlayerManager.GetInstance().GetPlayerInfo().milk;
            PlayerManager.GetInstance().GetPlayerInfo().milk = 0;
        }
        SendMessageUpwards("UpdateText");
    }

    void LevelUp()
    {
        petData.monsterLevel++;
        for (int i = 0; i < PlayerManager.GetInstance().GetPlayerInfo().monsterPetDatasList.Count; i++)
        {
            if (PlayerManager.GetInstance().GetPlayerInfo().monsterPetDatasList[i].monsterID == petData.monsterID)
            {
                PlayerManager.GetInstance().GetPlayerInfo().monsterPetDatasList[i] = petData;
                break;
            }
        }
        babyGo.SetActive(false);
        normalGo.SetActive(true);
        //playerManager.GetPlayerInfo().levelUnLocked[petData.monsterID - 1]++;
        //数组下标从0开始的 所以-1
        //playerManager.GetPlayerInfo().allSmallLevels[petData.monsterID * 5 - 1].unLocked = true;
    }

    void HideHeart()
    {
        feedHeartGo.SetActive(false);
    }

}

//怪物宠物的数据
public struct MonsterPetData
{
    public int monsterID, monsterLevel,//等级从0开始
      remainCookies, remainMilk;
}
