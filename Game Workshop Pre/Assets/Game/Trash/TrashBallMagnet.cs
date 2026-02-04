using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrashBallMagnet : MonoBehaviour
{
    private TrashBall _parentTrashBall;

    private void Awake()
    {
        _parentTrashBall = GetComponentInParent<TrashBall>();
    }

    private void OnTriggerStay2D(Collider2D collider)
    {
        _parentTrashBall.OnMagnetStay(collider);
    }
}
