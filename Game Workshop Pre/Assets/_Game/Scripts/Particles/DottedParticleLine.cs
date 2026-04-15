using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class DottedParticleLine : MonoBehaviour
{
    private ParticleSystem _particleSystem;
    private void Awake()
    {
        _particleSystem = GetComponent<ParticleSystem>();
    }

    public void UpdateLine(Vector2 endPos, int count)
    {
        _particleSystem.Clear();
        ParticleSystem.Particle[] particles = new ParticleSystem.Particle[count];

        for (int i = 0; i < count; i++)
        {
            float t = i / (float)(count - 1); // go from 0 → 1 inclusive
            Vector2 pos = Vector2.Lerp(Vector2.zero, endPos, t);

            particles[i].position = pos;
            particles[i].position = pos;
            particles[i].startSize = 0.3f;
            particles[i].startColor = Color.white;
            particles[i].remainingLifetime = float.PositiveInfinity;
            particles[i].startLifetime = float.PositiveInfinity;
        }

        _particleSystem.SetParticles(particles, particles.Length);
    }

    public void HideLine()
    {
        _particleSystem.Clear();
    }
}