using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicProjectile : BehaviourBase, IProjectile
{
    public CinemachineDollyCart cart;
    public CinemachinePathBase track;
    public DollyMoveEvent onTrackEndReached { get; set; }
    public CinemachineDollyCart GetCart() => cart;
    public CinemachinePathBase GetTrack() => track;
    public int GetDamage() => 5;
    public DamageType GetDamageType() => DamageType.PHYSICAL;
    public bool GetDestroyOnTrackFinished() => true;
}
