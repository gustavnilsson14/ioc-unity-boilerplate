using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum PickupType { 
    RESOURCE, USABLE_ITEM
}
public class PickupLogic : InterfaceLogicBase
{
    public static PickupLogic I;

    protected override void OnInstantiate(GameObject newInstance, IBase newBase)
    {
        base.OnInstantiate(newInstance, newBase);
        InitPickup(newBase as IPickup);
        InitAutoPickup(newBase as IAutoPickup);
    }
    private void InitPickup(IPickup pickup)
    {
        if (pickup == null)
            return;
    }
    private void InitAutoPickup(IAutoPickup autoPickup)
    {
        if (autoPickup == null)
            return;
    }
    protected override void OnRegisterInternalListeners(GameObject newInstance, IBase newBase)
    {
        base.OnRegisterInternalListeners(newInstance, newBase);
        PickupInternalListeners(newBase as IPickup);
        AutoPickupInternalListeners(newBase as IAutoPickup);
    }

    private void PickupInternalListeners(IPickup pickup)
    {
        if (pickup == null)
            return;
    }
    private void AutoPickupInternalListeners(IAutoPickup autoPickup)
    {
        if (autoPickup == null)
            return;
        autoPickup.onCollision.AddListener(OnAutoPickupCollide);
    }

    protected override void UnRegister(IBase b)
    {
        base.UnRegister(b, new List<IList>() {
            
        });
    }
    private void Update()
    {

    }

    private void OnAutoPickupCollide(IBase b, Collision other)
    {
        if (other.collider.attachedRigidbody == null)
            return;
        IAutoPickup autoPickup = b as IAutoPickup;
        switch (autoPickup.GetPickupType())
        {
            case PickupType.RESOURCE:
                Pickup(autoPickup, other.collider.attachedRigidbody.GetComponent<IInventory>());
                break;
            case PickupType.USABLE_ITEM:
                Pickup(autoPickup, other.collider.attachedRigidbody.GetComponent<IItemUser>());
                break;
        }

    }
    public void Pickup(IPickup pickup, IInventory inventory)
    {
        IResource resource = pickup as IResource;
        if (resource == null || inventory == null)
            return;
        ResourceLogic.I.AddResourceToInventory(resource.resourceType.resourceType, resource.GetAmount(), inventory);
        Pickup(pickup);
    }

    private void Pickup(IPickup pickup, IItemUser itemUser)
    {
        IUsableItem usableItem = pickup as IUsableItem;
        if (usableItem == null || itemUser == null)
            return;
        itemUser.GetUsableItems().Add(usableItem);
        Pickup(pickup);
    }

    private void Pickup(IPickup pickup)
    {
        Destroy(pickup.GetGameObject());
    }
}
public interface IPickup : IBase
{
    PickupEvent onPickupPickup { get; set; }
    PickupEvent onPickupDrop { get; set; }
    PickupType GetPickupType();
}
public interface IPromptPickup : IPickup
{
    PromptPickupEvent onPromptShow { get; set; }
    PromptPickupEvent onPromptHide { get; set; }
}
public interface IAutoPickup : IPickup
{

}
public class PickupEvent : UnityEvent<IPickup> { }
public class PromptPickupEvent : UnityEvent<IPromptPickup> { }
public class AutoPickupEvent : UnityEvent<IAutoPickup> { }