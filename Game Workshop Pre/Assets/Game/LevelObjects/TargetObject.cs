using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TargetObject : MonoBehaviour
{

    // Velocity needed to break the object
    [SerializeField] private float requiredVelocity;

    // Size needed to break the object
    [SerializeField] private float requiredSize;

    // Target Object's event
    [SerializeField] private UnityEvent BreakEvent;

    void OnTriggerEnter2D(Collider2D collision)
    {
        // Check that object is Trash Ball
        // and that its Size and Speed are equal to or exceed the Target Object's
        if (collision.gameObject.GetComponent<TrashBall>() == true)
        {
            if (collision.gameObject.GetComponent<TrashBall>().Size >= requiredSize 
                && Vector3.Magnitude(collision.gameObject.GetComponent<Rigidbody2D>().velocity) >= requiredVelocity)
            {
                Destroy(gameObject);
                BreakEvent?.Invoke(); // Call associated event
            }   
        }
    }


}
