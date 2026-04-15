using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistortScript : MonoBehaviour
{
    public Material distortMaterial;

    void Update()
    {
        if (distortMaterial != null)
        {
            distortMaterial.SetFloat("_TimeValue", Time.time);
        }
    }
}
