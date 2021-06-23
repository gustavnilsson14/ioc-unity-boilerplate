using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum ProductDefaultOutput { 
    UNCLAIMED, CONVERTER_INVENTORY
}
public enum ResourceType { 
    GOLD, FISH, DIRT, LUMBER, IRON, SAND, MEEPS, DODECAHEDRONS, BULLETS
}
public class ResourceLogic : InterfaceLogicBase
{
    public static ResourceLogic I;
    public Transform unclaimedContainer;

    public List<IResourceConverter> resourceConverters = new List<IResourceConverter>();

    public List<Resource> resourceTypes;

    [Header("Prefabs")]
    public GameObject basicResourcePrefab;

    private void Update()
    {
        UpdateConverters();
    }
    protected override void RegisterInstances()
    {
        base.RegisterInstances();
        CreateResourceTypes();
    }

    private void CreateResourceTypes()
    {
        resourceTypes = new List<Resource>() {
            new Resource(ResourceType.GOLD,basicResourcePrefab,999,1),
            new Resource(ResourceType.SAND,basicResourcePrefab,9999,1),
            new Resource(ResourceType.BULLETS,basicResourcePrefab,9999,1),
        };
    }

    protected override void OnInstantiate(GameObject newInstance, IBase newBase)
    {
        base.OnInstantiate(newInstance, newBase);
        InitResource(newBase as IResource);
        InitInventory(newBase as IInventory);
        InitResourceConverter(newBase as IResourceConverter);
    }
    private void InitResource(IResource resource)
    {
        if (resource == null)
            return;
        resource.onChange = new ResourceEvent();
        resource.resourceType = resourceTypes.Find(x => x.resourceType == resource.GetResourceType());
    }
    private void InitResourceConverter(IResourceConverter resourceConverter)
    {
        if (resourceConverter == null)
            return;
        if (!resourceConverter.GetGameObject().TryGetComponent(out IInventory inventory))
        {
            Destroy(resourceConverter.GetGameObject());
            return;
        }
        resourceConverter.onConversionStart = new ResourceConverterEvent();
        resourceConverter.onConversionEnd = new ResourceConverterEvent();
        resourceConverter.inventory = inventory;
        resourceConverter.inventory.onChange.AddListener(ConverterInventoryChange);
        resourceConverters = resourceConverters.FindAll(x => x != null);
        resourceConverters.Add(resourceConverter);
    }

    private void InitInventory(IInventory inventory)
    {
        if (inventory == null)
            return;
        inventory.onChange = new InventoryEvent();
        inventory.onDestroy.AddListener(DropInventoryExcess);
    }


    protected override void UnRegister(IBase b)
    {
        base.UnRegister(b);
        if (b is IResourceConverter)
            resourceConverters.Remove(b as IResourceConverter);
    }


    /// <summary>
    /// Resource conversion code
    /// </summary>
    private void UpdateConverters()
    {
        resourceConverters.ForEach(x => UpdateConverter(x));
    }

    private void UpdateConverter(IResourceConverter resourceConverter)
    {
        if (resourceConverter == null)
            return;
        if (!resourceConverter.isWorking)
            return;
        resourceConverter.currentConversionTime += Time.deltaTime;
        if (resourceConverter.currentConversionTime < resourceConverter.GetConversionTime())
            return;
        ConvertResources(resourceConverter);
    }

    private void ConvertResources(IResourceConverter resourceConverter)
    {
        resourceConverter.currentConversionTime = 0;
        resourceConverter.GetRecipe().ForEach(x => SpendResources(resourceConverter.inventory, x.resourceType, x.amount));
        switch (resourceConverter.GetProductDefaultOutput())
        {
            case ProductDefaultOutput.UNCLAIMED:
                Debug.Log("case ProductDefaultOutput.UNCLAIMED:");
                resourceConverter.GetProducts().ForEach(x => CreateResourcesUnclaimed(x.resourceType, resourceConverter.GetGameObject().transform.position, x.amount));
                break;
            case ProductDefaultOutput.CONVERTER_INVENTORY:
                resourceConverter.GetProducts().ForEach(x => AddResourceToInventory(x.resourceType, x.amount, resourceConverter.inventory));
                break;
            default:
                break;
        }

    }
    private void ConverterInventoryChange(IInventory inventory)
    {
        if (!inventory.GetGameObject().TryGetComponent<IResourceConverter>(out IResourceConverter resourceConverter))
            return;
        if (ConverterInventoryHasRecipe(resourceConverter))
            return;
        StopResourceConversion(resourceConverter);
    }
    public bool StartResourceConversion(IResourceConverter resourceConverter)
    {
        if (!ConverterInventoryHasRecipe(resourceConverter))
            return false;
        resourceConverter.isWorking = true;
        resourceConverter.currentConversionTime = 0;
        return true;
    }
    public void StopResourceConversion(IResourceConverter resourceConverter)
    {
        resourceConverter.isWorking = false;
        resourceConverter.currentConversionTime = 0;
    }
    private bool ConverterInventoryHasRecipe(IResourceConverter resourceConverter)
    {
        foreach (ResourceConversionPart conversionPart in resourceConverter.GetRecipe())
        {
            if (!InventoryContains(resourceConverter.inventory, conversionPart.resourceType, conversionPart.amount))
                return false;
        }
        return true;
    }
    private bool InventoryContains(IInventory inventory, ResourceType resourceType, int amount)
    {
        return amount < inventory.GetResources().FindAll(x => x.resourceType.resourceType == resourceType).Sum(x => x.GetAmount());
    }





