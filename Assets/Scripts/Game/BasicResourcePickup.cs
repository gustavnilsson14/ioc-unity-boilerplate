using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicResourcePickup : BehaviourBase, IAutoPickup, IResource
{
    public int amount;
    public ResourceType resType;

    public PickupEvent onPickupPickup { get; set; }
    public PickupEvent onPickupDrop { get; set; }
    public ResourceLogic.Resource resourceType { get; set; }
    public ResourceEvent onChange { get; set; }
    public bool isClaimed { get; set; }

    public bool DestroyOnEmpty() => true;

    public int GetAmount() => amount;

    public int SetAmount(int amount) => this.amount = amount;

    public PickupType GetPickupType() => PickupType.RESOURCE;

    public ResourceType GetResourceType() => resType;
}
