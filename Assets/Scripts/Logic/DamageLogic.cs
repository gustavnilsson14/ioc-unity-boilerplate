using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;

public enum DamageType { 
    NONE, PHYSICAL, MAGICAL
}

public class DamageLogic : InterfaceLogicBase
{
    public static DamageLogic I;
    protected override void OnInstantiate(GameObject newInstance)
    {
        base.OnInstantiate(newInstance);
        InitDamageable(newInstance);
        InitArmor(newInstance);
    }

    private void InitDamageable(GameObject newInstance)
    {
        if (!newInstance.TryGetComponent(out IDamageable damageable))
            return;
        damageable.currentHealth = damageable.GetMaxHealth();
        damageable.alive = true;
        damageable.onArmorBroken = new DamageEvent();
        damageable.onArmorHit= new DamageEvent();
        damageable.onDeath = new DamageEvent();
        damageable.onHit = new DamageEvent();
        damageable.onResist = new DamageEvent();
    }

    private void InitArmor(GameObject newInstance)
    {
        if (!newInstance.TryGetComponent<IArmor>(out IArmor armor))
            return;
        armor.currentDurability = armor.GetMaxDurability();
    }

    internal void TakeDamage(Collision collision, IDamageSource damageSource)
    {
        Debug.Log($"TakeDamage(Collision {collision.rigidbody}, IDamageSource {damageSource})");
        if (!collision.rigidbody.TryGetComponent(out IDamageable damageable))
            return;
        Debug.Log($"if (!collision.collider.TryGetComponent(out IDamageable {damageable}))");
        TakeDamage(damageable, damageSource);
    }

    public void TakeDamage(IDamageable damageable, IDamageSource damageSource)
    {
        Debug.Log($"IDamageable {damageable}, IDamageSource {damageSource} {damageable.alive}");
        if (!damageable.alive)
            return;
        if (WasAbsorbed(damageable, damageSource, out int damageRemaining)) {
            damageable.onResist.Invoke(damageable, damageSource);
            return;
        }
        damageable.onHit.Invoke(damageable, damageSource);
        damageable.currentHealth = Mathf.Clamp(damageable.currentHealth - damageRemaining, 0, int.MaxValue);
        Debug.Log($"damageable.currentHealth {damageable.currentHealth}");
        if (damageable.currentHealth > 0)
            return;
        Die(damageable, damageSource);
    }

    private bool WasAbsorbed(IDamageable damageable, IDamageSource damageSource, out int damageRemaining)
    {
        damageRemaining = damageSource.GetDamage();
        damageRemaining -= GetArmorReduction(damageable, damageSource);
        damageRemaining -= GetResistanceReduction(damageable, damageSource);
        bool result = (damageRemaining <= 0);

        return result;
    }

    private int GetArmorReduction(IDamageable damageable, IDamageSource damageSource)
    {
        int result = 0;
        int damage = damageSource.GetDamage();
        foreach (IArmor armor in damageable.GetGameObject().GetComponents<IArmor>())
        {
            if (!armor.GetDamageTypes().Contains(damageSource.GetDamageType()))
                continue;
            int damageToArmor = Mathf.Clamp(damage, 0, armor.currentDurability);
            result += damageToArmor;
            armor.currentDurability -= damageToArmor;
        }
        if (result > 0)
            damageable.onArmorHit.Invoke(damageable, damageSource);
        return result;
    }

    private int GetResistanceReduction(IDamageable damageable, IDamageSource damageSource)
    {
        int result = 0;
        foreach (IResistance resistance in damageable.GetGameObject().GetComponents<IResistance>())
        {
            if (!resistance.GetDamageTypes().Contains(damageSource.GetDamageType()))
                continue;
            result += resistance.GetFlatReduction();
        }
        return result;
    }

    private void Die(IDamageable damageable, IDamageSource damageSource)
    {
        damageable.alive = false;
        damageable.onDeath.Invoke(damageable, damageSource);
        Debug.Log($"private void Die(IDamageable {damageable}, IDamageSource {damageSource})");
        Destroy(damageable.GetGameObject(), damageable.GetDecayTime());
    }
}
public interface IDamageable : IBase
{
    float GetDecayTime();
    int GetMaxHealth();
    int currentHealth { get; set; }
    bool alive { get; set; }
    DamageEvent onHit { get; set; }
    DamageEvent onArmorHit { get; set; }
    DamageEvent onArmorBroken { get; set; }
    DamageEvent onResist { get; set; }
    DamageEvent onDeath { get; set; }
}
public interface IArmor : IBase
{
    List<DamageType> GetDamageTypes();
    int GetMaxDurability();
    int currentDurability { get; set; }
}
public interface IResistance : IBase
{
    List<DamageType> GetDamageTypes();
    int GetFlatReduction();
}
public interface IDamageSource : IBase
{
    int GetDamage();
    DamageType GetDamageType();
}

public class DamageEvent : UnityEvent<IDamageable, IDamageSource> { }