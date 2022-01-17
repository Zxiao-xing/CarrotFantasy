using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPanelFactory : BaseObjectFactory
{
    public UIPanelFactory()
    {
        path += "UIPanel/";
    }


    public override GameObject PopObject(string name)
    {
        GameObject go= base.PopObject(name);
        //面板不需要一实例化就激活,在面板进入起作用时才需要激活
        go.SetActive(false);
        return go;
    }


}
