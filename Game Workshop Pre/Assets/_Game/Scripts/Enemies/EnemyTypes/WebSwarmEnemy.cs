using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WebSwarmEnemy : EnemyBase
{
    
    [Header("Web Trap Properties")]

    [SerializeField] private SpiderWebTrap _webTrapPrefab;
    private bool _isSpawningWeb = false;

    [SerializeField] private LayerMask _playerLayer;

    protected override void OnStart()
    {
        
    }

    protected override void OnUpdate()
    {

    }


    // Spawn Web
    public IEnumerator SpawnWeb(Action<bool> onComplete)
    {
        if (_isSpawningWeb)
        {
            onComplete?.Invoke(false);
            yield break;
        }

        _animator.SetTrigger("StartAttack");
        _animator.SetTrigger("DoAttack");
        _isSpawningWeb = true;

        // Spawn web object
        yield return new WaitForSeconds(0.5f);

        Instantiate(_webTrapPrefab, transform.position, Quaternion.identity);
        _animator.SetTrigger("ReturnToIdle");
        _isSpawningWeb = false;
        onComplete?.Invoke(true);
    }

    protected override void ForceCancelAction()
    {
        //unneeded
    }

    protected override void ModifySwipe(ref EnemySwipeData data)
    {
        data.IsVulnerable = true;
    }

    protected override void ModifyPoke(ref EnemyPokeData data)
    {
        data.IsVulnerable = true;
    }

    protected override void ModifyAbsorb(ref EnemyAbsorbData data)
    {
        data.CanAbsorb = true;
    }

}