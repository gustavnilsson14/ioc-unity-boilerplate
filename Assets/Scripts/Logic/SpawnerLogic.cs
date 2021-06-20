using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum SpawnMode { 
    ALWAYS_FIRST_PREFAB, ROUND_ROBIN, SHUFFLE, WEIGHTED_SHUFFLE
}

public class SpawnerLogic : InterfaceLogicBase
{
    public static SpawnerLogic I;
    public List<ISpawner> spawners = new List<ISpawner>();
    protected override void OnInstantiate(GameObject newInstance)
    {
        base.OnInstantiate(newInstance);
        InitSpawner(newInstance);
    }

    private void InitSpawner(GameObject newInstance)
    {
        if (!newInstance.TryGetComponent(out ISpawner spawner))
            return;
        spawners.Add(spawner);
        spawner.onSpawn = new SpawnEvent();
        spawner.currentRoundRobinIndex = 0;
    }

    public void Spawn(ISpawner spawner)
    {
        if (!GetPrefab(spawner, out GameObject prefab))
            return;
        GameObject newInstance = PrefabFactory.I.Create(prefab, transform, spawner.GetSpawnPoint());
        spawner.onSpawn.Invoke(spawner, newInstance);
    }

    private bool GetPrefab(ISpawner spawner, out GameObject prefab)
    {
        prefab = null;
        if (spawner.GetSpawnedPrefabs().Count == 0)
            return false;
        switch (spawner.GetSpawnMode())
        {
            case SpawnMode.ALWAYS_FIRST_PREFAB:
                prefab = spawner.GetSpawnedPrefabs()[0];
                break;
            case SpawnMode.ROUND_ROBIN:
                prefab = spawner.GetSpawnedPrefabs()[spawner.currentRoundRobinIndex];
                spawner.currentRoundRobinIndex++;
                if (spawner.currentRoundRobinIndex >= spawner.GetSpawnedPrefabs().Count)
                    spawner.currentRoundRobinIndex = 0;
                break;
            case SpawnMode.SHUFFLE:
                spawner.SetSpawnedPrefabs(RandomUtil.Shuffle(spawner.GetSpawnedPrefabs()));
                prefab = spawner.GetSpawnedPrefabs()[0];
                break;
        }
        return prefab != null;
    }
}
public interface ISpawner { 
    SpawnEvent onSpawn { get; set; }
    List<GameObject> GetSpawnedPrefabs();
    void SetSpawnedPrefabs(List<GameObject> prefabs);
    Transform GetSpawnPoint();
    SpawnMode GetSpawnMode();
    int currentRoundRobinIndex { get; set; }
}
public class SpawnEvent : UnityEvent<ISpawner, GameObject> { }
