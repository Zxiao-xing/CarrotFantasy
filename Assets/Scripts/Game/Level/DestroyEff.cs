using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyEff:MonoBehaviour
{
    [SerializeField] float liveTime;
    [SerializeField] string objectname;

    private void OnEnable()
    {
        Invoke("DestroyByTime", liveTime/ GameController.GetInstance().playSpeed);
    }

    void DestroyByTime()
    {
        FactoryManager.GetInstance().PushObject(ObjectFactoryType.GameFactory, objectname, gameObject);
    }
}
