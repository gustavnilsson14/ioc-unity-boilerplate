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
        if (animated is IShooter)
            RegisterShooterAnimations(animated as IShooter);
        if (animated is IUsableItem)
            RegisterUsableItemAnimations(animated as IUsableItem);
        if (animated is IMeleeWeapon)
            RegisterMeleeWeaponsAnimations(animated as IMeleeWeapon);
        if (animated is IComboItem)
            RegisterComboItemAnimations(animated as IComboItem);
        animated.animator = animator;
    }

    private void RegisterUsableItemAnimations(IUsableItem usableItem)
    {
        usableItem.onItemUse.AddListener(OnItemUse);
    }

    private void RegisterMeleeWeaponsAnimations(IMeleeWeapon meleeWeapon)
    {
        meleeWeapon.onMeleeWeaponDealDamage.AddListener(OnMeleeWeaponDealDamage);
    }

    private void RegisterComboItemAnimations(IComboItem comboItem)
    {
        comboItem.onComboItemUse.AddListener(OnComboItemUse);
    }

    private void RegisterShooterAnimations(IShooter shooter)
    {
        shooter.onSpawn.AddListener(OnShoot);
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

    private void OnShoot(ISpawner animated, GameObject arg1)
    {
        PlayAnimation(animated as IAnimated, "Shoot");
    }

    private void OnItemUse(IUsableItem animated)
    {
        PlayAnimation(animated as IAnimated, "Use");
    }

    private void OnMeleeWeaponDealDamage(IMeleeWeapon animated)
    {
        PlayAnimation(animated, "DealDamage");
    }

    private void OnComboItemUse(IComboItem animated)
    {
        PlayAnimation(animated as IAnimated, $"Use{animated.currentComboIndex}");
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