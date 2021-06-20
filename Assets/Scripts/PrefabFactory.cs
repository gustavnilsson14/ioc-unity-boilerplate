using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class PrefabFactory : InterfaceLogicBase
{
    public static PrefabFactory I;
    public InstantiateEvent onInstantiate = new InstantiateEvent();

    protected override void PostStart()
    {
        Resources.FindObjectsOfTypeAll(typeof(GameObject)).ToList().ForEach(x => onInstantiate.Invoke(x as GameObject));
    }

    public GameObject Create(GameObject prefab)
    {
        return Create(prefab, null);
    }
    public GameObject Create(GameObject prefab, Transform parent)
    {
        GameObject newGameObject = Instantiate(prefab, parent);
        onInstantiate.Invoke(newGameObject);
        return newGameObject;
    }
}
public class InstantiateEvent : UnityEvent<GameObject> { }