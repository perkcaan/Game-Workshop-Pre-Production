using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatingBall : MonoBehaviour
{
    Vector3 direction;
    Vector3 up;
    Vector3 oldPosition;
    private float _size;
    public float size
    {
        get
        {
            return _size;
        }
        set
        {
            ScaleUp(value);
            _size = value;
        }
    }

    private void Start()
    {
        up = new Vector3(0, Mathf.Sqrt(2f) / 2f, -Mathf.Sqrt(2f) / 2f);
        oldPosition = transform.position;
        size = 4;
    }

    private void ScaleUp(float s)
    {
        transform.localScale = new Vector3(s, s, s);
    }

    // Update is called once per frame
    void Update()
    {
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
