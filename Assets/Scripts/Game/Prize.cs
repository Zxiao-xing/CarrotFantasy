using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prize : MonoBehaviour
{
    private void OnMouseDown()
    {
        GameController._Ins.GivePrize();
        GameController._Ins.PushObject(ObjectFactoryType.GameFactory, "Prize", gameObject);
    }

}
