using System.Collections;
using UnityEngine;

public class CollectableTrash : MonoBehaviour
{
    public float trashSize;
    public Rigidbody2D rb;
    public TrashBall trashBallPrefab;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Throw(Vector2 direction, float strength)
    {
        rb.velocity = direction * strength;
    }
    
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.TryGetComponent(out TrashBall trashBall))
        {
            trashBall.trashSize += trashSize;
            Destroy(this.gameObject);
            return;
        }

        if (other.gameObject.TryGetComponent(out CollectableTrash collectableTrash))
        {
            print("what");
            TrashBall newTrashBall = Instantiate(trashBallPrefab);
            newTrashBall.trashSize = trashSize + collectableTrash.trashSize;
            Destroy(collectableTrash.gameObject);
            Destroy(this.gameObject);
            return;
        }
    }
}
