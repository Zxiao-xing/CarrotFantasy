using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletWindMill : BulletBase
{
   [SerializeField] float aliveTime;
    float timer;
    protected override void Update()
    {
        if (GameController.GetInstance().isStop)
            return;
        if(timer>=aliveTime)
        {
            timer = 0;
            GameController.GetInstance().PushObject(ObjectFactoryType.GameFactory, "Tower/ID" + towerID + "/Bullect/" + towerLevel, gameObject);
            return;
        }
        timer += Time.deltaTime;
        transform.Translate(Vector3.forward * speed
            * Time.deltaTime * GameController.GetInstance().playSpeed);
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag.Equals("Monster") || collision.tag.Equals("Item"))
        {
            //再次判断目标是否已经被击杀 防止2颗子弹同时打中敌人的message调用Bug
            if (collision.gameObject.activeSelf == true)
                collision.SendMessage("TakeDamage", damage);
            GameObject effGO = GameController.GetInstance().GetObject(ObjectFactoryType.GameFactory, "Tower/ID" + towerID + "/Effect/" + towerLevel);
            effGO.transform.position = transform.position;           
        }
    }
}
