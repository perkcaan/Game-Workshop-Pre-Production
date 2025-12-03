using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableWall : MonoBehaviour
    
{
    [Tooltip("The ball size necessary to break this wall")]
    [SerializeField] int breakSize;
    [Tooltip("The ball speed necessary to break this wall")]
    [SerializeField] float breakSpeed;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        TrashBall trashBall = collision.gameObject.GetComponent<TrashBall>();

        if (trashBall != null && (trashBall.Size > breakSize) && ((trashBall.rigidBody.velocity.magnitude * 10) > breakSpeed))
        {
            Destroy(this.gameObject);
        }
    }
}
