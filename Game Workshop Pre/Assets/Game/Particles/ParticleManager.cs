using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using AYellowpaper.SerializedCollections;

public class ParticleManager : Singleton<ParticleManager>
{
    [SerializedDictionary("ID", "Particle System Prefab")]
    [SerializeField] private SerializedDictionary<string, ParticleSystem> _particlePrefabs;
    private Dictionary<string, IObjectPool<ParticleSystem>> _pools = new Dictionary<string, IObjectPool<ParticleSystem>>();

    protected override void Awake()
    {
        base.Awake();
        foreach (var entry in _particlePrefabs)
        {
            string key = entry.Key;
            ParticleSystem prefab = entry.Value;

            _pools[key] = new ObjectPool<ParticleSystem>(
                createFunc: () => Instantiate(prefab, transform), 
                actionOnGet: (ps) => ps.gameObject.SetActive(true), 
                actionOnRelease: (ps) => ps.gameObject.SetActive(false),
                actionOnDestroy: (ps) => Destroy(ps.gameObject),
                collectionCheck: false,
                defaultCapacity: 10,
                maxSize: 20
            );
        }
    }

    public void Play(string pCode, Vector3 position, Quaternion? rotation = null, Color? color = null, Transform parent = null, float force = 1f)
    {
        if (_pools.TryGetValue(pCode, out var pool))
        {
            var ps = pool.Get();
            if (parent != null)ps.transform.SetParent(parent);
            ps.transform.position = position;
            ps.transform.rotation = rotation ?? Quaternion.identity;

            var main = ps.main;
            var emission = ps.emission;
            if (color.HasValue)
                main.startColor = color.Value;

            if (_particlePrefabs.TryGetValue(pCode, out var prefab))
            {
                main.startSpeed = prefab.main.startSpeed.constant * force;
                main.startLifetime = prefab.main.startLifetime.constant * force;
            }

            if (emission.burstCount > 0)
            {
                ParticleSystem.Burst burst = emission.GetBurst(0);
                short newCount = (short)Mathf.Max(1, _particlePrefabs[pCode].emission.GetBurst(0).count.constant * force);
                burst.count = newCount;
                emission.SetBurst(0, burst);
            }

            ps.Play();
            StartCoroutine(ReturnToPoolAfterFinished(pCode, ps));
        }
    }

    private IEnumerator ReturnToPoolAfterFinished(string pCode, ParticleSystem ps)
    {
        yield return new WaitForSeconds(ps.main.duration + ps.main.startLifetime.constantMax);
        
        if (_pools.ContainsKey(pCode))
        {
            _pools[pCode].Release(ps);
        }
    }
}