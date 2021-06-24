using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;
using System;

public class WorldTextLogic : InterfaceLogicBase
{
    public static WorldTextLogic I;
    public GameObject worldTextPrefab;
    public List<IWorldText> worldTexts = new List<IWorldText>();
    protected override void OnInstantiate(GameObject newInstance, IBase newBase)
    {
        base.OnInstantiate(newInstance, newBase);
        InitWorldText(newBase as IWorldText);
    }
    private void InitWorldText(IWorldText worldText)
    {
        if (worldText == null)
            return;
        worldTexts.Add(worldText);
        worldText.textContainer = Instantiate(worldTextPrefab, worldText.GetGameObject().transform).GetComponentInChildren<TextMeshPro>();
        worldText.textContainer.text = worldText.GetWorldText();
        worldText.textContainer.gameObject.SetActive(false);
    }
    protected override void UnRegister(IBase b)
    {
        base.UnRegister(b, new List<IList>() {
            worldTexts
        });
    }
    private void Update()
    {
        worldTexts.ForEach(x => x.textContainer.transform.parent.LookAt(GetLookAtPosition(x.GetGameObject())));
    }

    private Vector3 GetLookAtPosition(GameObject g)
    {
        Vector3 lookAtPosition = Camera.main.transform.position;
        lookAtPosition.x = g.transform.position.x;
        return lookAtPosition;
    }

    public void Show(IWorldText worldText)
    {
        worldText.textContainer.gameObject.SetActive(true);
    }
}

public interface IWorldText : IBase
{
    WorldTextEvent onWorldTextShow { get; set; }
    WorldTextEvent onWorldTextHide { get; set; }
    string GetWorldText();
    string SetWorldText(string text);
    TextMeshPro textContainer { get; set; }
}
public class WorldTextEvent : UnityEvent<IWorldText> { }