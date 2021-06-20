using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using System.Linq;

public class ProjectileLogic : InterfaceLogicBase
{
    public static ProjectileLogic I;
    public List<IShooter> shooters = new List<IShooter>();
    private void Update()
    {
        shooters.ForEach(x => UpdateShooter(x));
    }

    protected override void OnInstantiate(GameObject newInstance, IBase newBase)
    {
        base.OnInstantiate(newInstance, newBase);
        InitShooter(newBase as IShooter);
        InitProjectile(newBase as IProjectile);
    }

    protected override void UnRegister(IBase b)
    {
        base.UnRegister(b);
        if (b is IShooter)
            shooters.Remove(b as IShooter);
    }
    private void InitShooter(IShooter shooter)
    {
        if (shooter == null)
            return;
        shooters.Add(shooter);
        shooter.burstIntervalCooldown = 0;
        shooter.onItemUse.AddListener(Shoot);
        shooter.onItemOutOfAmmo.AddListener(Reload);
    }

    private void Reload(IUsableItem item)
    {
        if (!EquippedItemLogic.I.GetHandler(item, out IItemUser itemUser))
            return;
        EquippedItemLogic.I.Reload(itemUser);
    }

    private void InitProjectile(IProjectile projectile)
    {
        if (projectile == null)
            return;
        projectile.onCollision.AddListener(OnProjectileCollision);
        projectile.projectileCollider = projectile.GetGameObject().GetComponentInChildren<Collider>();
        projectile.destroyDelay = GetProjectileDestroyDelay(projectile);
        ApplyMovementNoise(projectile);
    }

    private void ApplyMovementNoise(IProjectile projectile)
    {
        if (projectile.GetTrack() is CinemachineSmoothPath)
            ApplyNoiseToSmoothPath(projectile);
        if (projectile.GetTrack() is CinemachinePath)
            ApplyNoiseToPath(projectile);
    }

    private void ApplyNoiseToSmoothPath(IProjectile projectile)
    {
        CinemachineSmoothPath.Waypoint[] path = (projectile.GetTrack() as CinemachineSmoothPath).m_Waypoints;
        for (int i = 1; i < path.Length - 1; i++)
        {
            path[i].position = path[i].position + (projectile.GetMovementNoise() * UnityEngine.Random.Range(-1f, 1f));
        }
    }

    private void ApplyNoiseToPath(IProjectile projectile)
    {
        CinemachinePath.Waypoint[] path = (projectile.GetTrack() as CinemachinePath).m_Waypoints;
        for (int i = 1; i < path.Length - 1; i++)
        {
            path[i].position = path[i].position + (projectile.GetMovementNoise() * UnityEngine.Random.Range(-1f, 1f));
        }
    }

    private void OnProjectileCollision(IBase b, Collision collision)
    {
        DamageLogic.I.TakeDamage(collision, b as IDamageSource);
        Destroy((b as IProjectile).projectileCollider);
        Destroy(b.GetGameObject().GetComponentInChildren<Renderer>());
        Destroy(b.GetGameObject(), GetProjectileDestroyDelay((b as IProjectile)));
    }

    private float GetProjectileDestroyDelay(IProjectile projectile)
    {
        if (projectile.GetTrail() == null)
            return 0.01f;
        return projectile.GetTrail().time;
    }

    private void Shoot(IUsableItem item)
    {
        IShooter shooter = item as IShooter;
        shooter.isBursting = true;
        shooter.burstShotsLeft = shooter.GetBurstAmount();
        shooter.currentItemCooldown = shooter.GetItemCooldown();
    }
    private void UpdateShooter(IShooter shooter)
    {
        shooter.burstIntervalCooldown -= Time.deltaTime;
        if (!shooter.isBursting)
            return;
        if (shooter.burstIntervalCooldown > 0)
            return;
        shooter.burstShotsLeft -= 1;
        if (SpawnerLogic.I.Spawn(shooter, out GameObject newInstance))
            ApplyProjectileSpread(shooter, newInstance);
        
        shooter.burstIntervalCooldown = shooter.GetBurstInterval();
        if (shooter.burstShotsLeft > 0)
            return;
        shooter.isBursting = false;
    }

    private void ApplyProjectileSpread(IShooter shooter, GameObject newInstance)
    {
        float spread = shooter.GetProjectileSpread();
        
        if (shooter is ISkilled)
            spread = SkillLogic.I.ReduceBySkill(shooter as ISkilled, SkillType.SHOOTING, spread);
        Vector3 spreadOffset = new Vector3(
            UnityEngine.Random.Range(-spread, spread),
            UnityEngine.Random.Range(-spread, spread),
            UnityEngine.Random.Range(-spread, spread)
        );
        newInstance.transform.eulerAngles += spreadOffset;
    }
}
public interface IShooter : ISpawner, IUsableItem
{
    bool isBursting { get; set; }
    float burstIntervalCooldown { get; set; }
    int burstShotsLeft { get; set; }
    int GetBurstAmount();
    float GetBurstInterval();
    float GetProjectileSpread();
}
public interface IProjectile : IDamageSource, IDollyMover
{
    Vector3 GetMovementNoise();
    Collider projectileCollider { get; set; }
    TrailRenderer GetTrail();
}
