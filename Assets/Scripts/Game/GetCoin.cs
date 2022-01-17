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
            img_Coin.sprite = GameManager._Ins.factoryManager.GetSprite("NormalMordel/Game/Coin");
        else
            img_Coin.sprite = GameManager._Ins.factoryManager.GetSprite("NormalMordel/Game/ManyCoin");
        transform.parent.DOLocalMoveY(3, 2f / GameController._Ins.playSpeed);
        img_Coin.DOFade(0, 2f / GameController._Ins.playSpeed);
        txt_Coin.DOFade(0, 2f / GameController._Ins.playSpeed).OnComplete(Hide);
    }

    void Hide()
    {
        Color color = img_Coin.color;
        color.a = 1;
        img_Coin.color = color;

        color = txt_Coin.color;
        color.a = 1;
        txt_Coin.color = color;

        GameManager._Ins.factoryManager.PushObject(ObjectFactoryType.GameFactory, objectName, transform.parent.gameObject);
    }
}
