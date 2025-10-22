using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AYellowpaper.SerializedCollections;

public class ParticleManager : Singleton<ParticleManager>
{
    [SerializedDictionary("ID", "Particle System")]
    [SerializeField] private SerializedDictionary<string, ParticleSystem> _particles;

    public void Play(string pCode,Vector3 position)
    {
        if (_particles.ContainsKey(pCode))
        {
            ParticleSystem psInstance = Instantiate(_particles[pCode], position, Quaternion.identity, null);
            psInstance.Play();
        }
    }

    public void Play(string pCode, Vector3 position, Quaternion rotation, Transform parent = null)
    {
        if (_particles.ContainsKey(pCode))
        {
            ParticleSystem psInstance = Instantiate(_particles[pCode], position, rotation, parent);
            psInstance.Play();
        }
    }
    
}
