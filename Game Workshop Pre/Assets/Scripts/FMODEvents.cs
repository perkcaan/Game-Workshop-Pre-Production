using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class FMODEvents : MonoBehaviour
{

    [field: Header("LavaAmbience")]
    [field: SerializeField] public EventReference lavaAmbience { get; private set; }


    public static FMODEvents instance { get; private set; }


    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("Found more than one FMOD Events instance in the scene.");
        }
        instance = this;
    }

    
}
