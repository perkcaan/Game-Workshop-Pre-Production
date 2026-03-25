using System.Collections.Generic;
using UnityEngine;

public static class HitProcessor
{
    public static List<(Collider2D collider, float distance)> ProcessHits<T>(Collider2D[] hits, Vector2 origin) where T : IHittable
    {
        // Sort hits by their HitParent
        Dictionary<GameObject, List<Collider2D>> hitObjects = new();
        foreach (Collider2D collider in hits)
        {
            if (!collider.TryGetComponent(out T hittable)) continue;
            if (hittable.HitParent == null)
            {
                Debug.LogWarning("Please ensure a valid HitParent for Swipeable object: " + collider.name);
                continue;
            }

            if (!hitObjects.TryGetValue(hittable.HitParent, out List<Collider2D> list))
            {
                list = new List<Collider2D>();
                hitObjects.Add(hittable.HitParent, list);
            }
            list.Add(collider);
        }

        // Find best collider on each object.
        List<(Collider2D collider, float distance)> bestColliders = new();
        foreach (KeyValuePair<GameObject, List<Collider2D>> kvp in hitObjects)
        {
            GameObject gameObject = kvp.Key;
            List<Collider2D> colliders = kvp.Value;
            
            Collider2D bestCollider = colliders[0];
            float closestDistance = float.MaxValue;

            foreach (Collider2D collider in colliders)
            {
                float dist = Vector2.Distance(origin, collider.ClosestPoint(origin));
                if (dist < closestDistance)
                {
                    bestCollider = collider;
                    closestDistance = dist;
                }

            }

            bestColliders.Add((bestCollider, closestDistance));
        }

        // Sort by distance and Swipe each of them in order
        bestColliders.Sort((a, b) => a.distance.CompareTo(b.distance));
        return bestColliders;
    }
}
