using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Is grounded, used for pits and lava to ensure something has to "fall" into it.
// Maybe make it a full height system and add some kind of falling animation later?
public class GroundedMechanic : MonoBehaviour
{
    [SerializeField, ReadOnly] private int _isGrounded = 0;
    public int IsGrounded
    {
        get { return _isGrounded; }
        set
        {
            _isGrounded = value;
        }
    }


}
