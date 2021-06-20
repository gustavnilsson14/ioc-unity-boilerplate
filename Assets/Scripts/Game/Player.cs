﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class Player : BehaviourBase, IDamageable, IMeleeAttacker, IInventory, IAnimated, ISentient, ISkilled, IJumper, IShooter
{
    public int maxHealth;
    public bool test;
    public int maxInventoryStacks;
    public int currentHealth { get; set; }
    public bool alive { get; set; }
    public DamageEvent onHit { get; set; }
    public DamageEvent onArmorHit { get; set; }
    public DamageEvent onArmorBroken { get; set; }
    public DamageEvent onResist { get; set; }
    public DamageEvent onDeath { get; set; }
    public Vector3 previousMovementVector { get; set; }
    public MoveEvent onMove { get; set; }
    public MoveEvent onStart { get; set; }
    public MoveEvent onStop { get; set; }
    public InventoryEvent onChange { get; set; }
    public Animator animator { get; set; }
    public Vector3 destination { get; set; }
    public Vector3 movementVector { get; set; }
    public bool hasDestination { get; set; }
    public Disposition generalDisposition { get; set; }
    public Disposition currentDisposition { get; set; }
    public Dictionary<int, float> bonds { get; set; }
    public List<Skill> skills { get; set; }
    public SkillEvent onExperienceGain { get; set; }
    public SkillEvent onLevelUp { get; set; }
    public SkillCheckEvent onSkillCheck { get; set; }
    public List<Task> tasks { get; set; }
    public List<Task> activeTasks { get; set; }
    public JumpEvent onJump { get; set; }
    public int airJumps { get; set; }
    
    public bool isDirty { get; set; }
    public bool isGrounded { get; set; }
    public bool previousIsGrounded { get; set; }
    public MoveEvent onLand { get; set; }
    public SpawnEvent onSpawn { get; set; }
    public int currentRoundRobinIndex { get; set; }
    public bool isBursting { get; set; }
    public float burstRateCooldown { get; set; }
    public float burstIntervalCooldown { get; set; }
    public int burstShotsLeft { get; set; }

    public Disposition GetInitialDisposition() => new Disposition(0.5f, 0.5f, 0.5f);

    public int GetDamage() => 10;
    public DamageType GetDamageType() => DamageType.PHYSICAL;
    public float GetDecayTime() => 2;
    public int GetMaxHealth() => maxHealth;

    public int GetMaxStacks() => maxInventoryStacks;

    public List<IResource> GetResources() => GetComponentsInChildren<IResource>().ToList();

    public float GetSpeed() => 3;

    public int GetMaxFocus() => maxFocus;

    public float GetJumpForce() => jumpForce;
    public int GetMaxAirJumps() => maxAirJumps;

    public float GetGravityMultiplier() => gravityMultiplier;

    public float GetDamageDelay() => 1;

    public SphereCollider GetDamageCollider() => damageCollider;

    public float GetBurstRate() => burstRate;

    public int GetBurstAmount() => burstAmount;

    public float GetBurstInterval() => burstInterval;

    public List<GameObject> GetSpawnedPrefabs() => projectilePrefabs;

    public void SetSpawnedPrefabs(List<GameObject> prefabs) => this.projectilePrefabs = prefabs;

    public Transform GetSpawnPoint() => projectileOrigin;

    public SpawnMode GetSpawnMode() => SpawnMode.ALWAYS_FIRST_PREFAB;

    public bool testAdd = false;
    public bool testMelee = false;
    public bool testShoot = false;
    public bool testSpend = false;
    public bool testMoveTo = false;
    public bool testDisposition = false;
    public bool testTask = false;

    public Task task;

    public Vector3 moveTo;
    public int amount = 10;
    public ResourceType resourceType;

    [Range(0,1)]
    public float dispositionMood;
    [Range(0, 1)]
    public float dispositionPassion;
    [Range(0, 1)]
    public float dispositionStress;
    public int maxFocus;
    public float jumpForce;
    public bool testJump;
    public int maxAirJumps;
    public float gravityMultiplier = 1;
    public SphereCollider damageCollider;
    public Transform projectileOrigin;
    public List<GameObject> projectilePrefabs;
    public float burstRate;
    public int burstAmount;
    public float burstInterval;










    private void Update()
    {
        if (testMelee)
        {
            testMelee = false;
            MeleeLogic.I.Attack(this);
        }
        if (testShoot)
        {
            testShoot = false;
            ProjectileLogic.I.Shoot(this);
        }
        if (testSpend)
        {
            testSpend = false;
            Debug.Log($"spending: {ResourceLogic.I.SpendResources(this, resourceType, amount)}");
            Debug.Log(GetResources().Count);
        }
        if (testAdd)
        {
            testAdd = false;
            ResourceLogic.I.AddResourceToInventory(resourceType, amount, this);
        }
        if (testMoveTo)
        {
            testMoveTo = false;
            MovementLogic.I.MoveTo(this, moveTo);
        }
        if (testDisposition)
        {
            testDisposition = false;
            SentientCreatureLogic.I.SetCurrentDisposition(this, new Disposition(dispositionMood, dispositionPassion, dispositionStress));
            Debug.Log("Calm " + SentientCreatureLogic.I.GetCalm(this));
            Debug.Log("Hostility " + SentientCreatureLogic.I.GetHostility(this));
            Debug.Log("Happyness " + SentientCreatureLogic.I.GetHappyness(this));
        }
        if (testTask)
        {
            testTask = false;
            SentientCreatureLogic.I.AddTask(this, new Task(task));
        }
        if (testJump)
        {
            testJump = false;
            JumpLogic.I.Jump(this);
        }
        if (!test)
            return;
        test = false;
        DamageLogic.I.TakeDamage(this, this);
    }

}
