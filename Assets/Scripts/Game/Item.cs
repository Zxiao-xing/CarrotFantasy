using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class Item : MonoBehaviour
{
    private int HP;
    private int nowHP;
    public int ID;
    GameController gameController;
    private Slider hpSlider;
    float timeVal = 3;
    float timer = 0;
    int prize;

    private void OnEnable()
    {
        if (prize != 0)
            Init();
    }

    private void Start()
    {
        gameController = GameController._Ins;
        hpSlider = GetComponentInChildren<Slider>();
        prize = ID * 100;
        Init();
    }

    private void Update()
    {
        if (hpSlider.gameObject.activeSelf)
        {
            timer += Time.deltaTime;
            if (timer >= timeVal)
            {
                hpSlider.gameObject.SetActive(false);
                timer = 0;
            }
        }
    }

    void Init()
    {
        HP = 1000 - 100 * ID;
        nowHP = HP;
        hpSlider.value = 1;
        hpSlider.gameObject.SetActive(false);
    }

    private void OnMouseDown()
    {
        if (EventSystem.current.IsPointerOverGameObject())
            return;
        GameController._Ins.SetFirePoint(transform);
    }

    public void TakeDamage(int damage)
    {
        if (hpSlider.gameObject.activeSelf == false)
            hpSlider.gameObject.SetActive(true);
        timer = 0;
        nowHP -= damage;
        hpSlider.value = (float)nowHP / HP;
        if (nowHP <= 0)
            Die();
    }

    void Die()
    {
        if (gameController.fireTrans == transform)
            gameController.HideFirePoint();
        GameObject priseGO = GameManager._Ins.factoryManager.GetObject(ObjectFactoryType.GameFactory, "CoinCanvas");
        priseGO.transform.SetParent(gameController.transform);
        priseGO.transform.position = transform.position;
        priseGO.GetComponentInChildren<GetCoin>().ShowMoney(prize);

        GameObject desGo = GameManager._Ins.factoryManager.GetObject(ObjectFactoryType.GameFactory, "DestoryEff");
        desGo.transform.position = transform.position;
        GameManager._Ins.factoryManager.PushObject(ObjectFactoryType.GameFactory, gameController.CurLevelGroup + "/Items/" + ID, gameObject);
    }
}
