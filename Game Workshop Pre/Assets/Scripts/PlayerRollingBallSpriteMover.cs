using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRollingBallSpriteMover : MonoBehaviour
{
    float size;
    Vector2 targetDir = Vector2.one;
    [SerializeField] RotatingBall trashBall;

    void Start()
    {
        targetDir = targetDir.normalized;
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
