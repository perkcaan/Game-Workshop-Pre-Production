using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatingBall : MonoBehaviour
{
    Vector3 direction;
    Vector3 up;
    Vector3 oldPosition;
    public float size
    {
        get
        {
            return size;
        }
        set
        {
            ScaleUp(value);
            size = value;
        }
    }

    private void Start()
    {
        up = new Vector3(0, Mathf.Sqrt(2f) / 2f, -Mathf.Sqrt(2f) / 2f);
        oldPosition = transform.position;
        size = 0;
    }

    private void ScaleUp(float s)
    {
        transform.localScale = new Vector3(s, s, s);
    }

    void Update()
    {
        // Trash Sphere Rotation
        if (oldPosition != transform.position)
        {
            direction = transform.position - oldPosition;

            float displacement = -direction.magnitude;

            oldPosition = transform.position;

            Vector3 rotateAxis = Vector3.Cross(direction, up);

            transform.Rotate(rotateAxis, displacement / size * 2 * Mathf.Rad2Deg, Space.World);
        }
    }
}
