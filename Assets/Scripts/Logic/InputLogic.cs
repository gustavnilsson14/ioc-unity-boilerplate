using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;

public enum InputType
{
    SHOOT,
    JUMP,
    MELEE,
    OPEN_INVENTORY,
}
public enum AxisType
{
    MOVE_X,
    MOVE_Y,
}
public enum InputHandlingType
{
    GET_KEY_DOWN,
    GET_KEY_UP,
    GET_KEY,
}

public class InputLogic : InterfaceLogicBase
{
    public static InputLogic I;
    public List<IInputReciever> inputRecievers = new List<IInputReciever>();
    protected override void OnInstantiate(GameObject newInstance)
    {
        base.OnInstantiate(newInstance);
        InitInputReciever(newInstance);
    }

    private void InitInputReciever(GameObject newInstance)
    {
        if (!newInstance.TryGetComponent<IInputReciever>(out IInputReciever inputReciever))
            return;
        inputRecievers.Add(inputReciever);
    }
    protected override void UnRegister(IBase b)
    {
        base.UnRegister(b);
        if ((b is IInputReciever))
            inputRecievers.Remove(b as IInputReciever);
    }

    void Update()
    {
        HandleInput();
    }

    private void HandleInput()
    {
        foreach (IInputReciever inputReciever in inputRecievers)
        {
            HandleShooterInput(inputReciever);
            HandleMoverInput(inputReciever);
            HandleJumperInput(inputReciever);
            HandleMeleeInput(inputReciever);
        }

    }

    private void HandleMoverInput(IInputReciever inputReciever)
    {
        if (!(inputReciever is IMover))
            return;
        Vector3 movementVector = Vector3.zero;
        foreach (AxisMapping axisMapping in inputReciever.GetAxisMappings().FindAll(x => x.axisType == AxisType.MOVE_X))
        {
            movementVector += new Vector3(Input.GetAxis(axisMapping.axisName), 0, 0);
        }
        foreach (AxisMapping axisMapping in inputReciever.GetAxisMappings().FindAll(x => x.axisType == AxisType.MOVE_Y))
        {
            movementVector += new Vector3(0, 0, Input.GetAxis(axisMapping.axisName));
        }
        (inputReciever as IMover).movementVector = movementVector;
    }

    private void HandleShooterInput(IInputReciever inputReciever)
    {
        if (!(inputReciever is IShooter))
            return;
        foreach (InputMapping inputMapping in inputReciever.GetInputMappings().FindAll(x => x.inputType == InputType.SHOOT))
        {
            if (!GetKeyInput(inputMapping))
                continue;
            ProjectileLogic.I.Shoot(inputReciever as IShooter);
        }
    }

    private void HandleJumperInput(IInputReciever inputReciever)
    {
        if (!(inputReciever is IJumper))
            return;
        foreach (InputMapping inputMapping in inputReciever.GetInputMappings().FindAll(x => x.inputType == InputType.JUMP))
        {
            if (!GetKeyInput(inputMapping))
                continue;
            JumpLogic.I.Jump(inputReciever as IJumper);
        }

    }

    private void HandleMeleeInput(IInputReciever inputReciever)
    {
        if (!(inputReciever is IMeleeAttacker))
            return;
        foreach (InputMapping inputMapping in inputReciever.GetInputMappings().FindAll(x => x.inputType == InputType.MELEE))
        {
            if (!GetKeyInput(inputMapping))
                continue;
            MeleeLogic.I.Attack(inputReciever as IMeleeAttacker);
        }

    }

    private bool GetKeyInput(InputMapping inputMapping){
        switch (inputMapping.inputHandlingType)
        {
            case InputHandlingType.GET_KEY_DOWN:
                return Input.GetKeyDown(inputMapping.keyCode);
            case InputHandlingType.GET_KEY_UP:
                return Input.GetKeyUp(inputMapping.keyCode);
            case InputHandlingType.GET_KEY:
                return Input.GetKey(inputMapping.keyCode);
        }
        return false;
    }

}
[System.Serializable]
public class InputMapping
{
    public KeyCode keyCode;
    public InputType inputType;
    public InputHandlingType inputHandlingType;
}
[System.Serializable]
public class AxisMapping
{
    public string axisName;
    public AxisType axisType;
}
public interface IInputReciever : IBase
{
    List<InputMapping> GetInputMappings();
    List<AxisMapping> GetAxisMappings();
}
public class InputEvent : UnityEvent<KeyCode> { }