using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class WaveManager : MonoBehaviour
{

    public static WaveManager Instance { get; private set; }

    public int global_wave_interval; // Spawn wave interval

    private void Awake()
    {
        // Singleton instance
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        DontDestroyOnLoad(gameObject); // Exists across all scenes
    }

    public int GetGlobalWaveInterval()
    {
        return global_wave_interval;
    }

    public void SetGlobalWaveInterval(int x)
    {
        global_wave_interval = x;
    }
}
