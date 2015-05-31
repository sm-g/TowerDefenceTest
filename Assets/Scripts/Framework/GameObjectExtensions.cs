using System;
using System.Linq;
using UnityEngine;

public static class GameObjectExtensions
{
    /// <summary>
    /// Расстояние до цели в допустимых пределах.
    /// </summary>
    public static bool InRadialArea(this GameObject source, Transform target, float minDistance, float maxDistance)
    {
        return InRadialArea(source.transform, target, minDistance, maxDistance);
    }

    /// <summary>
    /// Расстояние до цели в допустимых пределах.
    /// </summary>
    public static bool InRadialArea(this Transform source, Transform target, float minDistance, float maxDistance)
    {
        var distance = Vector3.Distance(target.position, source.transform.position);
        return minDistance < distance && distance < maxDistance;
    }

    public static void AddChild(this GameObject parent, GameObject child)
    {
        child.transform.parent = parent.transform;
    }
}
