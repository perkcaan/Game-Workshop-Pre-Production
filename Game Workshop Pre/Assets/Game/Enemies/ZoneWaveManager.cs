using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoneWaveManager : MonoBehaviour // Assign a ZoneWaveManager to each Room
{
    [Header("Wave Settings (Cleanliness)")]

    public bool cleanMeterWaves; // Is the spawner tied to cleanliness meter or not

    //TODO: This needs to be fixed. Cleanliness doesnt exist as a singleton anymore.
    //private Cleanliness cm; // References the room's cleanliness meter
    private float cm = 1f;

    public Dictionary<float, bool> waveThresholds = new Dictionary<float, bool>(); // Tracks the thresholds that trigger a wave, and if they've already been triggered

    private void Start()
    {
        var keys = new List<float>(waveThresholds.Keys);

        foreach (var key in keys)
        {
            waveThresholds[key] = false;
        }
    }

    public float GetCleanlinessValue()
    {
        return cm;
    }
}
