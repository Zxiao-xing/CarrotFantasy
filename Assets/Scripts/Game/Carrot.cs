using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class Carrot : MonoBehaviour
{
    int totalHP;
    public int nowHP { get; private set; }
    [SerializeField] Text hpTxt;
    Animator am;
    Sprite[] sprite;
    SpriteRenderer spriteRenderer;
    float timeVal = 3;
    float timer = 3;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        am = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        Init();
    }

    private void Start()
    {
        sprite = new Sprite[7];
        for (int i = 0; i < 7; i++)
        {
            sprite[i] = GameManager._Ins.factoryManager.GetSprite("NormalMordel/Game/Carrot/" + i);
        }
    }

    private void Update()
    {
        if (nowHP > 7)
        {
            if (timer == timeVal)
            {
                timer = 0;
                am.Play("CarrotIdle");
            }
            else
                timer += Time.deltaTime;
        }
    }

    private void OnMouseDown()
    {
        if (EventSystem.current.IsPointerOverGameObject())
            return;
        am.Play("CarrotDance");
    }

    void Init()
    {
        totalHP = 10;
        nowHP = totalHP;
        hpTxt.text = nowHP.ToString();
    }

    public void SubtractHP()
    {
        nowHP--;
        hpTxt.text = nowHP.ToString();
        if (nowHP <= 0)
            GameController._Ins.GameOver(false);
        else
        {
            if (nowHP <= 7)
            {
                spriteRenderer.sprite = sprite[nowHP - 1];
                //动画组件会阻止sprite的改变
                if (am.enabled)
                    am.enabled = false;
            }
        }
    }

}
