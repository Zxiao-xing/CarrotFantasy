using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonSellTower : MonoBehaviour
{
    Button button;
    Text priceTxt;

    private void Awake()
    {
        button = GetComponent<Button>();
        priceTxt = GetComponentInChildren<Text>();
    }

    private void OnEnable()
    {
        button.onClick.AddListener(SellTower);
        priceTxt.text = GameController.GetInstance().selectedGrid.towerGo.
            GetComponent<TowerPersonalProperty>().SellPrice.ToString();
    }

    private void OnDisable()
    {
        button.onClick.RemoveListener(SellTower);
    }

    void SellTower()
    {
        GameController.GetInstance().selectedGrid.towerGo.
            GetComponent<TowerPersonalProperty>().SellTower();
    }
}
