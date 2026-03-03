 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoostPad : MonoBehaviour
{
    [SerializeField] float _boostAmount = 3;
    [SerializeField] float _maxBoostSpeed = 10000;
    [SerializeField] private Vector2 entryDirection = Vector2.up; // allowed direction of entry

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("TrashBall"))
        {
            TrashBall trashBall = collision.GetComponent<TrashBall>();
            Rigidbody2D rb = trashBall.Rigidbody;

            //relative velocity in boost pad's local space
            Vector2 localVelocity = transform.InverseTransformDirection(rb.velocity);
            //is ball coming from correct direction
            if (Vector2.Dot(localVelocity.normalized, entryDirection.normalized) > 0.5f)
            {
                Boost(rb);
            }
        }


    }


    public void Boost(Rigidbody2D rigidbody)
    {
        float currentSpeed = rigidbody.velocity.magnitude;

        if (currentSpeed < _maxBoostSpeed)
        {
            rigidbody.velocity *= _boostAmount;
        } else
        {
            rigidbody.velocity = rigidbody.velocity.normalized * _maxBoostSpeed;
        }

    }

}
