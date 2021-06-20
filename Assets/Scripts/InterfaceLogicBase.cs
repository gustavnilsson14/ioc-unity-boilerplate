using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;

public class InterfaceLogicBase : MonoBehaviour
{
    public List<GameObject> myInstances = new List<GameObject>();

    protected virtual void Awake()
    {
        FieldInfo field = GetType().GetField("I", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
        field.SetValue(null, this);
    }
    protected virtual void Start()
    {
        RegisterInstances();
        RegisterListeners();
        StartCoroutine(DelayedPostStart());
    }

    private IEnumerator DelayedPostStart()
    {
        yield return new WaitForSeconds(0.01f);
        PostStart();
    }

    protected virtual void PostStart() {  }

    protected virtual void RegisterInstances() { }

    protected virtual void RegisterListeners() {
        PrefabFactory.I.onInstantiate.AddListener(OnInstantiate);
    }
    protected virtual void OnInstantiate(GameObject newInstance)
    {
        if (!newInstance.TryGetComponent(out IBase newBase))
            return;
        myInstances.Add(newInstance);
        newInstance.GetComponents<IBase>().ToList().ForEach(x => OnInstantiate(newInstance, x));
    }
    protected virtual void OnInstantiate(GameObject newInstance, IBase newBase) {
        if (newBase.onDestroy != null) {
            newBase.onDestroy.AddListener(UnRegister);
            return;
        }
        newBase.uniqueId = newInstance.GetInstanceID();
        newBase.onDestroy = new DestroyEvent();
        newBase.onCollision = new CollisionEvent();
        newBase.onDestroy.AddListener(UnRegister);
    }

    protected virtual void UnRegister(IBase b)
    {
        myInstances.Remove(b.GetGameObject());
    }
}

public interface IBase {
    int uniqueId { get; set; }
    GameObject GetGameObject();
    DestroyEvent onDestroy { get; set; }
    CollisionEvent onCollision { get; set; }
}
public class DestroyEvent : UnityEvent<IBase> { }
public class CollisionEvent : UnityEvent<IBase, Collision> { }