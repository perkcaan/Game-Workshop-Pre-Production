 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoostPad : MonoBehaviour
{
    Rigidbody2D rb;
    float cappedSpeed; //80% of max speed
    [SerializeField] float boostAmount;
    float currentSpeed;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("TrashBall"))
            {
                TrashBall trashBall = collision.GetComponent<TrashBall>();
                rb = trashBall._rigidBody;
                boostTrash(trashBall);
            }
        


    }

    public void boostTrash(TrashBall trashBall)
    {
        cappedSpeed = (trashBall._maxSpeed * .8f);
        currentSpeed = rb.velocity.magnitude;

        if (currentSpeed < cappedSpeed)
        {
            rb.velocity *= boostAmount;

            if (currentSpeed > cappedSpeed)
            {
                rb.velocity = rb.velocity.normalized * cappedSpeed;
            }
        }
    }

}
