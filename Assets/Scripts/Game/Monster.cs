using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class Monster : MonoBehaviour
{
    GameController gameController;
    public List<Vector3> monsterPos = new List<Vector3>();
    Animator am;

    //属性
    public int id;
    public int allHp, nowHp;
    public float initSpeed, speed;
    public int nowPosIndex = 0;
    public int prize = 10;
    bool isReach = false;
    Slider hpslider;
    SpriteRenderer spriteRenderer;

    //减速有关
    ShitDeBuffProperty shitDeBuffProperty;
    bool isDesSpeed = false;
    float desTimer;
    SpriteRenderer desSpeedShitSP;

    private void Awake()
    {
        gameController = GameController._Ins;
        am = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        hpslider = GetComponentInChildren<Slider>();
        monsterPos = gameController.GetComponent<MapMaker>().GetMonsterPosVect();
        desSpeedShitSP = transform.Find("Shit").GetComponent<SpriteRenderer>();
    }

    private void OnEnable()
    {
        Init();
    }

    private void Update()
    {
        if (gameController.isStop)
            return;
        if (nowPosIndex == monsterPos.Count - 1)
        {
            isReach = true;
            Killed(isReach);
            return;
        }
        if(isDesSpeed)
        {
            if (desTimer <= 0)
                CancleDesSpeed();
            else
                desTimer -= Time.deltaTime;
        }

        if (isReach == false)
        {
            transform.position = Vector3.Lerp(transform.position, monsterPos[nowPosIndex + 1],
                1 / Vector3.Distance(transform.position, monsterPos[nowPosIndex + 1]) * Time.deltaTime * speed * gameController.playSpeed);

            if (Vector3.Distance(transform.position, monsterPos[nowPosIndex + 1]) < 0.1f)
            {
                transform.position = monsterPos[++nowPosIndex];
            }
        }
    }

    //获得怪物的其他特异性属性
    public void GetMonsterProperty()
    {
        //通过更换动画控制器来改变怪物的形态
        am.runtimeAnimatorController = GameManager._Ins.factoryManager.GetRuntimeAnimatorController("Monster/" + gameController.CurLevelGroup + "/" + id);
    }

    //每一次从对象池拿出来时做的初始化状态操作
    private void Init()
    {
        isReach = false;
        hpslider.gameObject.SetActive(false);
        nowPosIndex = 0;
        hpslider.value = 1;
        transform.position = monsterPos[0];
        isDesSpeed = false;
        desSpeedShitSP.enabled = false;
    }

    public void DesSpeed(ShitDeBuffProperty shitDeBuffProperty)
    {
        this.shitDeBuffProperty = shitDeBuffProperty;
        desTimer = shitDeBuffProperty.liveTime;
        speed = initSpeed;//多次被减速只是刷新减速持续时间 速度不再递减
        speed -= shitDeBuffProperty.desSpeed;
        isDesSpeed = true;
        desSpeedShitSP.enabled=true;
    }

    private void CancleDesSpeed()
    {
        desSpeedShitSP.enabled=false;
        desTimer = 0;
        isDesSpeed = false;
        speed = initSpeed;
    }

    public void TakeDamage(int damage)
    {
        hpslider.gameObject.SetActive(true);
        nowHp -= damage;
        if (nowHp <= 0)
        {
            nowHp = 0;
            Killed(isReach);
        }
        hpslider.value = (float)nowHp / allHp;
    }

    void Killed(bool isreach)
    {
        if (gameController.fireTrans == transform)
            gameController.HideFirePoint();
        GameObject dieEff = GameManager._Ins.factoryManager.GetObject(ObjectFactoryType.GameFactory, "DestoryEff");
        dieEff.transform.SetParent(gameController.transform);
        dieEff.transform.position = transform.position;
        gameController.KillMonster(isreach,transform.position);
        if (isreach == false)
        {
            GameObject coinGo = GameManager._Ins.factoryManager.GetObject(ObjectFactoryType.GameFactory, "CoinCanvas");
             coinGo.transform.SetParent(gameController.transform);
            coinGo.transform.localPosition = transform.localPosition;
            coinGo.GetComponentInChildren<GetCoin>().ShowMoney(prize);
        }
        GameManager._Ins.factoryManager.PushObject(ObjectFactoryType.GameFactory, "Monster", gameObject);
    }

    private void OnMouseDown()
    {
        if (EventSystem.current.IsPointerOverGameObject())
            return;
        gameController.SetFirePoint(transform);
    }

}
