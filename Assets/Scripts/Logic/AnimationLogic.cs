using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AnimationLogic : InterfaceLogicBase
{
    public static AnimationLogic I;

    protected override void OnInstantiate(GameObject newInstance)
    {
        base.OnInstantiate(newInstance);
        InitAnimated(newInstance);
    }
    private void InitAnimated(GameObject newInstance)
    {
        if (!newInstance.TryGetComponent<IAnimated>(out IAnimated animated))
            return;
        if (!GetAnimator(animated, out Animator animator))
        {
            Destroy(animated.GetGameObject(),0.01f);
            return;
        }
        if (animated is IDamageable)
            RegisterDamageableAnimations((animated as IDamageable));
        animated.animator = animator;
    }

    private void RegisterDamageableAnimations(IDamageable damageable)
    {
        damageable.onDeath.AddListener(OnDeath);
        damageable.onHit.AddListener(OnHit);
    }

    private void OnHit(IDamageable animated, IDamageSource damageSource)
    {
        PlayAnimation((animated as IAnimated), "Hit");
    }

    private void OnDeath(IDamageable animated, IDamageSource damageSource)
    {
        PlayAnimation((animated as IAnimated), "Death");
    }

    private bool GetAnimator(IAnimated animated, out Animator animator)
    {
        if (animated.GetGameObject().TryGetComponent<Animator>(out animator))
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