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

        // Fail if already spawning
        if (_isSpawningWeb)
        {
            onComplete?.Invoke(false);
            yield break;
        }

        _isSpawningWeb = true;

        // Spawn web object
        Instantiate(_webTrapPrefab, transform.position, Quaternion.identity);
        onComplete?.Invoke(true);
        

        _isSpawningWeb = false;
    }

    protected override void ForceDisableHitboxes()
    {
        //unneeded
    }

}