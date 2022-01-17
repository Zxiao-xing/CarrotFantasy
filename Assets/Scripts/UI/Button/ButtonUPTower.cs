using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonUPTower : MonoBehaviour
{
    Button button;
    Sprite canUpLevel, cannotUpLevel, reachHighestLevel;
    GameController gameController;
    Image image;
    Text priceTxt;

    private void Awake()
    {
        gameController = GameController._Ins;
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
        canUpLevel = gameController.GetSprite(path + "Btn_CanUpLevel");
        cannotUpLevel = gameController.GetSprite(path + "Btn_CantUpLevel");
        reachHighestLevel = gameController.GetSprite(path + "Btn_ReachHighestLevel");
        UpdateUI();
    }

    private void OnDisable()
    {
        button.onClick.RemoveListener(TowerUP);
    }

    void UpdateUI()
    {
        TowerPersonalProperty tp = gameController.selectedGrid.towerGo.GetComponent<TowerPersonalProperty>();
        priceTxt.enabled = true;
        if (tp.towerLevel >= 3)
        {
            image.sprite = reachHighestLevel;
            priceTxt.enabled = false;
            button.interactable = false;
        }

        else if (gameController.coins >= tp.upPrice)
        {
            image.sprite = canUpLevel;
            priceTxt.text = tp.upPrice.ToString();
            button.interactable = true;
        }

        else
        {
            button.interactable = false;
            priceTxt.text = tp.upPrice.ToString();
            image.sprite = cannotUpLevel;
        }
    }

    void TowerUP()
    {
        gameController.selectedGrid.towerGo.GetComponent<TowerPersonalProperty>().UPTower();
    }

}
