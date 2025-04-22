using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRollingBallSpriteMover : MonoBehaviour
{
    float size;
    Vector2 targetDir = Vector2.one;
    RotatingBall trashBall = null;

    void Start()
    {
        targetDir = targetDir.normalized;

        Transform holdChild = null;

        foreach(Transform x in transform.parent.transform)
        {
            if(x.name == "Trash Ball")
            {
                holdChild = x;
            }
        }

        if(holdChild != null)
        {
            trashBall = holdChild.gameObject.GetComponent<RotatingBall>();
        }
    }

    void Update()
    {
        Vector2 normalizedAxisDir = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")).normalized;

        if (normalizedAxisDir.magnitude > 0)
        {
            targetDir = -normalizedAxisDir;
        }

        float holdZ = 0;

        if (targetDir.y < 0)
        {
            holdZ = -7f;
        }
        else
        {
            holdZ = -3f;
        }

            transform.localPosition = new Vector3(targetDir.x * trashBall.size / 2f, targetDir.y * trashBall.size / 2f, holdZ);
    }
}
