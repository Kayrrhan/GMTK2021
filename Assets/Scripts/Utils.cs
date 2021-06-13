using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public static class Utils
{
    public static void BindSingleton<T>(this DiContainer container, T instance)
    {
        container.Bind<T>().FromInstance(instance).AsSingle();
    }

    public static bool IsGrounded(Rigidbody rigidbody, Collider collider)
    {
        if (Mathf.Abs(rigidbody.velocity.y) > 0.5f)
        {
            return false;
        }

        GameObject gameObject = rigidbody.gameObject;
        int layer = LayerMask.NameToLayer("TempLayer");
        int mask = ~(1 << layer | 1 << LayerMask.NameToLayer("Ignore Raycast"));
        Bounds bounds = collider.bounds;
        // Temporary apply layer to the current monkey.
        int originalLayer = gameObject.layer;
        rigidbody.gameObject.layer = layer;
        bool res = Physics.CheckCapsule(bounds.center, bounds.center + (bounds.extents.y + 0.1f) * Vector3.down, 
            bounds.size.x, mask, QueryTriggerInteraction.Ignore);
        // Restore layer
        gameObject.layer = originalLayer;
        return res;
    }
}
