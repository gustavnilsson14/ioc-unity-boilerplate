using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BehaviourBase : MonoBehaviour, IBase
{
    public DestroyEvent onDestroy { get; set; }
    public CollisionEvent onCollision { get; set; }
    public int uniqueId { get; set; }
    public GameObject GetGameObject() => gameObject;
    private void OnCollisionEnter(Collision collision)
    {
        if (onCollision == null)
            return;
        onCollision.Invoke(this, collision);
    }
    private void OnDestroy()
    {
        if (onDestroy == null)
            return;
        onDestroy.Invoke(this);
    }
}
