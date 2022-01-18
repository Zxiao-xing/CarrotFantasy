using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prize : MonoBehaviour
{
    private void OnMouseDown()
    {
        GameController.GetInstance().GivePrize();
        GameController.GetInstance().PushObject(ObjectFactoryType.GameFactory, "Prize", gameObject);
    }

}
