using System.Collections;
using UnityEngine;

public class CollectableTrash : PushableObject
{
    public bool mergable;
    public TrashBall trashBallPrefab;
    void Start()
    {
        WorldCleanliness.Instance.startingWorldTrash++;
        WorldCleanliness.Instance.currentWorldTrash++;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.TryGetComponent(out CollectableTrash trash))
        {
            if (rb.velocity.magnitude < trash.rb.velocity.magnitude && trash.mergable)
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

        // Disable the collectableTrash
        otherTrash.gameObject.SetActive(false);
        gameObject.SetActive(false);
    }

    public void OnDestroy()
    {
        if (WorldCleanliness.Instance != null)
            WorldCleanliness.Instance.RemoveTrash();
    }

    public void Explode()
    {
        StartCoroutine(MergeDelay());
    }

    public IEnumerator MergeDelay()
    {
        mergable = false;
        yield return new WaitForSeconds(1f);
        mergable = true;
    }
    
}
