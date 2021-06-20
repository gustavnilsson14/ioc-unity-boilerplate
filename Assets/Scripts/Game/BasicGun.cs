using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicGun : BehaviourBase, IShooter
{
    public List<GameObject> spawnedPrefabs;
    public SpawnMode spawnMode;
    public Transform projectileOrigin;

    public bool isBursting { get; set; }
    public float burstIntervalCooldown { get; set; }
    public int burstShotsLeft { get; set; }
    public SpawnEvent onSpawn { get; set; }
    public int currentRoundRobinIndex { get; set; }
    public int ammo { get; set; }
    public UsableItemEvent onItemUse { get; set; }
    public float currentItemCooldown { get; set; }
    public UsableItemEvent onItemOutOfAmmo { get; set; }
    public UsableItemEvent onReload { get; set; }

    public int GetAmmoCapacity() => 10;

    public ResourceType GetAmmoType() => ResourceType.BULLETS;

    public int GetBurstAmount() => 3;

    public float GetBurstInterval() => 0.05f;

    public float GetItemCooldown() => 0.5f + (GetBurstAmount() * GetBurstInterval());

    public ItemType GetItemType() => ItemType.RANGED_WEAPON;
    public float GetProjectileSpread() => 20;

    public List<GameObject> GetSpawnedPrefabs() => spawnedPrefabs;

    public SpawnMode GetSpawnMode() => spawnMode;

    public Transform GetSpawnPoint() => projectileOrigin;

    public bool GetUsesAmmo() => true;

    public void SetSpawnedPrefabs(List<GameObject> prefabs) => spawnedPrefabs = prefabs;
}
