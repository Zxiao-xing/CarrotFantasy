using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasePanel : MonoBehaviour, IBasePanel
{
    protected UIFacade uIFacade;

    protected virtual void Start()
    {
        uIFacade = UIManager.GetInstance().mUIFacade;
    }

    public virtual void Enter()
    {
        gameObject.SetActive(true);
    }

    public virtual void Exit(){ }

    public virtual void Init(){ transform.localPosition = Vector3.one; }

    public virtual void UpdateUI()
    {
       
    }
}
