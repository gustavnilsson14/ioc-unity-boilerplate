using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Resistance : BehaviourBase, IResistance
{
    public List<DamageType> damageTypes;
    public int flatReduction;

    public List<DamageType> GetDamageTypes() => damageTypes;
    public int GetFlatReduction() => flatReduction;
}
