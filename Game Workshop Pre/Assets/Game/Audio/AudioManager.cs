using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using AYellowpaper.SerializedCollections;
using static UnityEngine.ParticleSystem;
using UnityEngine.Rendering;
using UnityEngine.UI;



public class AudioManager : Singleton<AudioManager>
{
    

    [SerializedDictionary("Sound Code", "FMODEVent")]
    [SerializeField] private AYellowpaper.SerializedCollections.SerializedDictionary<string, EventReference> _sounds;

    [SerializedDictionary("Bus Name", "Bus Path")]
    [SerializeField] private AYellowpaper.SerializedCollections.SerializedDictionary<string, Bus> _buses;
    private EventReference eventRef;
    private EventInstance parent = new EventInstance();

    public Bus _masterBus;
    public Bus _sfxBus;
    public Bus _musicBus;

    

    


    public void Start()
    {
        _masterBus = RuntimeManager.GetBus("bus:/");
        _musicBus = RuntimeManager.GetBus("bus:/SFX");
        _sfxBus = RuntimeManager.GetBus("bus:/MUSIC");

        _buses["Master"] = _masterBus;
        _buses["Music"] = _musicBus;
        _buses["SFX"] = _sfxBus;

        
    }
    public void Play(string sCode, Transform position)
    {
        //bool instance = false;
        
        if (!_sounds.TryGetValue(sCode, out EventReference eventRef))
        {
            Debug.LogError($"AudioManager: '{sCode}' not found");
            return;
        }

        RuntimeManager.PlayOneShot(eventRef, position.position);

        //if (!instance)
        //{
        //    parent = new EventInstance();
        //    parent = RuntimeManager.CreateInstance(eventRef);
        //    parent.start();
        //    parent.set3DAttributes(RuntimeUtils.To3DAttributes(position));
        //    instance = true;
        //    return;
        //}
        //else
        //{
        //    parent.start();
        //    return;
        //}


        //RuntimeManager.PlayOneShot(eventRef, position.position);

    }

    public void Stop(GameObject obj, string sCode)
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
                emitter.EventInstance.Equals(parent);
                emitter.Stop();
                Debug.Log($"Stopped sound '{sCode}' on {obj.name}");
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
                emitter.EventInstance.Equals(parent);
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
                Debug.Log($"Playing sound '{sCode}' on {obj.name}");

                return;
            }
            //else
            //{
            //    
            //    StudioEventEmitter newEmitter = obj.AddComponent<StudioEventEmitter>();
            //    newEmitter.EventReference = eventRef;
            //    newEmitter.Play();
            //    return;
            //}
        }

        
    }

    public void ModifyBusVolume(Slider busSlider, string attachedBus)
    {

        if (_buses.TryGetValue(attachedBus, out Bus currentBus))
        {
            _buses[attachedBus] = currentBus;
            currentBus.setVolume(busSlider.value);
        }
        
        
        
        


    }
}

