using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StandingGround : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<GroundedMechanic>(out GroundedMechanic gm))
        {
            gm.IsGrounded = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.TryGetComponent<GroundedMechanic>(out GroundedMechanic gm))
        {
            gm.IsGrounded = false;
        }
    }
}
