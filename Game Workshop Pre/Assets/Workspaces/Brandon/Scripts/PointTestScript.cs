using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointTestScript : MonoBehaviour
{

    private bool _activated = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.name == "PlayerController" && _activated == false)
            TrashRadarManager.IncreaseSequenceNumber();
        else
            return;

        _activated = true;
    }

}
