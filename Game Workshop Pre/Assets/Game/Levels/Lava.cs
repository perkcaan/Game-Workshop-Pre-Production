using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lava : MonoBehaviour
{
    [SerializeField] private float _heatPerSecond = 1f;

    private void OnTriggerStay2D(Collider2D collider)
    {
        if (collider.TryGetComponent(out HeatMechanic heat))
        {
            //Check for grounded safety.
            if (collider.TryGetComponent(out GroundedMechanic gm))
            {
                if (gm.IsGrounded) return;
            }

            // otherwise... burn them to a crisp
            heat.ModifyHeat(_heatPerSecond * Time.fixedDeltaTime);
        }
    }
}
