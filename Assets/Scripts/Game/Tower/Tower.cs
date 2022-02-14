using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour
{
    [HideInInspector]
     public SpriteRenderer attackRender;
     public int towerID;
    CircleCollider2D circleCollider2D;
    TowerPersonalProperty towerPersonalProperty;
    //有没有集火目标 有没有目标
    bool isFireTarget, hasTarget;
      
    private void Awake()
    {
        attackRender = transform.Find("attackRange").GetComponent<SpriteRenderer>();
        circleCollider2D = GetComponent<CircleCollider2D>();
        towerPersonalProperty = GetComponent<TowerPersonalProperty>();
    }

    private void OnEnable()
    {
        Init();
    }

    private void Start()
    {
    }

    private void Update()
    {
        //更新集火目标发生的变化
        if (isFireTarget & towerPersonalProperty.targetTrans != GameController.GetInstance().fireTrans)
        {
            isFireTarget = false;
            hasTarget = false;
            towerPersonalProperty.targetTrans = null;
        }
        //更新目标的变化
        if (hasTarget && towerPersonalProperty.targetTrans.gameObject.activeSelf == false)
        {
            isFireTarget = false;
            hasTarget = false;
            towerPersonalProperty.targetTrans = null;
        }
    }

    #region 搜素怪物逻辑
    /* 一.怪物进入（Enter）
        1.有集火目标 但现在没找到集火目标
             1.找到的是集火目标
                 锁定当前第一个进入检测区域的物品为攻击目标,有目标,是集火目标
             2.找到的不是集火目标
                没有目标
                  锁定当前第一个进入检测区域的怪物为攻击目标,有目标
        2.没有集火目标
            没有目标
                锁定当前第一个进入检测区域的怪物为攻击目标,有目标
         如果现在的攻击目标是物品并且此物品不是集火目标,但是碰撞检测到
         一只怪物,就把攻击目标设为进入的怪物
         */
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.tag.Equals("Monster") && !collision.tag.Equals("Item"))
            return;
        if (GameController.GetInstance().fireTrans != null && isFireTarget == false)
        {
            if (collision.transform == GameController.GetInstance().fireTrans)
            {
                isFireTarget = true;
                hasTarget = true;
                towerPersonalProperty.targetTrans = collision.transform;
            }
            else
            {
                if (hasTarget == false)
                {
                    hasTarget = true;
                    towerPersonalProperty.targetTrans = collision.transform;
                }
            }
        }
        else if (GameController.GetInstance().fireTrans == null)
        {
            if (hasTarget == false)
            {
                hasTarget = true;
                towerPersonalProperty.targetTrans = collision.transform;
            }
        }
        if (towerPersonalProperty.targetTrans != GameController.GetInstance().fireTrans && towerPersonalProperty.targetTrans.
    tag.Equals("Item") && collision.tag.Equals("Monster"))
        {
            towerPersonalProperty.targetTrans = collision.transform;
        }
    }

    /*二.怪物滞留（Stay）
           同上*/
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!collision.tag.Equals("Monster") && !collision.tag.Equals("Item"))
        {
            return;
        }
        if (GameController.GetInstance().fireTrans != null && isFireTarget == false)
        {
            if (collision.transform == GameController.GetInstance().fireTrans)
            {
                isFireTarget = true;
                hasTarget = true;
                towerPersonalProperty.targetTrans = collision.transform;
            }
            else
            {
                if (hasTarget == false)
                {
                    hasTarget = true;
                    towerPersonalProperty.targetTrans = collision.transform;
                }
            }
        }
        else if (GameController.GetInstance().fireTrans == null)
        {
            if (hasTarget == false)
            {
                hasTarget = true;
                towerPersonalProperty.targetTrans = collision.transform;
            }
        }
        if (towerPersonalProperty.targetTrans != GameController.GetInstance().fireTrans && towerPersonalProperty.targetTrans.
         tag.Equals("Item") && collision.tag.Equals("Monster"))
        {
            towerPersonalProperty.targetTrans = collision.transform;
        }
    }

    /*三.怪物离开（Exit）
    如果当前检测区域的目标正是攻击的目标的时候,那么丢失目标，
    丢失集火目标,攻击目标为空*/
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.tag.Equals("Monster") && !collision.tag.Equals("Item"))
            return;
        if (collision.transform == towerPersonalProperty.targetTrans)
        {
            towerPersonalProperty.targetTrans = null;
            isFireTarget = false;
            hasTarget = false;
        }
    }
    #endregion

    /*可能会被放进对象池中,所以在拿出来使用时初始化数据*/
    private void Init()
    {
        attackRender.transform.localScale = Vector3.one * towerPersonalProperty.towerLevel;
        attackRender.enabled = false;
        circleCollider2D.radius = 1.1f * towerPersonalProperty.towerLevel;

        hasTarget = false;
        isFireTarget = false;
    }

}
