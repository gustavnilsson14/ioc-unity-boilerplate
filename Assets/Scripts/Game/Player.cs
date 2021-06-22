using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class Player : BehaviourBase, IDamageable, IInventory, IAnimated, ISentient, ISkilled, IJumper, IItemUser, IInputReciever, IDodger
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
    public JumpEvent onAirJump { get; set; }
    public IUsableItem currentEquippedItem { get; set; }
    public InvulnerableEvent onInvulnerableStart { get; set; }
    public InvulnerableEvent onInvulnerableEnd { get; set; }
    public InvulnerableEvent onInvulnerableHit { get; set; }
    public float currentInvulnerableTime { get; set; }
    public float currentInvulnerableCooldown { get; set; }
    public float currentDashCooldown { get; set; }
    public float currentDashDuration { get; set; }
    public DashEvent onDashStart { get; set; }
    public DashEvent onDashEnd { get; set; }
    public Vector3 dashVector { get; set; }
    public DodgeEvent onDodgeStart { get; set; }

    public Disposition GetInitialDisposition() => new Disposition(0.5f, 0.5f, 0.5f);

    public int GetDamage() => 10;
    public DamageType GetDamageType() => DamageType.PHYSICAL;
    public float GetDecayTime() => 2;
    public int GetMaxHealth() => maxHealth;

    public int GetMaxStacks() => maxInventoryStacks;

    public List<IResource> GetResources() => GetComponentsInChildren<IResource>().ToList();

    public float GetSpeed() => speed;

    public int GetMaxFocus() => maxFocus;

    public float GetJumpForce() => jumpForce;
    public int GetMaxAirJumps() => maxAirJumps;

    public float GetGravityMultiplier() => gravityMultiplier;

    public float GetBurstRate() => burstRate;

    public int GetBurstAmount() => burstAmount;

    public float GetBurstInterval() => burstInterval;

    public List<GameObject> GetSpawnedPrefabs() => projectilePrefabs;

    public void SetSpawnedPrefabs(List<GameObject> prefabs) => this.projectilePrefabs = prefabs;

    public Transform GetSpawnPoint() => projectileOrigin;

    public SpawnMode GetSpawnMode() => projectileSpawnMode;

    public bool testAdd = false;
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
    public SpawnMode projectileSpawnMode;
    public List<InputMapping> inputMappings;
    public List<AxisMapping> axisMappings;
    public float meleeCooldown;
    public float speed = 10;
    public bool rotateTowardsMouse;
    public float projectileSpread;
    public List<Skill> skills = new List<Skill>();
    public List<IUsableItem> usableItems;
    public float invulnerableTime;
    public float invulnerableCooldown;
    public float dashSpeedMultiplier;
    public float dashCooldown;
    public float dashDuration;

    private void Update()
    {
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
        if (test) {
            test = false;
            DamageLogic.I.TakeDamage(this as IDamageable, this as IDamageSource);
        }
    }

    public List<InputMapping> GetInputMappings() => inputMappings;

    public List<AxisMapping> GetAxisMappings() => axisMappings;

    public bool GetRotateTowardsMouse() => rotateTowardsMouse;

    public float GetProjectileSpread() => projectileSpread;

    public List<Skill> GetSkills() => skills;

    public List<IUsableItem> GetUsableItems() => usableItems;

    public List<IUsableItem> SetUsableItems(List<IUsableItem> usableItems) => this.usableItems = usableItems;

    public float GetInvulnerableTime() => invulnerableTime;

    public float GetInvulnerableCooldown() => invulnerableCooldown;

    public float GetDashSpeedMultiplier() => dashSpeedMultiplier;

    public float GetDashCooldown() => dashCooldown;
    public float GetDashDuration() => dashDuration;
}