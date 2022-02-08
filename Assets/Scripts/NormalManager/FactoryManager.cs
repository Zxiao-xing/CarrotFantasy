using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ObjectFactoryType
{
    UIFactory,
    UIPanelFactory,
    GameFactory
}
public class FactoryManager : Singleton<FactoryManager>
{
    Dictionary<ObjectFactoryType, BaseObjectFactory> m_objectFactoryDict = new Dictionary<ObjectFactoryType, BaseObjectFactory>();
    AudioClipFactory m_audioClipFactory;
    SpriteFactory m_spriteFactory;
    RunTimeAnimatorFactory m_runTimeAnimatorFactory;
    JsonFactory m_jsonFactory;

    protected override void Init()
    {
        m_objectFactoryDict[ObjectFactoryType.UIFactory] = new UIFactory();
        m_objectFactoryDict[ObjectFactoryType.GameFactory] = new GameFactory();
        m_objectFactoryDict[ObjectFactoryType.UIPanelFactory] = new UIPanelFactory();

        m_audioClipFactory = new AudioClipFactory();
        m_spriteFactory = new SpriteFactory();
        m_runTimeAnimatorFactory = new RunTimeAnimatorFactory();
        m_jsonFactory = new JsonFactory();
    }

    public GameObject GetObject(ObjectFactoryType factoryType, string itemName)
    {
        return m_objectFactoryDict[factoryType].PopObject(itemName);
    }

    public void PushObject(ObjectFactoryType factoryType, string itemName, GameObject go)
    {
        m_objectFactoryDict[factoryType].PushObject(itemName, go);
    }

    public Sprite GetSprite(string spriteName)
    {
        return m_spriteFactory.GetResource(spriteName); ;
    }

    public AudioClip GetAudioClip(string clipName)
    {
        return m_audioClipFactory.GetResource(clipName);
    }

    public RuntimeAnimatorController GetRuntimeAnimatorController(string controllerName)
    {
        return m_runTimeAnimatorFactory.GetResource(controllerName);
    }

    // 获取 json 文本
    public string GetJsonTextString(string fileName)
    {
        return m_jsonFactory.GetResource(fileName);
    }

    // 获取 json 中存储的对象
    public T GetJsonObject<T>(string fileName)
    {
        return m_jsonFactory.GetJsonObject<T>(fileName);
    }

    public void SaveJsonFile<T>(T obj, string fileName)
    {
        m_jsonFactory.SaveJsonFile(obj, fileName);
    }
}
