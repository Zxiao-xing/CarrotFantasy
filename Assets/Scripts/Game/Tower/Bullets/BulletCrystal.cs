using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 因为水晶闪电子弹只负责对目标造成伤害,BulletBase里
/// 很多东西都不需要,所以不继承它
/// </summary>
public class BulletCrystal : MonoBehaviour
{     [SerializeField] protected int damage;
    //生成特效使用
    [SerializeField] protected int towerID, towerLevel;

    public void TakeDamage(Transform targetTrans)
    {
        if (targetTrans != null && targetTrans.gameObject.activeSelf)
        {
            targetTrans.SendMessage("TakeDamage", damage);
            GameObject effectGO = GameController.GetInstance().GetObject(ObjectFactoryType.GameFactory, "Tower/ID" + towerID + "/Effect/" + towerLevel);
            effectGO.transform.position = targetTrans.position;
        }
    }
}
