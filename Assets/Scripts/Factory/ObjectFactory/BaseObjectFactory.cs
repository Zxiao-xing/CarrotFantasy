using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*这个工厂是用于实例化具体物体的,如UI面板,怪物等可以放在游戏场景中的东西*/
public class BaseObjectFactory : IObjectFactory
{
    //  protected Dictionary<string, GameObject> prefabDict = new Dictionary<string, GameObject>();
    protected Dictionary<string, Stack<GameObject>> poolDict = new Dictionary<string, Stack<GameObject>>();
    protected string path = "Prefabs/";

    public virtual GameObject PopObject(string name)
    {
        GameObject res;
        if (poolDict.ContainsKey(name) == false)
        {
            poolDict[name] = new Stack<GameObject>();
        }
        //处理场景跳转后对象池中有null物体的引用
        for (int i = 0; i < poolDict[name].Count; i++)
        {
            if (poolDict[name].Peek() == null)
            {
                poolDict[name].Pop();
                i--;
            }
            else
                break;
        }
        if (poolDict[name].Count <= 0)
            res = GameObject.Instantiate(GetResource(name));
        else
        {
            res = poolDict[name].Pop();
            res.SetActive(true);
        }
        return res;
    }
    /*对象池里面的东西都是先从里面取,再放进去.如果取的时候没有对应的对象池就创建*/
    /*如果放的时候还是没有对应对象池,则一定是代码出错了,而不应该创建此对象池*/
    public void PushObject(string name, GameObject go)
    {
        if (poolDict.ContainsKey(name) == false)
        {
            Debug.LogWarning("对象池不存在: " + name);
            return;
        }

        go.SetActive(false);
        poolDict[name].Push(go);
    }

    protected GameObject GetResource(string name)
    {
        GameObject go = Resources.Load<GameObject>(path + name);
        if (go == null)
            Debug.LogWarning("资源的路径错误 :" + path + name);
        return go;
    }
}
