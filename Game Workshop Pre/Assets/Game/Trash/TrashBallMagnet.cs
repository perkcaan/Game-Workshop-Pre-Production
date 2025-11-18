using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrashBallMagnet : MonoBehaviour
{
    public TrashBall parentTrashBall;
    void Awake()
    {
        parentTrashBall = GetComponentInParent<TrashBall>();
    }

    void OnTriggerStay2D(Collider2D other)
    {
        parentTrashBall.MagnetCollide(other);
    }
}
