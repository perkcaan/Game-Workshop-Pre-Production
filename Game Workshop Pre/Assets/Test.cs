using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{

    public Rigidbody2D rb;
    public float velocityMultiplier = 10f;

    private void FixedUpdate()
    {
        rb.AddForce(Vector2.down * velocityMultiplier, ForceMode2D.Force);
    }
}
