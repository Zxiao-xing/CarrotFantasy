using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletBase : MonoBehaviour
{
    protected Transform targetTrans;
    [SerializeField] protected float speed;
    [SerializeField] protected int damage;
    //用于对象池回收子弹时寻找路径
    [SerializeField] protected int towerID, towerLevel;

    protected virtual void Update()
    {
        if (!targetTrans.gameObject.activeSelf || GameController.GetInstance().isStop)
        {
            GameController.GetInstance().PushObject(ObjectFactoryType.GameFactory, "Tower/ID" + towerID + "/Bullect/" + towerLevel, gameObject);
            return;
        }
        if (targetTrans.gameObject.activeSelf)
        {
            LookAtTarget();
            transform.position = Vector3.MoveTowards(transform.position, targetTrans.position, speed
                * Time.deltaTime * GameController.GetInstance().playSpeed);
        }
    }

    public void SetTarget(Transform target)
    {
        targetTrans = target;
        //目标的左边Z值与子弹可能不同,为了碰撞到一起需要改变子弹的Z
        Vector3 pos = transform.position;
        pos.z = target.position.z;
        transform.position = pos;
        LookAtTarget();
    }

    protected void LookAtTarget()
    {
        transform.LookAt(targetTrans);
        //同炮塔旋转原理一致
        if (transform.localEulerAngles.y <= 1)
        {
            Vector3 rotate = transform.localEulerAngles;
            rotate.y = 90;
            transform.localEulerAngles = rotate;
        }
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag.Equals("Monster") || collision.tag.Equals("Item"))
        {
            //再次判断目标是否已经被击杀 防止2颗子弹同时打中敌人的message调用Bug
            if (collision.gameObject.activeSelf == true)
                collision.SendMessage("TakeDamage", damage);
            GameObject effGO = GameController.GetInstance().GetObject(ObjectFactoryType.GameFactory, "Tower/ID" + towerID + "/Effect/" + towerLevel);
            effGO.transform.position = transform.position;
            GameController.GetInstance().PushObject(ObjectFactoryType.GameFactory, "Tower/ID" + towerID + "/Bullect/" + towerLevel, gameObject);
        }
    }

}
