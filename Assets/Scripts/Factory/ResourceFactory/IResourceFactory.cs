using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IResourceFactory<T>
{
    T GetResource(string resourcePath);
}
