using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicResource : BehaviourBase, IResource
{
    public ResourceType resType;
    public int amount;
    public ResourceEvent onChange { get; set; }
    public bool isClaimed { get; set; }
    public bool DestroyOnEmpty() => true;
    public ResourceLogic.Resource resourceType { get; set; }

    public bool test;
    public GameObject inventory;
    private void Update()
    {
        if (!test)
            return;
        test = false;
        ResourceLogic.I.MoveResourceToInventory(this,inventory.GetComponent<IInventory>());
    }

    public int GetAmount() => amount;

    public int SetAmount(int amount) => this.amount = amount;

    public ResourceType GetResourceType() => resType;
}