    /// <summary>
    /// Resource creation code
    /// </summary>
    public bool CreateResourcesInInventory(out List<IResource> newResources, Resource resource, IInventory inventory, int amount) {
        newResources = new List<IResource>();
        int iterations = 99;
        while (amount > 0 && iterations > 0)
        {
            iterations--;
            newResources.Add(CreateResourceInInventory(resource, inventory, ref amount));
        }
        return true;
    }

    public bool CreateResourcesUnclaimed(ResourceType resourceType, Vector3 position, int amount)
    {
        return CreateResourcesUnclaimed(out List<IResource> newResources, resourceTypes.Find(x => x.resourceType == resourceType), position, amount);
    }
    public bool CreateResourcesUnclaimed(out List<IResource> newResources, Resource resource, Vector3 position, int amount)
    {
        newResources = new List<IResource>();
        int iterations = 99;
        while (amount > 0 && iterations > 0)
        {
            iterations--;
            newResources.Add(CreateResourceUnclaimed(resource, ref amount, position));
        }
        return true;
    }
    public IResource CreateResourceInInventory(Resource resource, IInventory inventory, ref int amount)
    {
        CreateResource(out IResource newStack, resource, unclaimedContainer);
        AddToStack(newStack, ref amount);
        SetResourceInInventory(newStack,inventory);
        return newStack;
    }

    public IResource CreateResourceUnclaimed(Resource resource, ref int amount, Vector3 position)
    {
        CreateResource(out IResource newStack, resource, unclaimedContainer);
        AddToStack(newStack, ref amount);
        SetResourceUnclaimed(newStack);
        newStack.GetGameObject().transform.position = position;
        return newStack;
    }
    public IResource CreateResource(out IResource newStack, Resource resource, Transform parent)
    {
        PrefabFactory.I.Create(resource.resourcePrefab, parent).TryGetComponent(out newStack);
        newStack.resourceType = resource;
        newStack.GetGameObject().name = resource.resourceType.ToString();
        return newStack;
    }

    public void AddResourceToInventory(ResourceType resourceType, int amount, IInventory inventory)
    {
        List<IResource> existingStacks = inventory.GetResources().FindAll(x => x.resourceType.resourceType == resourceType && x.GetAmount() < x.resourceType.maxStackAmount);
        existingStacks.ForEach(x => AddToStack(x, ref amount));
        if (amount == 0)
            return;
        CreateResourcesInInventory(out List<IResource> newResources, resourceTypes.Find(x => x.resourceType == resourceType), inventory, amount);
        DropInventoryExcess(inventory);
    }
    public void AddUnclaimedResource(ResourceType resourceType, int amount, Vector3 position) 
    {
        CreateResourcesUnclaimed(out List<IResource> newResources, resourceTypes.Find(x => x.resourceType == resourceType), position, amount);
    }
    public void MoveResourceToInventory(IResource resource, IInventory inventory)
    {
        List<IResource> existingStacks = inventory.GetResources().FindAll(x => x.resourceType.resourceType == resource.resourceType.resourceType && x.GetAmount() < x.resourceType.maxStackAmount);
        existingStacks.ForEach(x => AddToStack(x, resource));
        if (resource.GetAmount() == 0)
            return;
        SetResourceInInventory(resource, inventory);
    }




    public void SetResourceUnclaimed(IInventory inventory, IResource resource)
    {
        inventory.onChange.Invoke(inventory);
        resource.GetGameObject().transform.position = inventory.GetGameObject().transform.position;
        SetResourceUnclaimed(resource);
    }

    public void SetResourceUnclaimed(IResource resource)
    {
        resource.isClaimed = false;
        resource.GetGameObject().transform.parent = unclaimedContainer;
        List<IResource> existingResources = GetResourcesInRange(resource, resource.GetGameObject().transform.position);
        int iterations = 99;
        while (resource.GetAmount() > 0 && existingResources.Count > 0 && iterations > 0)
        {
            iterations--;
            AddToStack(existingResources[0], resource);
            existingResources.RemoveAt(0);
        }
    }
    public void SetResourceInInventory(IResource resource, IInventory inventory)
    {
        resource.isClaimed = true;
        inventory.onChange.Invoke(inventory);
        resource.GetGameObject().transform.parent = inventory.GetGameObject().transform;
    }



