using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StandingGround : MonoBehaviour
{
    protected List<GroundedMechanic> groundedObjects = new List<GroundedMechanic>();
    [SerializeField] int groundedLevel = 1;
    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out GroundedMechanic gm))
        {
            if (!groundedObjects.Contains(gm))
            {
                groundedObjects.Add(gm);
                //gm.IsGrounded += groundedLevel;
            }
        }
    }

    protected virtual void OnTriggerExit2D(Collider2D other)
    {
        if (other.TryGetComponent(out GroundedMechanic gm))
        {
            if (groundedObjects.Contains(gm))
            {
                groundedObjects.Remove(gm);
                //gm.IsGrounded -= groundedLevel;
            }
        }
    }
}
