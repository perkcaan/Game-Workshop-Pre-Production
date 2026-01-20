using System.Collections;
using DG.Tweening;
using UnityEngine;

public class BumperObject : MonoBehaviour
{
    [SerializeField] float bounceForce = 10f;
    [SerializeField] Transform outerRing;
    void Start()
    {
        
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

        BounceAnimation();
    }

    void BounceAnimation()
    {
        outerRing.localScale = Vector3.one * 1.4f;
        outerRing.DOScale(Vector3.one, 0.2f).SetEase(Ease.OutExpo);
    }

    void BounceObject(Collision2D collision, float forceMultiplier = 1f)
    {
        Vector3 bounceDirection = (collision.transform.position - transform.position).normalized;
        collision.rigidbody.AddForce(bounceDirection * bounceForce * forceMultiplier, ForceMode2D.Impulse); 
    }
}
