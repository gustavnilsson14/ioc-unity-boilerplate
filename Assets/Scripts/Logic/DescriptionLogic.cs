using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class DescriptionLogic : InterfaceLogicBase
{
    public static DescriptionLogic I;

    protected override void OnInstantiate(GameObject newInstance)
    {
        base.OnInstantiate(newInstance);
        InitDescribed(newInstance);
    }
    private void InitDescribed(GameObject newInstance)
    {
        if (!newInstance.TryGetComponent<IDescribed>(out IDescribed described))
            return;
        described.onDescriptionChange = new TextEvent();
    }

    public string GetDescription(IDescribed described) {
        string description = described.GetDescription();
        if (!ReflectionUtil.GetStoredObject(out StoredObject storedObject, described))
            return description;
        return StringUtil.FormatStringWithDict(description, ReflectionUtil.StoredObjectToDict(storedObject));
    }
}

public interface IDescribed : IBase
{
    string GetDescription();
    TextEvent onDescriptionChange { get; set; }
}
public class TextEvent : UnityEvent<IDescribed>{ }
