using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyEff:MonoBehaviour
{
    [SerializeField] float liveTime;
    [SerializeField] string objectname;

    private void OnEnable()
    {
        Invoke("DestroyByTime", liveTime/GameController._Ins.playSpeed);
    }

    void DestroyByTime()
    {
        GameManager._Ins.factoryManager.PushObject(ObjectFactoryType.GameFactory, objectname, gameObject);
    }
}
