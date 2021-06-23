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
    public List<IInvulnerable> invulnerables = new List<IInvulnerable>();
    protected override void OnInstantiate(GameObject newInstance, IBase newBase)
    {
        base.OnInstantiate(newInstance, newBase);
        InitDamageable(newBase as IDamageable);
        InitArmor(newBase as IArmor);
        InitInvulnerable(newBase as IInvulnerable);
    }

    private void InitDamageable(IDamageable damageable)
    {
        if (damageable == null)
            return;
        damageable.currentHealth = damageable.GetMaxHealth();
        damageable.alive = true;
        damageable.onArmorBroken = new DamageEvent();
        damageable.onArmorHit= new DamageEvent();
        damageable.onDeath = new DamageEvent();
        damageable.onHit = new DamageEvent();
        damageable.onResist = new DamageEvent();
    }

    private void InitArmor(IArmor armor)
    {
        if (armor == null)
            return;
        armor.currentDurability = armor.GetMaxDurability();
    }
    private void InitInvulnerable(IInvulnerable invulnerable)
    {
        if (invulnerable == null)
            return;
        invulnerables.Add(invulnerable);
        invulnerable.onInvulnerableEnd = new InvulnerableEvent();
        invulnerable.onInvulnerableStart = new InvulnerableEvent();
        invulnerable.onInvulnerableHit = new InvulnerableEvent();
    }
    private void Update()
    {
        invulnerables.ForEach(x => UpdateInvulnerable(x));
    }

    private void UpdateInvulnerable(IInvulnerable invulnerable)
    {
        float previous = invulnerable.currentInvulnerableTime;
        invulnerable.currentInvulnerableTime -= Time.deltaTime;
        invulnerable.currentInvulnerableCooldown -= Time.deltaTime;
        if (previous > 0 && invulnerable.currentInvulnerableTime < 0)
            invulnerable.onInvulnerableEnd.Invoke(invulnerable);
    }

    protected override void UnRegister(IBase b)
    {
        base.UnRegister(b);
        if (b is IInvulnerable)
            invulnerables.Remove(b as IInvulnerable);
    }
    public void StartInvulnerability(IInvulnerable invulnerable) {
        if (invulnerable.currentInvulnerableCooldown > 0)
            return;
        invulnerable.onInvulnerableStart.Invoke(invulnerable);
        invulnerable.currentInvulnerableCooldown = invulnerable.GetInvulnerableCooldown();
        invulnerable.currentInvulnerableTime = invulnerable.GetInvulnerableTime();
    }
    public void TakeDamage(Collision collision, IDamageSource damageSource)
    {
        if (collision.rigidbody == null)
            return;
        if (!collision.rigidbody.TryGetComponent(out IDamageable damageable))
            return;
        if (IsInvulnerable(collision.rigidbody, out IInvulnerable invulnerable)) {
            invulnerable.onInvulnerableHit.Invoke(invulnerable);
            return;
        }
        TakeDamage(damageable, damageSource);
    }

    private bool IsInvulnerable(Rigidbody rigidbody, out IInvulnerable invulnerable)
    {
        if (!rigidbody.TryGetComponent(out invulnerable))
            return false;
        return invulnerable.currentInvulnerableTime > 0;
    }

    public void TakeDamage(IDamageable damageable, IDamageSource damageSource)
    {
        if (!damageable.alive)
            return;
        if (WasAbsorbed(damageable, damageSource, out int damageRemaining)) {
            damageable.onResist.Invoke(damageable, damageSource);
            return;
        }
        damageable.onHit.Invoke(damageable, damageSource);
        damageable.currentHealth = Mathf.Clamp(damageable.currentHealth - damageRemaining, 0, int.MaxValue);
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

public interface IInvulnerable : IBase
{
    InvulnerableEvent onInvulnerableStart { get; set; }
    InvulnerableEvent onInvulnerableEnd { get; set; }
    InvulnerableEvent onInvulnerableHit { get; set; }
    float GetInvulnerableTime();
    float GetInvulnerableCooldown();
    float currentInvulnerableTime { get; set; }
    float currentInvulnerableCooldown { get; set; }
}

public class DamageEvent : UnityEvent<IDamageable, IDamageSource> { }
public class InvulnerableEvent : UnityEvent<IInvulnerable> { }