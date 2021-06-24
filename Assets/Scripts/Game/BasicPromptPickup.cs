using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BasicPromptPickup : BehaviourBase, IPromptPickup
{
    public PromptPickupEvent onPromptShow { get; set; }
    public PromptPickupEvent onPromptHide { get; set; }
    public bool isHeld { get; set; }
    public PickupEvent onPickupPickup { get; set; }
    public PickupEvent onPickupDrop { get; set; }
    public WorldTextEvent onWorldTextShow { get; set; }
    public WorldTextEvent onWorldTextHide { get; set; }
    public TextMeshPro textContainer { get; set; }

    public PickupType GetPickupType() => PickupType.USABLE_ITEM;

    public string GetWorldText()
    {
        throw new System.NotImplementedException();
    }

    public string SetWorldText(string text)
    {
        throw new System.NotImplementedException();
    }
}
