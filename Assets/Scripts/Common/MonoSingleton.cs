using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MonoSingleton<T> : MonoBehaviour where T : Component
{
    private static T s_instance;

    // 保证单例物体的唯一性
    protected virtual void Awake()
    {
        if(s_instance != null && s_instance.gameObject != gameObject)
        {
            if (Application.isPlaying)
            {
                Destroy(gameObject);
            }
            else
            {
                DestroyImmediate(gameObject);
            }
        }
        else if(s_instance == null)
        {
            s_instance = GetComponent<T>();
        }

        DontDestroyOnLoad(gameObject);

        Init();
    }

    // 确保单例能随着物体摧毁
    protected virtual void OnDestroy()
    {
        if(s_instance != null && s_instance.gameObject == gameObject)
        {
            s_instance = null;
        }
    }

    // 获取单例
    public static T GetInstance()
    {
        if(s_instance == null)
        {
            // 先从场景中找
            Type type = typeof(T);
            s_instance = (T)FindObjectOfType(type);

            // 没找到再创建
            if(s_instance == null)
            {
                GameObject go = new GameObject(type.Name);
                s_instance = go.AddComponent<T>();
            }
        }
        return s_instance;
    }

    public static void DestoryInstance()
    {
        if(s_instance != null)
        {
            Destroy(s_instance.gameObject);
        }
    }

    // 判断是否存在单例
    public bool HasInstance()
    {
        return (s_instance != null);
    }

    // 单例创建时需要进行初始化时重写
    protected virtual void Init()
    {

    }
}
