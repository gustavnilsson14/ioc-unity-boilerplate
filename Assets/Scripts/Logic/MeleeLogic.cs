using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public enum MeleeWeaponType { 
    NORMAL, LIGHT, HEAVY, STABBY
}

public class MeleeLogic : InterfaceLogicBase
{
    public static MeleeLogic I;
    public List<IMeleeWeapon> meleeWeapons = new List<IMeleeWeapon>();

    private void Update()
    {
        meleeWeapons.ForEach(x => UpdateMeleeWeapon(x));
    }

    private void UpdateMeleeWeapon(IMeleeWeapon meleeWeapon)
    {
        meleeWeapon.currentComboWindow -= Time.deltaTime;
    }

    protected override void OnInstantiate(GameObject newInstance, IBase newBase)
    {
        base.OnInstantiate(newInstance, newBase);
        InitMeleeWeapon(newBase as IMeleeWeapon);
    }

    private void InitMeleeWeapon(IMeleeWeapon meleeWeapon)
    {
        if (meleeWeapon == null)
            return;
        meleeWeapons.Add(meleeWeapon);
        meleeWeapon.onComboItemUse.AddListener(OnWeaponUse);
        meleeWeapon.onMeleeWeaponDealDamage = new MeleeWeaponEvent();
    }

    protected override void UnRegister(IBase b)
    {
        base.UnRegister(b);
        if ((b is IMeleeWeapon))
            meleeWeapons.Remove(b as IMeleeWeapon);
    }

    private void OnWeaponUse(IComboItem comboItem)
    {
        IMeleeWeapon meleeWeapon = comboItem as IMeleeWeapon;
        if (meleeWeapon == null)
            return;
        StartCoroutine(DelayedDealDamage(meleeWeapon));
    }

    private IEnumerator DelayedDealDamage(IMeleeWeapon meleeWeapon)
    {
        yield return new WaitForSeconds(meleeWeapon.GetDamageDelay());
        DealDamage(meleeWeapon);
    }

    private void DealDamage(IMeleeWeapon meleeWeapon)
    {
        meleeWeapon.onMeleeWeaponDealDamage.Invoke(meleeWeapon);
        List<Collider> hits = Physics.OverlapSphere(meleeWeapon.GetDamagePoint().position, 1).ToList();
        hits = hits.FindAll(x => x.attachedRigidbody != null);
        Collider shield = hits.Find(x => x.attachedRigidbody.GetComponent<IShield>() != null);
        if (shield != null)
        {
            return;
        }
        hits.ForEach(x => DealDamage(meleeWeapon, x));
    }

    private void DealDamage(IMeleeWeapon meleeWeapon, Collider collider)
    {
        if (!collider.attachedRigidbody.TryGetComponent(out IDamageable damageable))
            return;
        EquippedItemLogic.I.GetHandler(meleeWeapon, out IItemUser itemUser);
        if (damageable.uniqueId == itemUser.uniqueId)
            return;
        DamageLogic.I.TakeDamage(damageable, meleeWeapon);
    }
}
public interface IMeleeWeapon : IDamageSource, IComboItem, IAnimated
{
    MeleeWeaponEvent onMeleeWeaponDealDamage { get; set; }
    float GetDamageDelay();
    Transform GetDamagePoint();
    MeleeWeaponType GetMeleeWeaponType();
}
public interface IShield : IDamageable
{

}
public class MeleeWeaponEvent : UnityEvent<IMeleeWeapon> { }