    public List<IResource> GetResourcesInRange(IResource resource, Vector3 position, float range = 1)
    {
        List<Collider> hits = Physics.OverlapSphere(position, range).ToList().FindAll(x => x.GetComponent<IResource>() != null);
        List<IResource> resources = hits.Select(x => x.GetComponent<IResource>()).ToList();
        return resources.Where(x => x.GetGameObject().transform.parent == unclaimedContainer && x.resourceType.resourceType == resource.resourceType.resourceType).ToList();
    }

    public int GetResourceTotal(IInventory inventory, ResourceType resourceType)
    {
        return inventory.GetResources().FindAll(x => x.resourceType.resourceType == resourceType).Sum(x => x.GetAmount());
    }

    private void DropInventoryExcess(IBase inventory)
    {
        if (!(inventory is IInventory))
            return;
        (inventory as IInventory).GetResources().ForEach(x => SetResourceUnclaimed((inventory as IInventory), x));
    }
    private void DropInventoryExcess(IInventory inventory)
    {
        if (inventory.GetResources().Count <= inventory.GetMaxStacks())
            return;
        while (inventory.GetResources().Count > inventory.GetMaxStacks())
        {
            SetResourceUnclaimed(inventory.GetResources()[inventory.GetResources().Count-1]);
        }
    }
    public void AddToStack(IResource resource, ref int amount)
    {
        if (amount == 0)
            return;
        int spaceLeft = resource.resourceType.maxStackAmount - resource.GetAmount();
        int amountAdded = Mathf.Clamp(amount, 0, spaceLeft);
        amount -= amountAdded;
        ChangeAmount(resource, amountAdded);
    }

    public void AddToStack(IResource resource, IResource fromResource)
    {
        if (resource == fromResource)
            return;
        int amount = fromResource.GetAmount();
        AddToStack(resource, ref amount);
        fromResource.SetAmount(amount);
        CheckDestroy(fromResource);
    }
    public void ChangeAmount(IResource resource, int amount) {
        Resource resourceType = resource.resourceType;
        resource.SetAmount(Mathf.Clamp(resource.GetAmount() + amount, 0, resourceType.maxStackAmount));
        if (resource.onChange == null)
            return;
        resource.onChange.Invoke(resource, amount);
        CheckDestroy(resource);
    }
    private void CheckDestroy(IResource resource)
    {
        if (resource.GetAmount() > 0)
            return;
        if (!resource.DestroyOnEmpty())
            return;
        resource.onDestroy.Invoke(resource);
        Destroy(resource.GetGameObject(), 0.01f);
    }
    public bool SpendResources(IInventory inventory, ResourceType resourceType, int amount) {
        List<IResource> availableResources = inventory.GetResources().FindAll(x => x.resourceType.resourceType == resourceType);
        if (amount > availableResources.Sum(x => x.GetAmount()))
            return false;
        foreach (IResource resource in availableResources)
        {
            int payment = Mathf.Clamp(amount, 0, resource.GetAmount());
            amount -= payment;
            ChangeAmount(resource, -payment);
        }
        inventory.onChange.Invoke(inventory);
        return true;
    }




    [System.Serializable]
    public class Resource
    {
        public ResourceType resourceType;
        public int maxStackAmount;
        public GameObject resourcePrefab;
        public float autoStackDistance;
        public Resource(ResourceType resourceType, GameObject resourcePrefab, int maxStackAmount, float autoStackDistance) {
            this.resourceType = resourceType;
            this.resourcePrefab = resourcePrefab;
            this.maxStackAmount = maxStackAmount;
            this.autoStackDistance = autoStackDistance;
        }
    }
    [System.Serializable]
    public class ResourceConversionPart
    {
        public ResourceType resourceType;
        public int amount;
        public ResourceConversionPart(ResourceType resourceType, int amount)
        {
            this.resourceType = resourceType;
            this.amount = amount;
        }
    }
}
public interface IInventory : IBase
{
    int GetMaxStacks();
    InventoryEvent onChange { get; set; }
    List<IResource> GetResources();
}
public interface IResource : IBase
{
    ResourceLogic.Resource resourceType { get; set; }
    int GetAmount();
    int SetAmount(int amount);
    bool DestroyOnEmpty();
    ResourceType GetResourceType();

    ResourceEvent onChange { get; set; }
    bool isClaimed { get; set; }
}
public interface IResourceConverter : IBase
{
    List<ResourceLogic.ResourceConversionPart> GetRecipe();
    List<ResourceLogic.ResourceConversionPart> GetProducts();
    IInventory inventory { get; set; }
    bool isWorking { get; set; }
    ProductDefaultOutput GetProductDefaultOutput();
    float GetConversionTime();
    float currentConversionTime { get; set; }
    ResourceConverterEvent onConversionStart { get; set; }
    ResourceConverterEvent onConversionEnd { get; set; }
}

public class ResourceEvent : UnityEvent<IResource, int> { }
public class ResourceConverterEvent : UnityEvent<IResourceConverter> { }
public class InventoryEvent : UnityEvent<IInventory> { }
