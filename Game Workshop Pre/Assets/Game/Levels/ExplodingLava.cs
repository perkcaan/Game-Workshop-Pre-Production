using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class ExplodingLava : MonoBehaviour
{
    [SerializeField] float explosionTime = 10f;
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
        Debug.Log(timer);
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
            Debug.Log("reset");
            isExploding = false;
            timer = 0f;
            sr.color = Color.yellow; // back to yellow
            heatArea.Disable();
            heatArea.HideSprite(); 
        }
        
    }

    void explode()
    {
        Debug.Log("exploding");
        isExploding = true;
        heatArea.Enable();
        heatArea.ShowSprite(); //heat area shows orange 
    }

    void warning()
    {
        Debug.Log("warning");
        sr.color = Color.red;
    } 
}
