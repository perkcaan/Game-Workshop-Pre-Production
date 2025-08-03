using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hitbox : MonoBehaviour
{
    public GameObject fakeTrash;
    public GameObject Hitboxobj;
    public int BaseSpeed;

    // Start is called before the first frame update
    void Start()
    {
        // Explicitly convert Vector2 to Vector3 to resolve ambiguity
        Hitboxobj.transform.position -= (Vector3)new Vector2(.9f, 0);
    }

    // Update is called once per frame
    void Update()
    {
        if (Score.score > 0)
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
            Score.score++;
            TrashScore.score += 1;
            Destroy(other.gameObject);
            if (Score.score > 1)
            {
                BaseSpeed += 2;
            }
        }

        if (other.CompareTag("BigTrash"))
        {
            Score.score += 5;
            TrashScore.score += 1;
            Destroy(other.gameObject);
            if (Score.score > 1)
            {
                BaseSpeed += 4;
            }
        }

        Hitboxobj.transform.localScale += new Vector3(0.2f, 0.2f, 0.2f);
    }
}
