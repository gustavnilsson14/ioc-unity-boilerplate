using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicExplosive : BehaviourBase, ITimedExplosive
{
    public float currentTimeToDetonate { get; set; }
    public bool isCountingDown { get; set; }
    public TimedExplosiveEvent onExplosiveTimerStart { get; set; }
    public TimedExplosiveEvent onExplosiveTimerPause { get; set; }
    public TimedExplosiveEvent onExplosiveTimerReset { get; set; }
    public ExplosiveEvent onExplosiveDetonation { get; set; }
    public int currentHealth { get; set; }
    public bool alive { get; set; }
    public DamageEvent onHit { get; set; }
    public DamageEvent onArmorHit { get; set; }
    public DamageEvent onArmorBroken { get; set; }
    public DamageEvent onResist { get; set; }
    public DamageEvent onDeath { get; set; }
    public Animator animator { get; set; }

    public int GetDamage() => 5;

    public float GetDamageFalloffMultiplier() => 1;

    public DamageType GetDamageType() => DamageType.PHYSICAL;

    public float GetDecayTime() => 1;

    public int GetMaxHealth() => 99;

    public float GetShockForce() => 1000;

    public float GetShockRadius() => 10;

    public float GetTimeToDetonate() => 5;

    public bool test = false;
    void Update()
    {
        if (test)
        {
            test = false;
            ExplosionLogic.I.StartTimedCountdown(this);
        }
    }
}
