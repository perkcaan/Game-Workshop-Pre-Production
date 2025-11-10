using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTracker : MonoBehaviour
{
    void Update()
    {
        // Constantly set to the player's position
        transform.localPosition = GameObject.FindWithTag("Player").transform.position;
    }
}
