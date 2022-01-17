using UnityEngine;

public interface IObjectFactory 
{
    GameObject PopObject(string name);
    void PushObject(string name,GameObject go);
}
