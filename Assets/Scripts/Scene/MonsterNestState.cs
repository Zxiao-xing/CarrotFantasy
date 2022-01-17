using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterNestState : BaseSceneState
{
    public override int sceneID => 5;

    public MonsterNestState(UIFacade uIFacade) : base(uIFacade)
    {

    }

    public override void DoBeforeEntering()
    {
        uIFacade.AddScenePanel(StringManager.MonsterNestPanel);
        uIFacade.EnterPanel(StringManager.MonsterNestPanel);
        base.DoBeforeEntering();
    }
}
