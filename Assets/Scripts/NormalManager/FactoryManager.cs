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
    Dictionary<ObjectFactoryType, BaseObjectFactory> objectFactoryDict = new Dictionary<ObjectFactoryType, BaseObjectFactory>();
    AudioClipFactory audioClipFactory;
    SpriteFactory spriteFactory;
    RunTimeAnimatorFactory runTimeAnimatorFactory;
    JsonFactory m_jsonFactory;

    protected override void Init()
    {
        objectFactoryDict[ObjectFactoryType.UIFactory] = new UIFactory();
        objectFactoryDict[ObjectFactoryType.GameFactory] = new GameFactory();
        objectFactoryDict[ObjectFactoryType.UIPanelFactory] = new UIPanelFactory();

        audioClipFactory = new AudioClipFactory();
        spriteFactory = new SpriteFactory();
        runTimeAnimatorFactory = new RunTimeAnimatorFactory();
        m_jsonFactory = new JsonFactory();
    }

    public GameObject GetObject(ObjectFactoryType factoryType, string itemName)
    {
        return objectFactoryDict[factoryType].PopObject(itemName);
    }

    public void PushObject(ObjectFactoryType factoryType, string itemName, GameObject go)
    {
        objectFactoryDict[factoryType].PushObject(itemName, go);
    }

    public Sprite GetSprite(string spriteName)
    {
        return spriteFactory.GetResource(spriteName); ;
    }

    public AudioClip GetAudioClip(string clipName)
    {
        return audioClipFactory.GetResource(clipName);
    }

    public RuntimeAnimatorController GetRuntimeAnimatorController(string controllerName)
    {
        return runTimeAnimatorFactory.GetResource(controllerName);
    }

    public string GetJsonTextString(string fileName)
    {
        return m_jsonFactory.GetResource(fileName);
    }
}
