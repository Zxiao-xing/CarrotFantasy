using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class MainMenuState : BaseSceneState
{
    public override int sceneID =>1;

    public MainMenuState(UIFacade uIFacade) : base(uIFacade)
    {  }

    public override void DoBeforeEntering()
    {
        uIFacade.AddScenePanel(StringManager.MainPanel);
        uIFacade.AddScenePanel(StringManager.SetPanel);
        uIFacade.AddScenePanel(StringManager.HelpPanel);
        uIFacade.AddScenePanel(StringManager.GameLoadPanel);
        uIFacade.EnterPanel(StringManager.MainPanel);
        base.DoBeforeEntering();
    }
}
