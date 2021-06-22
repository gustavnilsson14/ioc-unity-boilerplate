using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using System;
using UnityEngine.Events;

public class MovementLogic : InterfaceLogicBase
{
    public static MovementLogic I;
    private List<IMover> movers = new List<IMover>();
    private List<IDasher> dashers = new List<IDasher>();
    private List<IDollyMover> dollyMovers = new List<IDollyMover>();

    protected override void OnInstantiate(GameObject newInstance, IBase newBase)
    {
        base.OnInstantiate(newInstance, newBase);
        InitMover(newBase as IMover);
        InitDollyMover(newBase as IDollyMover);
        InitDasher(newBase as IDasher);
        InitDodger(newBase as IDodger);
    }

    private void InitMover(IMover mover)
    {
        if (mover == null)
            return;
        movers.Add(mover);
        mover.hasDestination = false;
        mover.onLand = new MoveEvent();
        mover.onMove = new MoveEvent();
        mover.onStart = new MoveEvent();
        mover.onStop = new MoveEvent();
        if (!mover.GetGameObject().TryGetComponent(out Rigidbody r))
            mover.GetGameObject().AddComponent(typeof(Rigidbody));
    }

    private void InitDollyMover(IDollyMover dollyMover)
    {
        if (dollyMover == null)
            return;
        if (!dollyMover.GetGameObject().TryGetComponent(out Rigidbody r))
            dollyMover.GetGameObject().AddComponent(typeof(Rigidbody));
        dollyMover.onTrackEndReached = new DollyMoveEvent();
        dollyMovers.Add(dollyMover);
    }
    private void InitDasher(IDasher dasher)
    {
        if (dasher == null)
            return;
        dashers.Add(dasher);
        dasher.onDashEnd = new DashEvent();
        dasher.onDashStart = new DashEvent();
    }

    private void InitDodger(IDodger dodger)
    {
        if (dodger == null)
            return;
        dodger.onDodgeStart = new DodgeEvent();
    }
    protected override void UnRegister(IBase b)
    {
        base.UnRegister(b, new List<IList>() {
            movers,
            dollyMovers,
            dashers
        });
    }

    void Update()
    {
        movers.ForEach(x => Move(x));
        movers.ForEach(x => SetGroundedState(x));
        movers.ForEach(x => Rotate(x));
        dashers.ForEach(x => UpdateDasher(x));
        dollyMovers.ForEach(x => UpdateDollyMover(x));
    }

    private void UpdateDasher(IDasher dasher)
    {
        dasher.currentDashCooldown -= Time.deltaTime;
        float previous = dasher.currentDashDuration;
        dasher.currentDashDuration -= Time.deltaTime;
        if (previous > 0 && dasher.currentDashDuration < 0)
            dasher.onDashEnd.Invoke(dasher);
    }

    private void Rotate(IMover mover)
    {
        if (!mover.GetRotateTowardsMouse())
            return;
        
        Vector3 sceenCompensation = new Vector3(Screen.width / 2, 0, Screen.height / 2);
        Vector3 offset = Camera.main.WorldToScreenPoint(mover.GetGameObject().transform.position);
        offset = new Vector3(offset.x, 0, offset.y) - sceenCompensation;
        Vector3 mousePos = new Vector3(Input.mousePosition.x, 0, Input.mousePosition.y) - sceenCompensation;
        mousePos -= new Vector3(offset.x,0, offset.z);
        mover.GetGameObject().transform.LookAt(mover.GetGameObject().transform.position + mousePos, Vector3.up);
    }

    private void UpdateDollyMover(IDollyMover dollyMover)
    {
        HandleDollyTrackEnd(dollyMover);
    }

    private void HandleDollyTrackEnd(IDollyMover dollyMover)
    {   
        if (dollyMover.GetCart().m_Position < dollyMover.GetTrack().PathLength)
            return;
        dollyMover.onTrackEndReached.Invoke(dollyMover);
        if (dollyMover.GetDestroyOnTrackFinished())
            Destroy(dollyMover.GetGameObject(), dollyMover.destroyDelay);
    }

    private void Move(IMover mover)
    {
        HandleDestinationReached(mover);
        DidMoverStop(mover);
        DidMoverStart(mover);
        mover.previousMovementVector = mover.movementVector;
        if (!TryGetMovementVector(mover, out Vector3 movementVector))
            return;
        Rigidbody rigidbody = mover.GetGameObject().GetComponent<Rigidbody>();
        rigidbody.MovePosition(rigidbody.transform.position + (movementVector * GetSpeed(mover) * Time.deltaTime));
        mover.onMove.Invoke(mover);
    }

    private bool TryGetMovementVector(IMover mover, out Vector3 movementVector)
    {
        movementVector = mover.movementVector;
        if (TryGetDashVector(mover as IDasher, out Vector3 dashVector))
            movementVector = dashVector;
        if (movementVector == Vector3.zero)
            return false;
        return true;
    }

