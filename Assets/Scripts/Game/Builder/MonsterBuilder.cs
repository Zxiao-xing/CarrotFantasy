using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterBuilder : IBuilder<Monster>
{
    public int monsterID;

    public void GetData(Monster productClass)
    {
        productClass.id = monsterID;
        productClass.allHp = monsterID * 100;
        productClass.nowHp = productClass.allHp;
        productClass.speed = monsterID;
        productClass.initSpeed = monsterID;
        productClass.prize = monsterID * 50;
    }

    public void GetOtherResources(Monster product)
    {
        product.GetMonsterProperty();
    }

    public Monster GetProductClass(GameObject go)
    {
        return go.GetComponent<Monster>();
    }

    public GameObject GetProducts()
    {
        GameObject go= GameController._Ins.GetObject(ObjectFactoryType.GameFactory, "Monster");
        Monster monster = GetProductClass(go);
        GetData(monster);
        GetOtherResources(monster);
        return go;
    }

}
