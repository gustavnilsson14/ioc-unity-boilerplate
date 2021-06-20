using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Armor : BehaviourBase, IArmor
{
    public List<DamageType> damageTypes;
    public int maxDurability;
    public int currentDurability { get; set; }
    public List<DamageType> GetDamageTypes() => damageTypes;
    public int GetMaxDurability() => maxDurability;

}
