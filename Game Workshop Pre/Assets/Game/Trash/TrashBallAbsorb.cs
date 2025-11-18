using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrashBallAbsorb : MonoBehaviour
{
    public TrashBall parentTrashBall;
    void Awake()
    {
        parentTrashBall = GetComponentInParent<TrashBall>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        parentTrashBall.AbsorbCollide(other);
    }
}
