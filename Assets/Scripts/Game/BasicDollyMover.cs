using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicDollyMover : BehaviourBase, IDollyMover
{
    public CinemachinePathBase track;
    public CinemachineDollyCart cart;

    public DollyMoveEvent onTrackEndReached { get; set; }
    public float destroyDelay { get; set; }

    public CinemachineDollyCart GetCart() => cart;

    public bool GetDestroyOnTrackFinished() => true;

    public CinemachinePathBase GetTrack() => track;
}
