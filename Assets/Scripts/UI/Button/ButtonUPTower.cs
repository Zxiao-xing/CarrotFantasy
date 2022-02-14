using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonUPTower : MonoBehaviour
{
    Button button;
    Sprite canUpLevel, cannotUpLevel, reachHighestLevel;
    Image image;
    Text priceTxt;

    private void Awake()
    {
        button = GetComponent<Button>();
        image = GetComponent<Image>();
        priceTxt = GetComponentInChildren<Text>();
    }

    private void OnEnable()
    {
        button.onClick.AddListener(TowerUP);
        if (canUpLevel == null)
            return;
        UpdateUI();
    }

    private void Start()
    {
        string path = "NormalMordel/Game/Tower/";
        canUpLevel = FactoryManager.GetInstance().GetSprite(path + "Btn_CanUpLevel");
        cannotUpLevel = FactoryManager.GetInstance().GetSprite(path + "Btn_CantUpLevel");
        reachHighestLevel = FactoryManager.GetInstance().GetSprite(path + "Btn_ReachHighestLevel");
        UpdateUI();
    }

    private void OnDisable()
    {
        button.onClick.RemoveListener(TowerUP);
    }

    void UpdateUI()
    {
        TowerPersonalProperty tp = GameController.GetInstance().selectedGrid.towerGo.GetComponent<TowerPersonalProperty>();
        priceTxt.enabled = true;
        if (tp.towerLevel >= 3)
        {
            image.sprite = reachHighestLevel;
            priceTxt.enabled = false;
            button.interactable = false;
        }

        else if (GameController.GetInstance().coins >= tp.UpPrice)
        {
            image.sprite = canUpLevel;
            priceTxt.text = tp.UpPrice.ToString();
            button.interactable = true;
        }

        else
        {
            button.interactable = false;
            priceTxt.text = tp.UpPrice.ToString();
            image.sprite = cannotUpLevel;
        }
    }

    void TowerUP()
    {
        GameController.GetInstance().selectedGrid.towerGo.GetComponent<TowerPersonalProperty>().UpTower();
    }

}
