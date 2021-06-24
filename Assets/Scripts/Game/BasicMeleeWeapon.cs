using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BasicMeleeWeapon : BehaviourBase, IMeleeWeapon, IPromptPickup, IWorldText
{
    public int damage;
    public DamageType damageType;
    public float cooldown;
    public float damageDelay;
    public Transform damagePoint;
    public int maxCombo;
    public MeleeWeaponType meleeWeaponType;
    public float comboTimeWindow;
    public string worldText;

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
    public PromptPickupEvent onPromptShow { get; set; }
    public PromptPickupEvent onPromptHide { get; set; }
    public bool isHeld { get; set; }
    public PickupEvent onPickupPickup { get; set; }
    public PickupEvent onPickupDrop { get; set; }
    public WorldTextEvent onWorldTextShow { get; set; }
    public WorldTextEvent onWorldTextHide { get; set; }
    public TextMeshPro textContainer { get; set; }

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

    public PickupType GetPickupType() => PickupType.USABLE_ITEM;

    public bool GetUsesAmmo() => false;

    public string GetWorldText() => worldText;

    public string SetWorldText(string text) => worldText = text;
}
