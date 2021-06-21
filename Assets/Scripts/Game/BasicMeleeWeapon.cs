using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicMeleeWeapon : BehaviourBase, IMeleeWeapon
{
    public int damage;
    public DamageType damageType;
    public float cooldown;
    public float damageDelay;
    public Transform damagePoint;
    public int maxCombo;
    public MeleeWeaponType meleeWeaponType;
    public float comboTimeWindow;

    public int ammo { get; set; }
    public UsableItemEvent onItemUse { get; set; }
    public UsableItemEvent onItemOutOfAmmo { get; set; }
    public UsableItemEvent onReload { get; set; }
    public float currentItemCooldown { get; set; }
    public Animator animator { get; set; }
    public MeleeWeaponEvent onMeleeWeaponDealDamage { get; set; }
    public int currentComboIndex { get; set; }
    public float currentComboWindow { get; set; }
    public ComboItemEvent onComboItemUse { get; set; }

    public int GetAmmoCapacity() => 0;

    public ResourceType GetAmmoType() => ResourceType.BULLETS;

    public float GetComboTimeWindow() => comboTimeWindow;

    public int GetDamage() => damage;

    public float GetDamageDelay() => damageDelay;

    public Transform GetDamagePoint() => damagePoint;

    public DamageType GetDamageType() => damageType;

    public float GetItemCooldown() => cooldown;

    public ItemType GetItemType() => ItemType.MELEE_WEAPON;

    public int GetMaxCombo() => maxCombo;

    public MeleeWeaponType GetMeleeWeaponType() => meleeWeaponType;

    public bool GetUsesAmmo() => false;
}
