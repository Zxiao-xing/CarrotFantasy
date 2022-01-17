using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBaseSceneState
{
    void DoBeforeEntering();
    void DoBeforeLeaving();
}
