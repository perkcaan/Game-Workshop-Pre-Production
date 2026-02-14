using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Is grounded, used for pits and lava to ensure something has to "fall" into it.
// Maybe make it a full height system and add some kind of falling animation later?
public class GroundedMechanic : MonoBehaviour
{
    [SerializeField] private bool _isGrounded = false;
    public bool IsGrounded
    {
        get { return _isGrounded; }
        set
        {
            _isGrounded = value;
        }
    }


}
