using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersistentCam : MonoBehaviour
{
    public static PersistentCam instance { get; private set; }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
