using UnityEngine;

public class EnemySubCollider : MonoBehaviour//, ISwipeable
{ 
    public GameObject HitParent => _parent.gameObject;
    private EnemyBase _parent;    

    public void Initialize(EnemyBase parent)
    {
        _parent = parent;
    }

    public void OnSwipe(Vector2 direction, float force, Collider2D collider)
    {
        // Class unimplemented for now. Mostly just a test
    }
}
