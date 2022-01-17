using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 正常模式下战斗场景状态
/// </summary>
public class GameNormalState : BaseSceneState
{
    public override int sceneID => 3;

    public GameNormalState(UIFacade uIFacade) : base(uIFacade)
    {

    }

    public override void DoBeforeEntering()
    {            
        uIFacade.AddScenePanel(StringManager.NormalModelPanel);
        uIFacade.EnterPanel(StringManager.NormalModelPanel);
        base.DoBeforeEntering();
    }
}
