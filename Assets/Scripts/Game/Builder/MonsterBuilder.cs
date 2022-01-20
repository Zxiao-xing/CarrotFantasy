using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterBuilder : IBuilder<Monster>
{
    // todo：这个 id 也不用，创建怪物时用参数传 id 进来就行了
    public int monsterID;

    public void GetData(Monster monster)
    {
        monster.id = monsterID;
        // 从读出的数据拿就完事了
        MonsterInfo monsterInfo = PlayerManager.GetInstance().MonsterInfoDict[monsterID];
        monster.allHp = monsterInfo.Hp;
        monster.nowHp = monsterInfo.Hp;
        monster.speed = monsterInfo.Speed;
        monster.initSpeed = monsterInfo.Speed;
        monster.coin = monsterInfo.Coin;
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
        GameObject go = GameController.GetInstance().GetObject(ObjectFactoryType.GameFactory, "Monster");
        Monster monster = GetProductClass(go);
        GetData(monster);
        GetOtherResources(monster);
        return go;
    }

}
