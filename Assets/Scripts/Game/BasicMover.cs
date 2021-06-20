using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicMover : BehaviourBase, IMover
{
    public float speed;

    public Vector3 previousMovementVector { get; set; }
    public MoveEvent onMove { get; set; }
    public MoveEvent onStart { get; set; }
    public MoveEvent onStop { get; set; }

    public Vector3 movementVector { get; set; }
    public bool hasDestination { get; set; }
    public Vector3 destination { get; set; }
    public bool isGrounded { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
    public bool previousIsGrounded { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
    public MoveEvent onLand { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

    public RigidbodyConstraints GetConstraints()
    {
        throw new System.NotImplementedException();
    }

    public bool GetRotateTowardsMouse()
    {
        throw new System.NotImplementedException();
    }

    public float GetSpeed() => speed;

    public Transform GetTransform() => transform;
}
