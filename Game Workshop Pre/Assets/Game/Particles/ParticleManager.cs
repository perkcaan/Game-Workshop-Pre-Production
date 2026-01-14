using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AYellowpaper.SerializedCollections;

public class ParticleManager : Singleton<ParticleManager>
{
    [SerializedDictionary("ID", "Particle System")]
    [SerializeField] private SerializedDictionary<string, ParticleSystem> _particles;
    private ParticleSystem psInstance;
    public bool modified;

    private void Start()
    {
        modified = false;
    }
    public void Play(string pCode,Vector3 position)
    {
        if (_particles.ContainsKey(pCode))
        {
            psInstance = Instantiate(_particles[pCode], position, Quaternion.identity, null);
            psInstance.Play();
        }
    }

    public void Play(string pCode, Vector3 position, Quaternion rotation,Color color, Transform parent = null)
    {
        if (_particles.ContainsKey(pCode))
        {
            psInstance = Instantiate(_particles[pCode], position, rotation, parent);
            psInstance.startColor = color;
            psInstance.Play();
        }
    }

    public void Modify(string pCode,float duration,int maxParticles, int speed, string operation)
    {
        psInstance = _particles[pCode];
         
        var MainModule = psInstance.main;
        float initialDuration = MainModule.duration;
        int initialParticles = MainModule.maxParticles;
        

        if (!modified)
        {
            switch (operation)
            {
                case "Subtract":
                    MainModule.duration -= duration;
                    MainModule.maxParticles -= maxParticles;
                    MainModule.startSpeed = speed;
                    //Debug.Log("Subtracted from Particles");
                    break;

                case "Add":
                    MainModule.duration += duration;
                    MainModule.maxParticles += maxParticles;
                    MainModule.startSpeed = speed;
                    //Debug.Log("Added to Particles");
                    break;

                case "Multiply":
                    MainModule.duration *= duration;
                    MainModule.maxParticles *= maxParticles;
                    MainModule.startSpeed = speed;
                    break;

                case "Divide":
                    MainModule.duration /= duration;
                    MainModule.maxParticles /= maxParticles;
                    MainModule.startSpeed = speed;
                    break;

                case "Restore":
                    MainModule.duration = initialDuration;
                    MainModule.maxParticles = initialParticles;
                    MainModule.startSpeed = speed;
                    Debug.Log("Restored");
                    break;
            }
        }

        


    }
    
}
