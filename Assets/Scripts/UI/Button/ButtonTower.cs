using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 建塔按钮的控制脚本,根据现有金钱切换按钮图标；
/// 处理点击按钮后建塔逻辑
/// </summary>
public class ButtonTower : MonoBehaviour
{
    Button button;
    Image image;
    int towerPrice;
    public int towerID;
    string spritePath = "NormalMordel/Game/Tower/";

    private void OnEnable()
    {
        //防止第一次Enable执行时,由于Start还没执行
        //在Start里的各种引用都没拿到而出错,
        if (towerPrice == 0)
            return;
        button.onClick.AddListener(CreateTower);
        UpdateIcon();
    }

    void Start()
    {
        button = GetComponent<Button>();
        image = GetComponent<Image>();
        towerPrice = GameController.GetInstance().towerPrice[towerID - 1];
        spritePath += towerID + "/";
        UpdateIcon();
        button.onClick.AddListener(CreateTower);
    }

    private void OnDisable()
    {
        button.onClick.RemoveListener(CreateTower);
    }

    void UpdateIcon()
    {
        if (GameController.GetInstance().coins >= towerPrice)
        {
            image.sprite = FactoryManager.GetInstance().GetSprite(spritePath + "CanClick1");
            button.interactable = true;
        }
        else
        {
            image.sprite = FactoryManager.GetInstance().GetSprite(spritePath + "CanClick0");
            button.interactable = false;
        }
    }

    void CreateTower()
    {
        //建塔
        GameObject tower = FactoryManager.GetInstance().GetObject(ObjectFactoryType.GameFactory, "Tower/"+"ID"+towerID+ "/TowerSet/1");
        tower.transform.SetParent(GameController.GetInstance().selectedGrid.transform);
        tower.transform.position = GameController.GetInstance().selectedGrid.transform.position+Vector3.forward;
        tower.transform.localEulerAngles = new Vector3(-90, 90, 0);
        //加特效
        GameObject eff = FactoryManager.GetInstance().GetObject(ObjectFactoryType.GameFactory, "BuildEff");
        eff.transform.SetParent(GameController.GetInstance().selectedGrid.transform);
        eff.transform.position = GameController.GetInstance().selectedGrid.transform.position;
        //处理格子
        GameController.GetInstance().selectedGrid.towerGo = tower;
        GameController.GetInstance().selectedGrid.HideGrid();

        GameController.GetInstance().selectedGrid = null;
        //其他设置
        GameController.GetInstance().towerList.SetActive(false);
        GameController.GetInstance().ChangeCoin(-towerPrice);
    }
}
