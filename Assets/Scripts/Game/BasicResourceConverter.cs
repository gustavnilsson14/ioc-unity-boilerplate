using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicResourceConverter : BehaviourBase, IResourceConverter, IInventory, IDescribed
{
    public float conversionTime;
    public ProductDefaultOutput productDefaultOutput;
    public List<ResourceLogic.ResourceConversionPart> products;
    public List<ResourceLogic.ResourceConversionPart> recipe;
    public int maxStacks;

    public IInventory inventory { get; set; }
    public bool isWorking { get; set; }
    public float currentConversionTime { get; set; }
    public ResourceConverterEvent onConversionStart { get; set; }
    public ResourceConverterEvent onConversionEnd { get; set; }
    public InventoryEvent onChange { get; set; }
    TextEvent IDescribed.onDescriptionChange { get; set; }

    public float GetConversionTime() => conversionTime;

    public ProductDefaultOutput GetProductDefaultOutput() => productDefaultOutput;

    public List<ResourceLogic.ResourceConversionPart> GetProducts() => products;

    public List<ResourceLogic.ResourceConversionPart> GetRecipe() => recipe;
    public int GetMaxStacks() => maxStacks;

    public List<IResource> GetResources() => GetComponentsInChildren<IResource>().ToList();



    /// <summary>
    /// TEST CODE
    /// </summary>
    public bool test;
    public bool testDescription;
    public bool testAdd = false;
    public bool testSpend = false;
    public int amount = 10;
    public ResourceType resourceType;
    [TextArea(10,20)]
    public string description;

    private void Update()
    {
        if (testDescription)
        {
            testDescription = false;
            Debug.Log(DescriptionLogic.I.GetDescription(this));
        }
        if (test)
        {

            ResourceLogic.I.StartResourceConversion(this);
            test = false;
            return;
        }
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

    public string GetDescription() => description;
}
