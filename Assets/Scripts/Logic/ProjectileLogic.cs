using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileLogic : InterfaceLogicBase
{
    public static ProjectileLogic I;
    public List<IProjectile> projectiles = new List<IProjectile>();
    protected override void OnInstantiate(GameObject newInstance)
    {
        base.OnInstantiate(newInstance);
        InitShooter(newInstance);
        InitProjectile(newInstance);
    }

    private void InitShooter(GameObject newInstance)
    {
        if (!newInstance.TryGetComponent(out IShooter shooter))
            return;
    }

    private void InitProjectile(GameObject newInstance)
    {
        if (!newInstance.TryGetComponent(out IProjectile projectile))
            return;
        projectiles.Add(projectile);
        projectile.onCollision.AddListener(OnProjectileCollision);
    }

    private void OnProjectileCollision(IBase b, Collision collision)
    {
        DamageLogic.I.TakeDamage(collision, b as IDamageSource);
        Destroy(b.GetGameObject(), 0.1f);
    }
}
public interface IShooter : IBase
{
    Transform GetProjectileOrigin();
    IProjectile GetProjectile();
    float GetBurstRate();
    int GetBurstAmount();
    float GetBurstInterval();
}
public interface IProjectile : IDamageSource, IDollyMover
{
}
