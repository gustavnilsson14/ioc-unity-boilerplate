using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public enum PickupType { 
    RESOURCE, USABLE_ITEM
}
public class PickupLogic : InterfaceLogicBase
{
    public static PickupLogic I;
    public List<IPromptPickup> promptPickups = new List<IPromptPickup>();

    public float promptPickupRange = 3;

    protected override void OnInstantiate(GameObject newInstance, IBase newBase)
    {
        base.OnInstantiate(newInstance, newBase);
        InitPickup(newBase as IPickup);
        InitAutoPickup(newBase as IAutoPickup);
        InitPromptPickup(newBase as IPromptPickup);
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
    private void InitPromptPickup(IPromptPickup promptPickup)
    {
        if (promptPickup == null)
            return;
        promptPickups.Add(promptPickup);
        promptPickup.isHeld = promptPickup.GetGameObject().GetComponentInParent<IItemUser>() != null;
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
            promptPickups
        });
    }
    private void Update()
    {
        promptPickups.ForEach(x => CheckForItemUsers(x));
    }

    private void CheckForItemUsers(IPromptPickup promptPickup)
    {
        if (promptPickup.isHeld)
            return;
        Physics.OverlapSphere(promptPickup.GetGameObject().transform.position, promptPickupRange).ToList().ForEach(x => ShowPromptFor(promptPickup, x));
    }

    private void ShowPromptFor(IPromptPickup promptPickup, Collider collider)
    {
        if (!TryGetItemUser(collider, out IItemUser itemUser))
            return;
        WorldTextLogic.I.Show(promptPickup);
    }

    private bool TryGetItemUser(Collider collider, out IItemUser itemUser)
    {
        itemUser = null;
        if (collider == null)
            return false;
        if (collider.attachedRigidbody == null)
            return false;
        if (!collider.attachedRigidbody.TryGetComponent(out itemUser))
            return false;
        return true;
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

    private void Pickup(IPromptPickup promptPickup, IItemUser itemUser)
    {
        if (promptPickup.isHeld)
            return;
        Pickup(promptPickup, itemUser);
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
public interface IPromptPickup : IPickup, IWorldText
{
    PromptPickupEvent onPromptShow { get; set; }
    PromptPickupEvent onPromptHide { get; set; }
    bool isHeld { get; set; }
}
public interface IAutoPickup : IPickup
{

}
public class PickupEvent : UnityEvent<IPickup> { }
public class PromptPickupEvent : UnityEvent<IPromptPickup> { }
public class AutoPickupEvent : UnityEvent<IAutoPickup> { }