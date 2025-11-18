using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using AYellowpaper.SerializedCollections;
using static UnityEngine.ParticleSystem;


public class AudioManager : Singleton<AudioManager>
{
    
    [SerializedDictionary("ID", "FMODEmitter")]
    [SerializeField] private SerializedDictionary<string, StudioEventEmitter> _sounds;
    private StudioEventEmitter sInstance;


    public void Play(string sCode, Vector3 position)
    {
        if (_sounds.TryGetValue(sCode, out sInstance))
        {
            
            
            sInstance.Play();
        }
        else
        {
            Debug.LogWarning($"AudioManager: FMOD key '{sCode}' not found.");
        }
    }


    public void ModifyParameter(string sCode,string param,float value)
    {
        sInstance = _sounds[sCode];
        if (sInstance != null)
        {
            if (_sounds.TryGetValue(sCode ,out sInstance))
            {
                
                sInstance.EventInstance.setParameterByName(param, value);

            }
            else
            {
                Debug.LogError("That shit didn't work");
            }
        }
    }

}
