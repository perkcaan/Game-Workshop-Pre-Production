using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lava : MonoBehaviour
{
    [SerializeField] private float _heatPerSecond = 200f;
    [SerializeField] private float _heatPerSecondWhenGrounded = 10f;
    [SerializeField] private float _maxHeatWhenGrounded = 70f;
    [SerializeField] private float _delayBeforeHeatingWhenGrounded = 1f;
    private float _delayTimer = 0f;
    private void OnTriggerStay2D(Collider2D collider)
    {
        if (collider.TryGetComponent(out HeatMechanic heat))
        {
            //Check for grounded safety. but still toast them a little
            if (collider.TryGetComponent(out GroundedMechanic gm))
            {
                if (gm.IsGrounded) 
                {
                    
                        _delayTimer += Time.fixedDeltaTime;
                        if (_delayTimer > _delayBeforeHeatingWhenGrounded)
                        {
                            if (heat.Heat < _maxHeatWhenGrounded)
                            {
                                heat.ModifyHeat(_heatPerSecondWhenGrounded * Time.fixedDeltaTime);
                            }
                            else
                            {
                                heat.ModifyHeat(0); // dont cooldown, just stay at max heat
                            }
                        } 
                    
                }
                if (!gm.IsGrounded) return;
            }
            // otherwise... burn them to a crisp
            heat.ModifyHeat(_heatPerSecond * Time.fixedDeltaTime);
        }
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.TryGetComponent(out GroundedMechanic gm))
        {
            _delayTimer = 0f;
        }
    }
}
