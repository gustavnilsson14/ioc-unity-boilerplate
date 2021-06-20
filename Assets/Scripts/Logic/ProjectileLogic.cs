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
        shooters.Add(shooter);
        shooter.burstRateCooldown = 0;
        shooter.burstIntervalCooldown = 0;
    }

    private void InitProjectile(GameObject newInstance)
    {
        if (!newInstance.TryGetComponent(out IProjectile projectile))
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

    public void Shoot(IShooter shooter)
    {
        if (shooter.burstRateCooldown > 0)
            return;
        shooter.isBursting = true;
        shooter.burstShotsLeft = shooter.GetBurstAmount();
        shooter.burstRateCooldown = shooter.GetBurstRate() + (shooter.GetBurstInterval() * shooter.GetBurstAmount());
    }
    private void UpdateShooter(IShooter shooter)
    {
        shooter.burstRateCooldown -= Time.deltaTime;
        shooter.burstIntervalCooldown -= Time.deltaTime;
        if (!shooter.isBursting)
            return;
        if (shooter.burstIntervalCooldown > 0)
            return;
        shooter.burstShotsLeft -= 1;
        SpawnerLogic.I.Spawn(shooter);
        shooter.burstIntervalCooldown = shooter.GetBurstInterval();
        if (shooter.burstShotsLeft > 0)
            return;
        shooter.isBursting = false;
    }

}
public interface IShooter : ISpawner
{
    bool isBursting { get; set; }
    float burstRateCooldown{ get; set; }
    float burstIntervalCooldown { get; set; }
    int burstShotsLeft { get; set; }
    float GetBurstRate();
    int GetBurstAmount();
    float GetBurstInterval();
}
public interface IProjectile : IDamageSource, IDollyMover
{
    Vector3 GetMovementNoise();
    Collider projectileCollider { get; set; }
    TrailRenderer GetTrail();
}
