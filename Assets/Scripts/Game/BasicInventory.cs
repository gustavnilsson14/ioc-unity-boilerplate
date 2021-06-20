using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicInventory : BehaviourBase, IInventory
{
    public int maxStacks;
    public InventoryEvent onChange { get; set; }
    public int GetMaxStacks() => maxStacks;
    public List<IResource> GetResources() => GetComponentsInChildren<IResource>().ToList();
    public bool testAdd = false;
    public bool testSpend = false;
    public int amount = 10;
    public ResourceType resourceType;

    private void Update()
    {
        if (testSpend)
        {
            testSpend = false;
            Debug.Log($"spending: {ResourceLogic.I.SpendResources(this, resourceType, amount)}");
            Debug.Log(GetResources().Count);
        }
        if (!testAdd)
            return;
        testAdd = false;
        ResourceLogic.I.AddResourceToInventory(resourceType, amount, this);
    }
}
