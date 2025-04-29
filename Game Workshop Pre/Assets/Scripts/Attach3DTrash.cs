using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attach3DTrash : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        GameObject target = collision.gameObject;

        if (target.CompareTag("3DTrash") && !target.transform.IsChildOf(transform))
        {
            target.transform.parent = transform;

            target.transform.localPosition = target.transform.localPosition.normalized / 2f;
        }
    }
}
