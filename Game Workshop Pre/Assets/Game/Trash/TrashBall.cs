using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TrashBall : PushableObject
{
    [SerializeField] float scaleMultiplier;
    public List<CollectableTrash> consumedTrash = new List<CollectableTrash>();
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.TryGetComponent(out CollectableTrash trash))
        {
            consumedTrash.Add(trash);
            weight += trash.weight;
            SetSize();
            Destroy(trash.gameObject);
            return;
        }
        if (other.gameObject.TryGetComponent(out TrashBall trashBall))
        {
            if (rb.velocity.magnitude > trashBall.rb.velocity.magnitude)
            {
                consumedTrash.AddRange(trashBall.consumedTrash);
                weight += trashBall.weight;
                SetSize();
                Destroy(trashBall.gameObject);
            }
            return;
        }
    }

    public void SetSize()
    {
        float newSize = scaleMultiplier * Mathf.Sqrt(weight);
        transform.localScale = Vector3.one * newSize;
    }
}
