using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class StartLoadPanel : BasePanel
{
    public override void Enter()
    {
        base.Enter();
    }
    protected override void Start()
    {
        base.Start();     
        Invoke("NextScene", 2);
    }

    void NextScene()
    {
        uIFacade.ChangeScene(new MainMenuState(uIFacade));
        Invoke("ChangeScene",2);
    }

    void ChangeScene()
    {
        SceneManager.LoadScene(1);
    }
}
