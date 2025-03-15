using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatingBall : MonoBehaviour
{
    Vector3 direction;
    Vector3 up;
    float speed = 100;

    private void Start()
    {
        up = new Vector3(0, Mathf.Sqrt(2f) / 2f, -Mathf.Sqrt(2f) / 2f);
    }

    // Update is called once per frame
    void Update()
    {
        direction = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

        Vector3 rotateAxis = Vector3.Cross(direction, up);

        print(rotateAxis);

        transform.Rotate(rotateAxis, -direction.magnitude * speed * Time.deltaTime, Space.World);
    }
}
