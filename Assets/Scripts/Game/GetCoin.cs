using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class GetCoin : MonoBehaviour
{
    [SerializeField] string objectName;
    //[SerializeField] float aliveTime;
    Image img_Coin;
    Text txt_Coin;
    private void Awake()
    {
        img_Coin = GetComponentInChildren<Image>();
        txt_Coin = GetComponentInChildren<Text>();
    }

    public void ShowMoney(int money)
    {
        txt_Coin.text = money.ToString();
        if (money > 500)
            img_Coin.sprite = FactoryManager.GetInstance().GetSprite("NormalMordel/Game/Coin");
        else
            img_Coin.sprite = FactoryManager.GetInstance().GetSprite("NormalMordel/Game/ManyCoin");
        transform.parent.DOLocalMoveY(3, 2f / GameController.GetInstance().playSpeed);
        img_Coin.DOFade(0, 2f / GameController.GetInstance().playSpeed);
        txt_Coin.DOFade(0, 2f / GameController.GetInstance().playSpeed).OnComplete(Hide);
    }

    void Hide()
    {
        Color color = img_Coin.color;
        color.a = 1;
        img_Coin.color = color;

        color = txt_Coin.color;
        color.a = 1;
        txt_Coin.color = color;

        FactoryManager.GetInstance().PushObject(ObjectFactoryType.GameFactory, objectName, transform.parent.gameObject);
    }
}
