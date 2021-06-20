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
        jumper.onAirJump = new JumpEvent();
        jumper.onLand.AddListener(OnJumperLand);
    }

    private void OnJumperLand(IMover mover)
    {
        ResetAirJumps((mover as IJumper));
    }

    public bool Jump(IJumper jumper) {
        if (!CanJump(jumper, out bool isAirJump))
            return false;
        GetJumpEvent(jumper, isAirJump).Invoke(jumper);
        jumper.GetGameObject().GetComponent<Rigidbody>().AddForce(Vector3.up * jumper.GetJumpForce(), ForceMode.Impulse);
        return true;
    }

    private JumpEvent GetJumpEvent(IJumper jumper, bool isAirJump)
    {
        if (isAirJump)
            return jumper.onAirJump;
        return jumper.onJump;
    }

    private bool CanJump(IJumper jumper, out bool isAirJump)
    {
        isAirJump = false;
        if (jumper.isGrounded)
            return true;
        if (jumper.airJumps == 0)
            return false;
        jumper.airJumps--;
        isAirJump = true;
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
    JumpEvent onAirJump { get; set; }
    int airJumps { get; set; }
}
public class JumpEvent : UnityEvent<IJumper> { }
