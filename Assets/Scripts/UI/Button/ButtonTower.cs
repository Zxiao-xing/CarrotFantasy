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
    GameController gameController;
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
        gameController = GameController._Ins; ;
        button = GetComponent<Button>();
        image = GetComponent<Image>();
        towerPrice = gameController.towerPrice[towerID - 1];
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
        if (gameController.coins >= towerPrice)
        {
            image.sprite = gameController.GetSprite(spritePath + "CanClick1");
            button.interactable = true;
        }
        else
        {
            image.sprite = gameController.GetSprite(spritePath + "CanClick0");
            button.interactable = false;
        }
    }

    void CreateTower()
    {
        //建塔
        GameObject tower = gameController.GetObject(ObjectFactoryType.GameFactory, "Tower/"+"ID"+towerID+ "/TowerSet/1");
        tower.transform.SetParent(gameController.selectedGrid.transform);
        tower.transform.position = gameController.selectedGrid.transform.position+Vector3.forward;
        tower.transform.localEulerAngles = new Vector3(-90, 90, 0);
        //加特效
        GameObject eff = gameController.GetObject(ObjectFactoryType.GameFactory, "BuildEff");
        eff.transform.SetParent(gameController.selectedGrid.transform);
        eff.transform.position = gameController.selectedGrid.transform.position;
        //处理格子
        gameController.selectedGrid.towerGo = tower;
        gameController.selectedGrid.HideGrid();
        
        gameController.selectedGrid = null;
        //其他设置
        gameController.towerList.SetActive(false);
        gameController.ChangeCoin(-towerPrice);
    }
}
