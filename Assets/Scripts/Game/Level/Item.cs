using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class Item : MonoBehaviour
{
    public GridTool m_gridTool;

    private int HP;
    private int nowHP;
    public int ID;
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
        hpSlider = GetComponentInChildren<Slider>();
        prize = PlayerManager.GetInstance().ItemInfoDict[ID].Coin;
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
        HP = PlayerManager.GetInstance().ItemInfoDict[ID].Hp;
        nowHP = HP;
        hpSlider.value = 1;
        hpSlider.gameObject.SetActive(false);
    }

    private void OnMouseDown()
    {
        // 暂时这样
        if (MapMakerTool.HasInstance())
        {
            return;
        }
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }
        GameController.GetInstance().SetFirePoint(transform);
    }

    public void TakeDamage(int damage)
    {
        if (hpSlider.gameObject.activeSelf == false)
        {
            hpSlider.gameObject.SetActive(true);
        }
        timer = 0;
        nowHP -= damage;
        hpSlider.value = (float)nowHP / HP;
        if (nowHP <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        if (GameController.GetInstance().fireTrans == transform)
        {
            GameController.GetInstance().HideFirePoint();
        }

        GameObject priseGO = FactoryManager.GetInstance().GetObject(ObjectFactoryType.GameFactory, "CoinCanvas");
        priseGO.transform.SetParent(GameController.GetInstance().transform);
        priseGO.transform.position = transform.position;
        priseGO.GetComponentInChildren<GetCoin>().ShowMoney(prize);

        GameObject desGo = FactoryManager.GetInstance().GetObject(ObjectFactoryType.GameFactory, "DestoryEff");
        desGo.transform.position = transform.position;
        FactoryManager.GetInstance().PushObject(ObjectFactoryType.GameFactory, LevelManager.GetInstance().LevelGroupId + "/Items/" + ID, gameObject);
    }
}
