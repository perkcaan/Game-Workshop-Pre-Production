using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Heatable : MonoBehaviour
{

    //Current Heat Level
    [Header("Objects Current Heat Level")]
    [SerializeField] int heatLevel;

    //Rate at which heat level falls for actor
    [Header("Rate at which heat falls per second")]
    [SerializeField] int heatEntropy;

    //Max heat for actor before adverse affects
    [Header("Threshold at which adverse affects start")]
    [SerializeField] int heatThreshold;


    void Start()
    {
        StartCoroutine(HeatEntropy());
    }

    public void RaiseHeat(int heatValue)
    {
        this.heatLevel = Mathf.Clamp(this.heatLevel + heatValue, 0, heatThreshold);
    }

    public void LowerHeat(int heatValue)
    {
        this.heatLevel = Mathf.Clamp(this.heatLevel - heatValue, 0, heatThreshold);
    }

    private IEnumerator HeatEntropy()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);
            LowerHeat(heatEntropy);
        }
        
    }

}
