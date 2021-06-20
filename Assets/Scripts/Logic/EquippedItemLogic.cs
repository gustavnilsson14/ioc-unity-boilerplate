﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public enum ItemType {
    MELEE_WEAPON, RANGED_WEAPON, TOOL
}

public class EquippedItemLogic : InterfaceLogicBase
{
    public static EquippedItemLogic I;
    public List<IUsableItem> usableItems = new List<IUsableItem>();
    protected override void OnInstantiate(GameObject newInstance, IBase newBase)
    {
        base.OnInstantiate(newInstance, newBase);
        InitItemUser(newBase as IItemUser);
        InitUsableItem(newBase as IUsableItem);
    }

    private void InitItemUser(IItemUser itemUser)
    {
        if (itemUser == null)
            return;
        itemUser.SetUsableItems(itemUser.GetGameObject().GetComponentsInChildren<IUsableItem>().ToList());
        itemUser.GetUsableItems().ForEach(x => x.GetGameObject().SetActive(false));
        SwapEquippedItem(itemUser, 0);
    }

    private void InitUsableItem(IUsableItem usableItem)
    {
        if (usableItem == null)
            return;
        usableItems.Add(usableItem);
        usableItem.onItemUse = new UsableItemEvent();
        usableItem.onItemOutOfAmmo = new UsableItemEvent();
        usableItem.onReload = new UsableItemEvent();
        usableItem.ammo = 0;
    }

    private void Update()
    {
        usableItems.ForEach(x => UpdateUsableItem(x));
    }

    private void UpdateUsableItem(IUsableItem usableItem)
    {
        usableItem.currentItemCooldown -= Time.deltaTime;
    }

    protected override void UnRegister(IBase b)
    {
        base.UnRegister(b);
        if (b is IUsableItem)
            usableItems.Remove(b as IUsableItem);
    }
    public void Use(IItemUser itemUser) {
        IUsableItem item = itemUser.currentEquippedItem;
        if (!CanUse(item, out bool outOfAmmo)) {
            if (outOfAmmo)
                item.onItemOutOfAmmo.Invoke(item);
            return;
        }
        item.ammo -= 1;
        item.currentItemCooldown = item.GetItemCooldown();
        item.onItemUse.Invoke(item);
    }
    public void Reload(IItemUser itemUser)
    {
        IUsableItem item = itemUser.currentEquippedItem;
        if (OnCooldown(item))
            return;
        IInventory inventory = itemUser as IInventory;
        if (inventory == null)
            return;
        int totalAmmo = ResourceLogic.I.GetResourceTotal(inventory, item.GetAmmoType());
        item.ammo = Mathf.Clamp(item.GetAmmoCapacity(), 0, totalAmmo);
        item.currentItemCooldown = item.GetItemCooldown();
        item.onReload.Invoke(item);
        ResourceLogic.I.SpendResources(inventory, item.GetAmmoType(), itemUser.currentEquippedItem.ammo);
    }

    private bool CanUse(IUsableItem item, out bool outOfAmmo)
    {
        outOfAmmo = false;
        if (OnCooldown(item))
            return false;
        if (!HasAmmo(item)) {
            outOfAmmo = true;
            return false;
        }
        return true;
    }

    private bool OnCooldown(IUsableItem item) => item.currentItemCooldown > 0;

    private bool HasAmmo(IUsableItem item)
    {
        if (!item.GetUsesAmmo())
            return true;
        return item.ammo > 0;
    }

    public void SwapEquippedItem(IItemUser itemUser, int index)
    {
        if (index >= itemUser.GetUsableItems().Count)
            index = 0;
        if (itemUser.currentEquippedItem != null)
            itemUser.currentEquippedItem.GetGameObject().SetActive(false);
        itemUser.currentEquippedItem = itemUser.GetUsableItems()[index];
        itemUser.currentEquippedItem.GetGameObject().SetActive(true);
    }
    public void SwapEquippedItem(IItemUser itemUser)
    {
        Debug.Log($"itemUser {itemUser}");
        int index = itemUser.GetUsableItems().IndexOf(itemUser.currentEquippedItem) + 1;
        SwapEquippedItem(itemUser, index);
    }
    public bool GetHandler(IUsableItem usableItem, out IItemUser itemUser) {
        itemUser = usableItem.GetGameObject().GetComponentInParent<IItemUser>();
        return itemUser != null;
    }
}
public interface IUsableItem : IBase
{
    int GetAmmoCapacity();
    ItemType GetItemType();
    ResourceType GetAmmoType();
    float GetItemCooldown();
    bool GetUsesAmmo();
    int ammo { get; set; }
    UsableItemEvent onItemUse { get; set; }
    UsableItemEvent onItemOutOfAmmo { get; set; }
    UsableItemEvent onReload { get; set; }
    float currentItemCooldown { get; set; }
}
public interface IItemUser : IBase
{
    List<IUsableItem> SetUsableItems(List<IUsableItem> usableItems);
    List<IUsableItem> GetUsableItems();
    IUsableItem currentEquippedItem { get; set; }
}
public class UsableItemEvent : UnityEvent<IUsableItem> { }
public class ItemUserEvent : UnityEvent<IItemUser> { }