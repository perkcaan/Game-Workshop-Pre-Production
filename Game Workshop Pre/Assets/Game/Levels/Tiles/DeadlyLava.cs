using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadlyLava : MonoBehaviour
{
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.TryGetComponent(out CollectableTrash trash))
        {
            Destroy(trash.gameObject);
            PlayerState.Instance.trashDeleted.Invoke(trash.Size);
        }
        if (collision.gameObject.TryGetComponent(out TrashBall trashBall))
        {
            // TODO: replace damage with heat and put this into a trigger enter. Heat mechanics haven't been added yet.
            trashBall.TempMelt();
            PlayerState.Instance.trashDeleted.Invoke(trashBall.Size);
        }
    }
}
