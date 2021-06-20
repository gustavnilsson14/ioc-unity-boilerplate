using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MeleeLogic : InterfaceLogicBase
{
    public static MeleeLogic I;

    protected override void OnInstantiate(GameObject newInstance)
    {
        base.OnInstantiate(newInstance);
        InitMeleeAttacker(newInstance);
    }
    private void InitMeleeAttacker(GameObject newInstance)
    {
        if (!newInstance.TryGetComponent<IMeleeAttacker>(out IMeleeAttacker meleeAttacker))
            return;
    }

    public void Attack(IMeleeAttacker meleeAttacker)
    {
        StartCoroutine(DelayedDealDamage(meleeAttacker));
    }

    private IEnumerator DelayedDealDamage(IMeleeAttacker meleeAttacker)
    {
        yield return new WaitForSeconds(meleeAttacker.GetDamageDelay());
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
public interface IMeleeAttacker : IDamageSource {
    float GetDamageDelay();
    SphereCollider GetDamageCollider();
}