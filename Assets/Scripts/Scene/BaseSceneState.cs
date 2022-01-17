using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public abstract class BaseSceneState : IBaseSceneState
{
   protected  UIFacade uIFacade;
    public abstract  int sceneID { get;  }

    public BaseSceneState(UIFacade uIFacade)
    {
        this.uIFacade = uIFacade;
    }
    public virtual void DoBeforeEntering()
    {
        SceneManager.LoadScene(sceneID);
    }

    public virtual void DoBeforeLeaving()
    {
        uIFacade.ClearScenePanelDict();
        uIFacade.EnterNewScene();
    }
}
