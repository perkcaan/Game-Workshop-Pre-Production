using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(SpriteRenderer))]
public class HeatController : MonoBehaviour
{
    public float heatLevel = 0f; // 0 → 3
    private SpriteRenderer sr;
    private MaterialPropertyBlock mpb;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        mpb = new MaterialPropertyBlock();
    }

    void Update()
    {
        sr.GetPropertyBlock(mpb);
        mpb.SetFloat("_HeatLevel", heatLevel); 
        sr.SetPropertyBlock(mpb);
    }
}