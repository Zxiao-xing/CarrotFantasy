using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class CrystalTowerProperty : TowerPersonalProperty
{
    GameObject bulletGO;
    float bulletLength, bulletWidth, distance;
    Vector3 tempPos;

    private void OnEnable()
    {
        bulletGO = FactoryManager.GetInstance().GetObject(ObjectFactoryType.GameFactory, "Tower/ID" + tower.towerID + "/Bullect/" + towerLevel);
        bulletGO.SetActive(false);
    }

    protected override void Start()
    {
        base.Start();
        bulletGO = FactoryManager.GetInstance().GetObject(ObjectFactoryType.GameFactory, "Tower/ID" + tower.towerID + "/Bullect/" + towerLevel);
        bulletGO.SetActive(false);
    }

    protected override void Update()
    {
        if (GameController.GetInstance().isStop)
            return;
        if (targetTrans == null || targetTrans.gameObject.activeSelf == false)
        {
            if (bulletGO.activeSelf)
                bulletGO.SetActive(false);
        }
        else
        {
            if (bulletGO.activeSelf==false)
                bulletGO.SetActive(true);
            SetFlashData();
            if (attackTimeVal >= attackCD/ GameController.GetInstance().playSpeed)
            {
                animator.Play("Attack");
                bulletGO.GetComponent<BulletCrystal>().TakeDamage(targetTrans);
                attackTimeVal = 0;
            }
            else
                attackTimeVal += Time.deltaTime;
        }
    }

    /// <summary>
    /// 设置发射出来的闪电的长度 宽度与位置
    /// </summary>
    void SetFlashData()
    {
        tempPos = targetTrans.position;
        tempPos.z = transform.position.z;
        distance = Vector3.Distance(transform.position, tempPos);
        bulletWidth = 3 / distance;
        bulletWidth = Mathf.Clamp(bulletWidth, 0.5f, 1f);
        bulletLength = distance / 2;
        bulletGO.transform.position = new Vector3((tempPos.x + transform.position.x) / 2, (tempPos.y + transform.position.y) / 2, transform.position.z);
        bulletGO.transform.localScale = new Vector3(1, bulletWidth, bulletLength);
        bulletGO.transform.LookAt(tempPos);
    }

    protected override void DestroyTower()
    {
        FactoryManager.GetInstance().PushObject(ObjectFactoryType.GameFactory, "Tower/ID" + tower.towerID + "/Bullect/" + towerLevel, bulletGO); 
        base.DestroyTower();
    }
}
