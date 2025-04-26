using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attach3DTrash : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        GameObject target = collision.collider.gameObject;

        if (target.CompareTag("3DTrash") && !target.transform.IsChildOf(transform))
        {
            target.transform.parent = transform;

            target.transform.localPosition = target.transform.localPosition.normalized / 2f;
        }
    }
}
