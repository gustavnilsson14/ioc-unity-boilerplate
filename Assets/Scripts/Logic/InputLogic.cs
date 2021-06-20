using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;

public enum InputType { 
    SHOOT,
    MOVE,
    OPEN_INVENTORY,

}

public class InputLogic : InterfaceLogicBase
{
    public static InputLogic I;
    protected override void OnInstantiate(GameObject newInstance)
    {
        base.OnInstantiate(newInstance);
        InitInputReciever(newInstance);
    }

    private void InitInputReciever(GameObject newInstance)
    {
        if (!newInstance.TryGetComponent<IInputReciever>(out IInputReciever inputReciever))
            return;
        inputReciever.onInput = new InputEvent();
    }

    void Update()
    {
        myInstances.ForEach(x => GetInputForReciever(x.GetComponent<IInputReciever>()));
    }

    private void GetInputForReciever(IInputReciever inputReciever)
    {
    }
}
public interface IInputReciever : IBase
{
    List<KeyCode> GetKeyCodes();
    InputEvent onInput { get; set; }
}
public class InputEvent : UnityEvent<KeyCode> { }