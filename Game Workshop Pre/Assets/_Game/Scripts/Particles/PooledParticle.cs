using UnityEngine;
using UnityEngine.Pool;

public class PooledParticle : MonoBehaviour
{
    private IObjectPool<ParticleSystem> _pool;
    private ParticleSystem _ps;

    public void Initialize(IObjectPool<ParticleSystem> pool)
    {
        _pool = pool;
        _ps = GetComponent<ParticleSystem>();
        
        var main = _ps.main;
        main.stopAction = ParticleSystemStopAction.Callback;
    }

    private void OnParticleSystemStopped()
    {
        transform.SetParent(ParticleManager.Instance.transform);
        _pool.Release(_ps);
    }
}