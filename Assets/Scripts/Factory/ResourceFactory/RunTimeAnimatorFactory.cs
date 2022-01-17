using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunTimeAnimatorFactory : IResourceFactory<RuntimeAnimatorController>
{
    Dictionary<string, RuntimeAnimatorController> animatorDict = new Dictionary<string, RuntimeAnimatorController>();
    string path = "Animator/AnimatorController/";
    public RuntimeAnimatorController GetResource(string resourcePath)
    {
        RuntimeAnimatorController animator ;
        if (animatorDict.ContainsKey(resourcePath))
            animator = animatorDict[resourcePath];
        else
            animator = Resources.Load<RuntimeAnimatorController>(path + resourcePath);
        return animator;
    }

   
}
