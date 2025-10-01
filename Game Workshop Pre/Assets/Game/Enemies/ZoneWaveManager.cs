using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoneWaveManager : MonoBehaviour
{
    [Header("Wave Settings (Cleanliness)")]

    public bool cleanMeterWaves; // Is the spawner tied to cleanliness meter or not

    public Cleanliness cm; // References the room's cleanliness meter

    public Dictionary<float, bool> waveThresholds = new Dictionary<float, bool>(); // Tracks the thresholds that trigger a wave, and if they've already been triggered

    private Cleanliness GetCleanlinessMeter()
    {
        return cm;
    }
}
