using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 单例类，不走 mono 的事件回调，该单例没有禁用掉构造函数，所以劝你耗子尾汁，不要乱 new
public class Singleton<T> where T : class, new()
{
    private static T s_instance;

    // 获取单例
    public T GetInstance()
    {
        if(s_instance == null)
        {
            CreateInstance();
        }
        return s_instance;
    }

    // 可以更自由的主动创建单例，如创建代价较大可以提前创建等
    public void CreateInstance()
    {
        s_instance = new T();
        (s_instance as Singleton<T>).Init();
    }

    // 销毁单例
    public void DestoryInstance()
    {
        if(s_instance != null)
        {
            (s_instance as Singleton<T>).Uninit();
            s_instance = null;
        }
    }

    // 是否拥有单例
    public bool HasInstance()
    {
        return (s_instance != null);
    }

    // 创建单例时如需进行初始化工作则重写
    protected virtual void Init()
    {

    }

    // 销毁单例时如需进行善后工作则重写
    protected virtual void Uninit()
    {

    }
}
