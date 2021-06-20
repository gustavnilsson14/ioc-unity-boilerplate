using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicProjectile : BehaviourBase, IProjectile
{
    public CinemachineDollyCart cart;
    public CinemachinePathBase track;
    public TrailRenderer trail;
    public Vector3 movementNoise;

    public DollyMoveEvent onTrackEndReached { get; set; }
    public Collider projectileCollider { get; set; }
    public float destroyDelay { get; set; }

    public CinemachineDollyCart GetCart() => cart;
    public CinemachinePathBase GetTrack() => track;
    public int GetDamage() => 5;
    public DamageType GetDamageType() => DamageType.PHYSICAL;
    public bool GetDestroyOnTrackFinished() => true;

    public TrailRenderer GetTrail() => trail;

    public Vector3 GetMovementNoise() => movementNoise;
}
