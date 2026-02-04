using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrashBallAbsorb : MonoBehaviour
{
    private TrashBall _parentTrashBall;
    
    private void Awake()
    {
        _parentTrashBall = GetComponentInParent<TrashBall>();
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        _parentTrashBall.OnAbsorbTrigger(collider);
    }
}
