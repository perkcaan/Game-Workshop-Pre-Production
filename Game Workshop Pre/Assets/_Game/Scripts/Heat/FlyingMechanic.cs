using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Like Grounded Mechanic, but for flying Enemies
public class FlyinMechanic : MonoBehaviour
{
    [SerializeField, ReadOnly] private int _isFlying = 0;
    public int IsFlying
    {
        get { return _isFlying; }
        set
        {
            _isFlying = value;
        }
    }
}
