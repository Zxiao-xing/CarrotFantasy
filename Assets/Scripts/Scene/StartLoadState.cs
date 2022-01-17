using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartLoadState : BaseSceneState
{
    public override int sceneID => 0;

    public StartLoadState(UIFacade uIFacade) : base(uIFacade)
    {
    }

    public override void DoBeforeEntering()
    {
        uIFacade.AddScenePanel(StringManager.StartLoadPanel);
        uIFacade.EnterPanel(StringManager.StartLoadPanel);
    }

}
