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
        return Create(prefab, parent, parent);
    }
    public GameObject Create(GameObject prefab, Transform parent, Transform origin)
    {
        GameObject newGameObject = Instantiate(prefab, parent);
        newGameObject.transform.position = origin.position;
        onInstantiate.Invoke(newGameObject);
        return newGameObject;
    }
}
public class InstantiateEvent : UnityEvent<GameObject> { }