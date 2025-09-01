using System.Collections;
using UnityEngine;

public class CollectableTrash : PushableObject
{
    public TrashBall trashBallPrefab;

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.TryGetComponent(out CollectableTrash collectableTrash))
        {
            if (gameObject.GetInstanceID() < other.gameObject.GetInstanceID())
            {
                CreateMergedTrashBall(collectableTrash);
            }
        }
    }
    private void CreateMergedTrashBall(CollectableTrash otherTrash)
    {
        // Instantiate the trash ball
        TrashBall newTrashBall = Instantiate(trashBallPrefab);
        newTrashBall.transform.position = transform.position;
        newTrashBall.weight = this.weight + otherTrash.weight;
        newTrashBall.SetSize();
        
        // Destroy the collectableTrash
        Destroy(otherTrash.gameObject);
        Destroy(this.gameObject);
    }
}
