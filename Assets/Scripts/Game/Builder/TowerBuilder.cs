using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerBuilder : IBuilder<Tower>
{
    public int towerID, towerLevel;

    public void GetData(Tower productClass)
    {
        throw new System.NotImplementedException();
    }

    public void GetOtherResources(Tower product)
    {
        throw new System.NotImplementedException();
    }

    public Tower GetProductClass(GameObject go)
    {
        throw new System.NotImplementedException();
    }

    public GameObject GetProducts()
    {
        throw new System.NotImplementedException();
    }

}
