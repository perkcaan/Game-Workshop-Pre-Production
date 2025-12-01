using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class ExplodingLava : MonoBehaviour
{
    [SerializeField] float explosionTime = 12f;
    [SerializeField] float warningTime = 3f;
    [SerializeField] float explosionDuration = 3f;
    [SerializeField] private EnemyHeatHitbox heatArea;
    SpriteRenderer sr;
    private bool isExploding = false;
    float heatMax = 45f; //half of players heat
    float timer;


    // Start is called before the first frame update
    void Start()
    {
        timer = 0f;
        sr = GetComponent<SpriteRenderer>();
        heatArea.Disable();
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (timer > warningTime)
        {
            warning();
        }

        if (timer > explosionTime)
        {
            explode();
        }

        if (timer > explosionTime + explosionDuration)
        {
            isExploding = false;
            timer = 0f;
            heatArea.Disable();
        }
        
    }

    void explode()
    {
        isExploding = true;
        heatArea.Enable();
    
    }

    void warning()
    {
        sr.color = Color.red;
    } 
}
