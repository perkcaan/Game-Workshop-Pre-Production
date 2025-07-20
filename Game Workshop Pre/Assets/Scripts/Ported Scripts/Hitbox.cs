using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hitbox : MonoBehaviour
{
    public int TrashScore = 0;
    public GameObject fakeTrash;
    public GameObject Hitboxobj;
    public int BaseSpeed;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (TrashScore > 0)
        {
            fakeTrash.SetActive(true);
        }
        else
        {
            fakeTrash.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Trash") || other.CompareTag("TrashBullet"))
        {
            TrashScore++;
            Destroy(other.gameObject);
            if (TrashScore > 1)
            {
                Hitboxobj.transform.localScale += new Vector3(0.2f, 0.2f, 0.2f);
                BaseSpeed += 2;

            }
        }

        
        if (other.CompareTag("BigTrash"))
        {
            TrashScore += 5;
            Destroy(other.gameObject);
            if (TrashScore > 1)
            {
                Hitboxobj.transform.localScale += new Vector3(0.4f, 0.4f, 0.4f);
                BaseSpeed += 4;
            }
        }


        


    }

   


}
