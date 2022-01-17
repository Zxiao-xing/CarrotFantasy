using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBuilder<T>
{
    //生成物体
     GameObject GetProducts();
     T GetProductClass(GameObject go);
    //为物体身上的脚本初始属性
    void GetData(T productClass);
    //获取其他特异性属性
    void GetOtherResources(T product);
}
