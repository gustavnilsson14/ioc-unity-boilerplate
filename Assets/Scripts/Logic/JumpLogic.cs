using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class JumpLogic : InterfaceLogicBase
{
    public static JumpLogic I;
    protected override void OnInstantiate(GameObject newInstance, IBase newBase)
    {
        base.OnInstantiate(newInstance, newBase);
        InitJumper(newBase as IJumper);
    }
    private void InitJumper(IJumper jumper)
    {
        if (jumper == null)
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
        Vector3 velocity = jumper.GetGameObject().GetComponent<Rigidbody>().velocity;
        velocity = new Vector3(velocity.x, jumper.GetJumpForce(), velocity.z);
        jumper.GetGameObject().GetComponent<Rigidbody>().velocity = velocity;
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
