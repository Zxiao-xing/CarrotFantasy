using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 处理不同的塔的特异性
/// 处理塔的升级以及出售
/// </summary>
public class TowerPersonalProperty : MonoBehaviour
{
    public int towerLevel;
    [HideInInspector]
    public Transform targetTrans;
    public int SellPrice { get; protected set; }
    public int UpPrice { get; protected set; }
    [SerializeField] int price;
    protected Tower tower;
    protected float attackTimeVal;
    [SerializeField] protected float attackCD;
    protected Animator animator;
    protected Vector3 targetPos;
    [SerializeField] bool canRotate;

    protected void Awake()
    {
        tower = GetComponent<Tower>();
        animator = transform.Find("tower").GetComponent<Animator>();
    }

    protected virtual void Start()
    {
        SellPrice = price / 2;
        UpPrice = (int)(price * 1.5f);
    }

    private void OnEnable()
    {
        Init();
    }

    protected virtual void Update()
    {
        if (GameController.GetInstance().isStop)
        {
            return;
        }
        if (canRotate)
        {
            RotateTower();
        }
        Attack();

    }

    /* 旋转炮塔对准目标*/
    protected virtual void RotateTower()
    {
        if (targetTrans != null)
        {
            targetPos = targetTrans.position;
            targetPos.z = transform.position.z;
            transform.LookAt(targetPos);
        }
        //因为LookAtAPI的原因,物体可能会旋转y轴为0度,就看不见物体了
        //手动调整y为90
        /*必须是localEulerAngles 必须是<=1,防止0.几几情况*/
        if (transform.localEulerAngles.y <= 1)
        {
            Vector3 pos = transform.localEulerAngles;
            pos.y = 90;
            transform.localEulerAngles = pos;
        }
    }

    protected virtual void Attack()
    {
        if (attackTimeVal >= attackCD / GameController.GetInstance().playSpeed)
        {
            if (targetTrans != null)
            {
                attackTimeVal = 0;
                //直接播放动画机上的指定名称动画
                animator.Play("Attack");
                GameObject bullect = FactoryManager.GetInstance().GetObject(ObjectFactoryType.GameFactory, "Tower/ID" + tower.towerID + "/Bullect/" + towerLevel);
                bullect.transform.position = transform.position;
                bullect.GetComponent<BulletBase>().SetTarget(targetTrans);
            }
        }
        else
        {
            attackTimeVal += Time.deltaTime;
        }
    }

    protected virtual void Init()
    {
        attackTimeVal = attackCD;
    }

    public void SellTower()
    {
        GameController.GetInstance().ChangeCoin(SellPrice);

        GameController.GetInstance().selectedGrid.HideGrid();
        GameController.GetInstance().selectedGrid.towerGo = null;
        GameController.GetInstance().selectedGrid.levelUPsign.SetActive(false);
        GameController.GetInstance().selectedGrid = null;
        DestroyTower();
    }

    public void UpTower()
    {
        GameController.GetInstance().ChangeCoin(-UpPrice);

        GameObject go = FactoryManager.GetInstance().GetObject(ObjectFactoryType.GameFactory, "Tower/ID" + tower.towerID + "/TowerSet/" + (towerLevel + 1));
        go.transform.SetParent(transform.parent);
        go.transform.position = transform.position;
        GameController.GetInstance().selectedGrid.towerGo = go;

        GameController.GetInstance().selectedGrid.HideGrid();
        GameController.GetInstance().selectedGrid = null;
        DestroyTower();
    }

   protected virtual void DestroyTower()
    {
        GameObject eff = FactoryManager.GetInstance().GetObject(ObjectFactoryType.GameFactory, "BuildEff");
        eff.transform.position = transform.position;
        FactoryManager.GetInstance().PushObject(ObjectFactoryType.GameFactory, "Tower/ID" + tower.towerID + "/TowerSet/" + towerLevel, gameObject);
    }

}
