using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class PrefabFactory : InterfaceLogicBase
{
    public static PrefabFactory I;
    public InstantiateEvent onInstantiate = new InstantiateEvent();
    public InstantiateEvent onRegisterInternalListeners = new InstantiateEvent();
    public Transform gameRoot;

    protected override void PostStart()
    {
        GameObject.FindObjectsOfType(typeof(GameObject)).ToList().ForEach(x => StartCoroutine(RegisterNewInstance(x as GameObject)));
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
        StartCoroutine(RegisterNewInstance(newGameObject));
        return newGameObject;
    }

    public IEnumerator RegisterNewInstance(GameObject newGameObject)
    {
        onInstantiate.Invoke(newGameObject);
        yield return 0;
        onRegisterInternalListeners.Invoke(newGameObject);
        
    }
}
public class InstantiateEvent : UnityEvent<GameObject> { }