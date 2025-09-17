using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadlyLava : MonoBehaviour
{
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.TryGetComponent(out Trash trash))
        {
            Destroy(trash.gameObject);
            PlayerState.Instance.trashDeleted.Invoke(trash.Size);
        }
    }
}
