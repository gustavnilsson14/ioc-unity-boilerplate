using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AnimationLogic : InterfaceLogicBase
{
    public static AnimationLogic I;

    protected override void OnInstantiate(GameObject newInstance, IBase newBase)
    {
        base.OnInstantiate(newInstance, newBase);
        InitAnimated(newBase as IAnimated);
    }
    private void InitAnimated(IAnimated animated)
    {
        if (animated == null)
            return;
        if (!GetAnimator(animated, out Animator animator))
        {
            Destroy(animated.GetGameObject(),0.01f);
            return;
        }
        if (animated is IDamageable)
            RegisterDamageableAnimations(animated as IDamageable);
        if (animated is IMeleeAttacker)
            RegisterMeleeAnimations(animated as IMeleeAttacker);
        if (animated is IShooter)
            RegisterShooterAnimations(animated as IShooter);
        animated.animator = animator;
    }

    private void RegisterShooterAnimations(IShooter shooter)
    {
        shooter.onSpawn.AddListener(OnShoot);
    }

    private void RegisterMeleeAnimations(IMeleeAttacker meleeAttacker)
    {
        meleeAttacker.onAttackStart.AddListener(OnAttackStart);
        meleeAttacker.onAttackFinish.AddListener(OnAttackFinish);
    }

    private void RegisterDamageableAnimations(IDamageable damageable)
    {
        damageable.onDeath.AddListener(OnDeath);
        damageable.onHit.AddListener(OnHit);
    }

    private void OnHit(IDamageable animated, IDamageSource damageSource)
    {
        PlayAnimation(animated as IAnimated, "Hit");
    }

    private void OnDeath(IDamageable animated, IDamageSource damageSource)
    {
        PlayAnimation(animated as IAnimated, "Death");
    }

    private void OnAttackStart(IMeleeAttacker animated)
    {
        PlayAnimation(animated as IAnimated, "AttackStart");
    }

    private void OnAttackFinish(IMeleeAttacker animated)
    {
        PlayAnimation(animated as IAnimated, "AttackFinish");
    }

    private void OnShoot(ISpawner animated, GameObject arg1)
    {
        PlayAnimation(animated as IAnimated, "Shoot");
    }


    private bool GetAnimator(IAnimated animated, out Animator animator)
    {
        if (animated.GetGameObject().TryGetComponent(out animator))
            return true;
        animator = animated.GetGameObject().GetComponentInChildren<Animator>();
        return animator != null;
    }

    public void PlayAnimation(IAnimated animated, string animationName) {
        animated.animator.Play(animationName);
    }
    public void SetTrigger(IAnimated animated, string triggerName)
    {
        animated.animator.SetTrigger(triggerName);
    }
}
public interface IAnimated : IBase { 
    Animator animator { get; set; }
}