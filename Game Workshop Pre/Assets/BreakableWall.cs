using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableWall : MonoBehaviour
    
{
    [Tooltip("The ball size necessary to break this wall")]
    [SerializeField] int _breakSize;
    [Tooltip("The ball speed necessary to break this wall")]
    [SerializeField] float _breakSpeed;

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.TryGetComponent(out TrashBall trashBall))
        {
            if ((trashBall.Size > _breakSize) && (trashBall.Rigidbody.velocity.magnitude * 10) > _breakSpeed)
            {
                Destroy(gameObject);
            }
        }
    }
}