    private bool TryGetDashVector(IDasher dasher, out Vector3 dashVector)
    {
        dashVector = Vector3.zero;
        if (dasher == null)
            return false;
        if (dasher.currentDashDuration <= 0)
            return false;
        dashVector = dasher.dashVector;
        return true;
    }

    public void MoveTo(IMover mover, Vector3 destination)
    {
        mover.destination = destination;
        mover.hasDestination = true;
        mover.movementVector = (destination - mover.GetGameObject().transform.position).normalized;
    }

    private void HandleDestinationReached(IMover mover)
    {
        GameObject go = mover.GetGameObject();
        Vector3 flattenedPosition = new Vector3(go.transform.position.x, 0, go.transform.position.z);
        Vector3 flattenedDestination = new Vector3(mover.destination.x, 0, mover.destination.z);
        if (!mover.hasDestination)
            return;
        float distance = Vector3.Distance(flattenedPosition, flattenedDestination);
        if (distance > GetSpeed(mover) * Time.deltaTime)
            return;
        mover.hasDestination = false;
        go.transform.position += mover.movementVector * distance;
        mover.movementVector = Vector3.zero;
    }
    private void DidMoverStart(IMover mover)
    {
        if (mover.previousMovementVector != Vector3.zero || mover.movementVector == Vector3.zero)
            return;
        mover.onStart.Invoke(mover);
    }

    private void DidMoverStop(IMover mover)
    {
        if (mover.previousMovementVector == Vector3.zero || mover.movementVector != Vector3.zero)
            return;
        mover.onStop.Invoke(mover);
    }

    private void SetGroundedState(IMover mover)
    {
        GameObject gameObject = mover.GetGameObject();
        foreach (RaycastHit hit in Physics.RaycastAll(gameObject.transform.position, Vector3.down, 1))
        {
            if (hit.collider.gameObject == gameObject)
                continue;
            if (!mover.previousIsGrounded)
                mover.onLand.Invoke(mover);
            mover.isGrounded = true;
            return;
        }
        mover.isGrounded = false;
    }

    public void Dash(IDasher dasher) {
        if (dasher.currentDashCooldown > 0)
            return;
        if (dasher.movementVector == Vector3.zero)
            return;
        HandleDodge(dasher as IDodger);
        dasher.currentDashCooldown = dasher.GetDashCooldown();
        dasher.currentDashDuration = dasher.GetDashDuration();
        dasher.dashVector = dasher.movementVector;
    }

    private void HandleDodge(IDodger dodger)
    {
        if (dodger == null)
            return;
        DamageLogic.I.StartInvulnerability(dodger);
        dodger.onDodgeStart.Invoke(dodger);
    }

    public float GetSpeed(IMover mover) {
        float multiplier = 1;
        if (mover is IDasher) {
            if ((mover as IDasher).currentDashDuration > 0)
                multiplier *= (mover as IDasher).GetDashSpeedMultiplier();
        }
        if (mover is ISentient) {
            ISentient sentient = (mover as ISentient);
            float stress = SentientCreatureLogic.I.GetTotalDisposition(sentient).stress;
            multiplier *= stress + 0.5f;
        }
        return mover.GetSpeed() * multiplier;
    }
}

public interface IMover : IBase
{
    bool GetRotateTowardsMouse();
    float GetSpeed();
    Vector3 movementVector { get; set; }
    Vector3 previousMovementVector { get; set; }
    Vector3 destination { get; set; }
    bool hasDestination { get; set; }
    bool previousIsGrounded { get; set; }
    bool isGrounded { get; set; }
    MoveEvent onMove { get; set; }
    MoveEvent onStart { get; set; }
    MoveEvent onStop { get; set; }
    MoveEvent onLand { get; set; }
}
public interface IDollyMover : IBase
{
    float destroyDelay { get; set; }
    bool GetDestroyOnTrackFinished();
    CinemachineDollyCart GetCart();
    CinemachinePathBase GetTrack();
    DollyMoveEvent onTrackEndReached { get; set; }
}
public interface IDasher : IMover
{
    float GetDashSpeedMultiplier();
    float GetDashCooldown();
    float GetDashDuration();
    Vector3 dashVector { get; set; }
    float currentDashCooldown { get; set; }
    float currentDashDuration { get; set; }
    DashEvent onDashStart { get; set; }
    DashEvent onDashEnd { get; set; }
}
public interface IDodger : IDasher, IInvulnerable {
    DodgeEvent onDodgeStart { get; set; }
}
public class MoveEvent : UnityEvent<IMover> { }
public class DashEvent : UnityEvent<IDasher> { }
public class DodgeEvent : UnityEvent<IDodger> { }
public class DollyMoveEvent : UnityEvent<IDollyMover> { }
