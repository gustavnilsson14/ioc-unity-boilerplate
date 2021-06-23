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
    public ExplosiveEvent onExplosiveDetonationImminent { get; set; }

    public int GetDamage() => 5;

    public float GetDamageFalloffMultiplier() => 1;

    public DamageType GetDamageType() => DamageType.PHYSICAL;

    public float GetDecayTime() => 1;

    public int GetMaxHealth() => 99;

    public float GetShockForce() => shockForce;

    public float GetShockRadius() => shockRadius;

    public float GetTimeToDetonate() => 5;

    public bool test = false;
    public float shockForce = 1000;
    public float shockRadius = 5;

    void Update()
    {
        if (test)
        {
            test = false;
            ExplosionLogic.I.StartTimedCountdown(this);
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = (new Color(0.1f * (GetShockForce()/500), 0, 0, 0.5f));
        Gizmos.DrawSphere(transform.position, GetShockRadius());
    }

    public float GetWarningTime() => 1;
}
