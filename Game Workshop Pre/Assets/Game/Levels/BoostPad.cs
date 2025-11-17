using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoostPad : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("TrashBall"))
            {
                TrashBall trashBall = collision.GetComponent<TrashBall>();
                boostTrash(trashBall);
            }
        


    }

    public void boostTrash(TrashBall trashBall)
    {

    }

}
