using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StandingGround : MonoBehaviour
{
    protected List<GroundedMechanic> groundedObjects = new List<GroundedMechanic>();
    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out GroundedMechanic groundedObject))
        {
            if (!groundedObjects.Contains(groundedObject))
            {
                groundedObjects.Add(groundedObject);
                groundedObject.IsGrounded++;
            }
        }
    }

    protected virtual void OnTriggerExit2D(Collider2D other)
    {
        if (other.TryGetComponent(out GroundedMechanic groundedObject))
        {
            if (groundedObjects.Contains(groundedObject))
            {
                groundedObjects.Remove(groundedObject);
                groundedObject.IsGrounded--;
            }
        }
    }
}
