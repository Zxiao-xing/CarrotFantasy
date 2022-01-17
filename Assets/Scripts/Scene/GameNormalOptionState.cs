using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameNormalOptionState : BaseSceneState
{
    public override int sceneID => 2;

    public GameNormalOptionState(UIFacade uIFacade) : base(uIFacade)
    {   }

    public override void DoBeforeEntering()
    {      
        uIFacade.AddScenePanel(StringManager.GameNormalOptionPanel);
        uIFacade.AddScenePanel(StringManager.NormalBigLevelPanel);
        uIFacade.AddScenePanel(StringManager.NormalLevelPanel);     
        uIFacade.AddScenePanel(StringManager.HelpPanel);
        uIFacade.AddScenePanel(StringManager.GameLoadPanel);

        uIFacade.EnterPanel(StringManager.GameNormalOptionPanel);
        uIFacade.EnterPanel(StringManager.NormalBigLevelPanel);
      
        base.DoBeforeEntering();
    }

}
