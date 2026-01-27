using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using AYellowpaper.SerializedCollections;
using static UnityEngine.ParticleSystem;
using UnityEngine.Rendering;



public class AudioManager : Singleton<AudioManager>
{

    [SerializedDictionary("Sound Code", "FMODEVent")]
    [SerializeField] private AYellowpaper.SerializedCollections.SerializedDictionary<string, EventReference> _sounds;
    private EventReference eventRef;


    //private void Awake()
    //{
    //    base.Awake(); 
    //    Debug.Log($"AudioManager initialized with {_sounds.Count} sounds");
    //}
    public void Play(string sCode, Transform position)
    {
        if (!_sounds.TryGetValue(sCode, out EventReference eventRef))
        {
            Debug.LogError($"AudioManager: '{sCode}' not found");
            return;
        }

        RuntimeManager.PlayOneShot(eventRef, position.position);
    }

    public void Stop(GameObject obj, string sCode)
    {
        if (!_sounds.TryGetValue(sCode, out EventReference eventRef))
        {
            Debug.LogError($"AudioManager: '{sCode}' not found");
            return;
        }

        foreach (var emitter in obj.GetComponents<StudioEventEmitter>())
        {
            if (emitter.EventReference.Equals(eventRef))
            {
                emitter.Stop();
                return;
            }
        }
    }

    public void ModifyGlobalParameter(string param, float value)
    {
        RuntimeManager.StudioSystem.setParameterByName(param, value);
    }

    public void ModifyParameter(GameObject obj, string sCode, string param, float value)
    {
        if (!_sounds.TryGetValue(sCode, out EventReference eventRef))
        {
            Debug.LogError($"AudioManager: '{sCode}' not found");
            return;
        }

        foreach (StudioEventEmitter emitter in obj.GetComponents<StudioEventEmitter>())
        {
            if (emitter.EventReference.Equals(eventRef))
            {
                emitter.EventInstance.setParameterByName(param, value);
                return;
            }
        }
    }
    public void PlayOnInstance(GameObject obj, string sCode)
    {
        if (obj == null) return;

        if (!_sounds.TryGetValue(sCode, out EventReference eventRef))
        {
            Debug.LogError($"AudioManager: '{sCode}' not found");
            return;
        }

        StudioEventEmitter[] emitters = obj.GetComponents<StudioEventEmitter>();
        foreach (StudioEventEmitter emitter in emitters)
        {
            if (emitter.EventReference.Equals(eventRef))
            {
                emitter.Play();
                return;
            }
        }

        
    }
}

