using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class Monster : MonoBehaviour
{
    public List<Vector3> monsterPos = new List<Vector3>();
    Animator m_animator;

    //属性
    public int id;
    public int allHp, nowHp;
    public float initSpeed, speed;
    public int nowPosIndex = 0;
    public int coin = 10;
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
        m_animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        hpslider = GetComponentInChildren<Slider>();
        monsterPos = MapMaker.GetInstance().GetMonsterPosVect();
        desSpeedShitSP = transform.Find("Shit").GetComponent<SpriteRenderer>();
    }

    private void OnEnable()
    {
        Init();
    }

    private void Update()
    {
        if (GameController.GetInstance().isStop)
            return;
        if (nowPosIndex == monsterPos.Count - 1)
        {
            isReach = true;
            Killed(isReach);
            return;
        }
        if (isDesSpeed)
        {
            if (desTimer <= 0)
                CancleDesSpeed();
            else
                desTimer -= Time.deltaTime;
        }

        if (isReach == false)
        {
            transform.position = Vector3.Lerp(transform.position, monsterPos[nowPosIndex + 1],
                1 / Vector3.Distance(transform.position, monsterPos[nowPosIndex + 1]) * Time.deltaTime * speed * GameController.GetInstance().playSpeed);

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
        m_animator.runtimeAnimatorController = FactoryManager.GetInstance().GetRuntimeAnimatorController("Monster/" + GameController.GetInstance().CurLevelGroup + "/" + PlayerManager.GetInstance().MonsterInfoDict[id].AnimatorName);
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
        desSpeedShitSP.enabled = true;
    }

    private void CancleDesSpeed()
    {
        desSpeedShitSP.enabled = false;
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
        if (GameController.GetInstance().fireTrans == transform)
            GameController.GetInstance().HideFirePoint();
        GameObject dieEff = FactoryManager.GetInstance().GetObject(ObjectFactoryType.GameFactory, "DestoryEff");
        dieEff.transform.SetParent(GameController.GetInstance().transform);
        dieEff.transform.position = transform.position;
        GameController.GetInstance().KillMonster(isreach, transform.position);
        if (isreach == false)
        {
            GameObject coinGo = FactoryManager.GetInstance().GetObject(ObjectFactoryType.GameFactory, "CoinCanvas");
            coinGo.transform.SetParent(GameController.GetInstance().transform);
            coinGo.transform.localPosition = transform.localPosition;
            coinGo.GetComponentInChildren<GetCoin>().ShowMoney(coin);
        }
        FactoryManager.GetInstance().PushObject(ObjectFactoryType.GameFactory, "Monster", gameObject);
    }

    private void OnMouseDown()
    {
        if (EventSystem.current.IsPointerOverGameObject())
            return;
        GameController.GetInstance().SetFirePoint(transform);
    }

}
