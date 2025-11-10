using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AYellowpaper.SerializedCollections;

public class ParticleManager : Singleton<ParticleManager>
{
    [SerializedDictionary("ID", "Particle System")]
    [SerializeField] private SerializedDictionary<string, ParticleSystem> _particles;
    private ParticleSystem psInstance;

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

    public void Modify(string pCode,float duration,int maxParticles, int speed)
    {
        psInstance = _particles[pCode];
        var MainModule = psInstance.main;
        MainModule.duration += duration;
        MainModule.maxParticles += maxParticles;
        MainModule.startSpeed = speed;
       
    }
    
}
