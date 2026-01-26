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
    

    private void Start()
    {
        
    }
    public void Play(string sCode, Transform position)
    {
        if (_sounds.TryGetValue(sCode, out sInstance))
        {

            
            //sInstance.EventInstance.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(position));
            
            //RuntimeManager.PlayOneShotAttached(sCode, position.gameObject);
            sInstance.Play();
            




        }
        else
        {
            //Unity may throw an error before this ever triggers, but just in case
            Debug.LogWarning($"AudioManager: FMOD key '{sCode}' not found.");
        }
    }

    public void Stop(string sCode)
    {
        if (_sounds.TryGetValue(sCode, out sInstance))
        {
            sInstance.Stop();
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
                //If you see this try checking your spelling on either the parameter name or the sound code
                Debug.LogError("That shit didn't work");
            }
        }
    }
    public void PlayOnInstance(GameObject obj, string sCode)
    {
        sInstance = _sounds[sCode];
        if (obj == null) return;
        StudioEventEmitter[] emitters = obj.GetComponents<StudioEventEmitter>();

        foreach (StudioEventEmitter emitter in emitters) 
        {
            if (emitter.EventReference.Equals(sInstance.EventReference))
            {
                
                emitter.Play();
            }
            else
            {
                
            }
        }

        
    }
    public void PlayOnInstance(GameObject obj, string sCode)
    {
        sInstance = _sounds[sCode];
        if (obj == null) return;
        StudioEventEmitter[] emitters = obj.GetComponents<StudioEventEmitter>();

        foreach (StudioEventEmitter emitter in emitters)
        {
            if (emitter.EventReference.Equals(sInstance.EventReference))
            {

                emitter.Play();
            }
            else
            {

            }
        }


    }

}