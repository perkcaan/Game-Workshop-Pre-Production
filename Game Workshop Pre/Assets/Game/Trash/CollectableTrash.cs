using System.Collections;
using UnityEngine;

public class CollectableTrash : PushableObject
{
    public TrashBall trashBallPrefab;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.TryGetComponent(out CollectableTrash trash))
        {
            if (rb.velocity.magnitude < trash.rb.velocity.magnitude)
            {
                CreateMergedTrashBall(trash);
            }
        }
    }
    private void CreateMergedTrashBall(CollectableTrash otherTrash)
    {
        // Instantiate the trash ball
        TrashBall newTrashBall = Instantiate(trashBallPrefab);
        newTrashBall.transform.position = otherTrash.transform.position;
        newTrashBall.weight = weight + otherTrash.weight;
        newTrashBall.rb.velocity = rb.velocity + otherTrash.rb.velocity;
        newTrashBall.consumedTrash.Add(this);
        newTrashBall.consumedTrash.Add(otherTrash);
        newTrashBall.SetSize();

        // Destroy the collectableTrash
        Destroy(otherTrash.gameObject);
        Destroy(gameObject);
    }
}
