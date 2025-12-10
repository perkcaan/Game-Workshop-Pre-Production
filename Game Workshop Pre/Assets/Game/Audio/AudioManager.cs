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

    public void Stop(string sCode)
    {
        if (_sounds.TryGetValue(sCode, out sInstance))
        {
            sInstance.EventInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        }
        else
        {
            Debug.LogWarning($"AudioManager: FMOD key '{sCode}' not found.");
        }
    }


    public void ModifyParameter(string sCode, string param, float value, string Itype)
    {
        sInstance = _sounds[sCode];
        if (sInstance != null)
        {
            if (_sounds.TryGetValue(sCode, out sInstance))
            {

                switch (Itype)
                {
                    case "Global":
                        RuntimeManager.StudioSystem.setParameterByName(param, value);
                        break;
                    case "Local":
                        sInstance.EventInstance.setParameterByName(param, value);
                        break;
                }





            }
            else
            {
                Debug.LogError("That shit didn't work");
            }
        }
    }

}