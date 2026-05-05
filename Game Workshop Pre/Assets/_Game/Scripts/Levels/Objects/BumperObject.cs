using System.Collections;
using DG.Tweening;
using UnityEngine;

public class BumperObject : MonoBehaviour
{
    [SerializeField] float bounceForce = 10f;
    Animator animator;
    Collider2D _collider2D;
    void Awake()
    {
        animator = GetComponent<Animator>();
        _collider2D = GetComponent<Collider2D>();
    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.rigidbody == null) return;
        if (collision.transform.TryGetComponent(out PlayerMovementController player))
        {
            BounceObject(collision, 3f);
        }
        else
        {
            BounceObject(collision);
        }
        animator.SetTrigger("Bounce");
    }

    void BounceObject(Collision2D collision, float forceMultiplier = 1f)
    {
        if (collision.gameObject.TryGetComponent(out ISwipeable target))
        {
            Vector3 bounceDirection = (collision.transform.position - transform.position).normalized;
            float knockbackMultiplier = 0f;
            target.OnSwipe(bounceDirection, bounceForce * forceMultiplier, _collider2D, ref knockbackMultiplier);
        }
    }
}
