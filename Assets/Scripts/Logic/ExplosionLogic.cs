using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class ExplosionLogic : InterfaceLogicBase
{
    public static ExplosionLogic I;
    private List<ITimedExplosive> timedExplosives = new List<ITimedExplosive>();

    protected override void OnInstantiate(GameObject newInstance, IBase newBase)
    {
        base.OnInstantiate(newInstance, newBase);
        InitExplosive(newBase as IExplosive);
        InitTimedExplosive(newBase as ITimedExplosive);
        InitContactExplosive(newBase as IContactExplosive);
    }
    protected override void OnRegisterInternalListeners(IBase newBase)
    {
        RegisterExplosiveInternalListeners(newBase as IExplosive);
    }
    private void RegisterExplosiveInternalListeners(IExplosive explosive)
    {
        if (explosive == null)
            return;
        explosive.onHit.AddListener(Detonate);
    }

    private void InitExplosive(IExplosive explosive)
    {
        if (explosive == null)
            return;
        explosive.onExplosiveDetonation = new ExplosiveEvent();
    }
    private void InitTimedExplosive(ITimedExplosive timedExplosive)
    {
        if (timedExplosive == null)
            return;
        timedExplosive.onExplosiveTimerStart = new TimedExplosiveEvent();
        timedExplosive.onExplosiveTimerPause = new TimedExplosiveEvent();
        timedExplosive.onExplosiveTimerReset = new TimedExplosiveEvent();
        timedExplosive.isCountingDown = false;
        timedExplosive.currentTimeToDetonate = timedExplosive.GetTimeToDetonate();
        timedExplosives.Add(timedExplosive);
    }

    private void InitContactExplosive(IContactExplosive contactExplosive)
    {
        if (contactExplosive == null)
            return;
    }
    protected override void UnRegister(IBase b)
    {
        base.UnRegister(b, new List<IList>() {
            timedExplosives
        });
    }
    private void Update()
    {
        timedExplosives.ForEach(x => UpdateTimedExplosive(x));
    }

    private void UpdateTimedExplosive(ITimedExplosive timedExplosive)
    {
        if (!timedExplosive.isCountingDown)
            return;
        timedExplosive.currentTimeToDetonate -= Time.deltaTime;
        if (timedExplosive.currentTimeToDetonate > 0)
            return;
        Detonate(timedExplosive);
    }
    public void StartTimedCountdown(ITimedExplosive timedExplosive)
    {
        timedExplosive.isCountingDown = true;
        timedExplosive.onExplosiveTimerStart.Invoke(timedExplosive);
    }
    public void PauseTimedCountdown(ITimedExplosive timedExplosive)
    {
        timedExplosive.isCountingDown = false;
        timedExplosive.onExplosiveTimerPause.Invoke(timedExplosive);
    }
    public void ResetTimedCountdown(ITimedExplosive timedExplosive)
    {
        timedExplosive.currentTimeToDetonate = timedExplosive.GetTimeToDetonate();
        timedExplosive.onExplosiveTimerReset.Invoke(timedExplosive);
    }

    private void Detonate(IDamageable explosive, IDamageSource arg1)
    {
        Detonate(explosive as IExplosive);
    }
    private void Detonate(IExplosive explosive)
    {
        List<Collider> hits = Physics.OverlapSphere(explosive.GetGameObject().transform.position, explosive.GetShockRadius()).ToList();
        hits.ForEach(x => ApplyExplosionToHit(explosive, x));
        if (!(explosive is ITimedExplosive))
            return;
        (explosive as ITimedExplosive).isCountingDown = false;
    }

    private void ApplyExplosionToHit(IExplosive explosive, Collider collider)
    {
        if (collider.attachedRigidbody == null)
            return;
        if (collider.attachedRigidbody.gameObject == explosive.GetGameObject())
            return;
        collider.attachedRigidbody.AddExplosionForce(explosive.GetShockForce(), explosive.GetGameObject().transform.position, explosive.GetShockRadius());
        if (!collider.attachedRigidbody.TryGetComponent(out IDamageable damageable))
            return;
        DamageLogic.I.TakeDamage(damageable, explosive);
    }
}
public interface IExplosive : IDamageSource, IDamageable, IAnimated
{
    float GetShockForce();
    float GetShockRadius();
    float GetDamageFalloffMultiplier();
    ExplosiveEvent onExplosiveDetonation { get; set; }
}
public interface ITimedExplosive : IExplosive
{
    float GetTimeToDetonate();
    float currentTimeToDetonate { get; set; }
    bool isCountingDown { get; set; }
    TimedExplosiveEvent onExplosiveTimerStart { get; set; }
    TimedExplosiveEvent onExplosiveTimerPause { get; set; }
    TimedExplosiveEvent onExplosiveTimerReset { get; set; }
}
public interface IContactExplosive : IExplosive
{

}
public class ExplosiveEvent : UnityEvent<IExplosive> { }
public class TimedExplosiveEvent : UnityEvent<ITimedExplosive> { }
public class ContactExplosiveEvent : UnityEvent<IContactExplosive> { }