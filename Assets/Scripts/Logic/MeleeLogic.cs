using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class MeleeLogic : InterfaceLogicBase
{
    public static MeleeLogic I;
    public List<IMeleeAttacker> meleeAttackers = new List<IMeleeAttacker>();

    protected override void OnInstantiate(GameObject newInstance)
    {
        base.OnInstantiate(newInstance);
        InitMeleeAttacker(newInstance);
    }
    private void InitMeleeAttacker(GameObject newInstance)
    {
        if (!newInstance.TryGetComponent(out IMeleeAttacker meleeAttacker))
            return;
        meleeAttackers.Add(meleeAttacker);
        meleeAttacker.onAttackStart = new MeleeEvent();
        meleeAttacker.onAttackFinish = new MeleeEvent();
    }
    protected override void UnRegister(IBase b)
    {
        base.UnRegister(b);
        if ((b is IMeleeAttacker))
            meleeAttackers.Remove(b as IMeleeAttacker);
    }
    private void Update()
    {
        meleeAttackers.ForEach(x => UpdateMeleeAttacker(x));
    }

    private void UpdateMeleeAttacker(IMeleeAttacker meleeAttacker)
    {
        meleeAttacker.currentMeleeCooldown -= Time.deltaTime;
    }

    public void Attack(IMeleeAttacker meleeAttacker)
    {
        if (meleeAttacker.currentMeleeCooldown > 0)
            return;
        meleeAttacker.currentMeleeCooldown = meleeAttacker.GetMeleeCooldown();
        meleeAttacker.onAttackStart.Invoke(meleeAttacker);
        StartCoroutine(DelayedDealDamage(meleeAttacker));
    }

    private IEnumerator DelayedDealDamage(IMeleeAttacker meleeAttacker)
    {
        yield return new WaitForSeconds(meleeAttacker.GetDamageDelay());
        meleeAttacker.onAttackFinish.Invoke(meleeAttacker);
        SphereCollider damageCollider = meleeAttacker.GetDamageCollider();
        foreach (Collider collider in Physics.OverlapSphere(damageCollider.transform.position, damageCollider.radius))
        {
            if (collider == damageCollider)
                continue;
            IDamageable damageable = collider.GetComponentInParent<IDamageable>();
            if (damageable == null)
                continue;
            DealDamage(meleeAttacker, damageable);
        }
    }

    private void DealDamage(IMeleeAttacker meleeAttacker, IDamageable damageable)
    {
        DamageLogic.I.TakeDamage(damageable, meleeAttacker);
    }
}
public interface IMeleeAttacker : IDamageSource
{
    MeleeEvent onAttackStart { get; set; }
    MeleeEvent onAttackFinish { get; set; }
    float GetDamageDelay();
    SphereCollider GetDamageCollider();
    float GetMeleeCooldown();
    float currentMeleeCooldown { get; set; }
}
public class MeleeEvent : UnityEvent<IMeleeAttacker> { }