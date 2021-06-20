using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class JumpLogic : InterfaceLogicBase
{
    public static JumpLogic I;

    protected override void OnInstantiate(GameObject newInstance)
    {
        base.OnInstantiate(newInstance);
        InitJumper(newInstance);
    }
    private void InitJumper(GameObject newInstance)
    {
        if (!newInstance.TryGetComponent<IJumper>(out IJumper jumper))
            return;
        jumper.onJump = new JumpEvent();
        jumper.onLand.AddListener(OnJumperLand);
    }

    private void OnJumperLand(IMover mover)
    {
        ResetAirJumps((mover as IJumper));
    }

    public bool Jump(IJumper jumper) {
        if (!CanJump(jumper))
            return false;
        jumper.onJump.Invoke(jumper);
        jumper.GetGameObject().GetComponent<Rigidbody>().AddForce(Vector3.up * jumper.GetJumpForce(), ForceMode.Impulse);
        return true;
    }

    private bool CanJump(IJumper jumper)
    {
        if (jumper.isGrounded)
            return true;
        if (jumper.airJumps == 0)
            return false;
        jumper.airJumps--;
        return true;
    }

    private void ResetAirJumps(IJumper jumper)
    {
        jumper.airJumps = jumper.GetMaxAirJumps();
    }
}
public interface IJumper : IMover
{
    float GetJumpForce();
    int GetMaxAirJumps();
    JumpEvent onJump { get; set; }
    int airJumps { get; set; }
}
public class JumpEvent : UnityEvent<IJumper> { }
