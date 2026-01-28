using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHive : TrashPile
{
    [Header("Enemy Hive")]
    [SerializeField] EnemyBase _enemyPrefab;
    [SerializeField] private bool _canSpawn = true;
    [SerializeField] float _spawnInterval = 5f;
    [SerializeField] private float _maxRandomTimeOffset = 1f;
    [SerializeField] private int _maxAliveEnemies = 4;
    [SerializeField] private float _spawnRadius = 0.4f;
    [SerializeField] private float _enemyColliderRadius = 0.3f;
    [SerializeField] private LayerMask _invalidSpawnLayers = (1 << 12) | (1 << 14); // 12 and 14 are intended to be Lava and Wall;
    private float _currentTime;
    private int _currentAliveEnemies = 0;

    private void Start()
    {
        _currentTime += Random.Range(0,_maxRandomTimeOffset);
    }

    void Update()
    {
        _currentTime += Time.deltaTime;
        if (_currentTime >= _spawnInterval)
        {
            _currentTime = 0f;
            if (_canSpawn &&
            isActiveAndEnabled &&
            _enemyPrefab.Size <= _parentRoom.FreeTrashAmount &&
            _currentAliveEnemies < _maxAliveEnemies) SpawnEnemy();
        }
    }

    private void SpawnEnemy()
    {
        EnemyBase newEnemy = Instantiate(_enemyPrefab, PickNewPoint(), Quaternion.identity);
        _parentRoom.AddCleanableToRoom(newEnemy);
        _currentAliveEnemies++;
        newEnemy.OnDestroy += OnEnemyDestroy;
    }


    private void OnEnemyDestroy()
    {
        _currentAliveEnemies--;
    }

    private Vector2 PickNewPoint()
    {
        // chooses random point within a radius from the start position. Try up to 10 times.
        int tries = 0;
        while (tries < 10)
        {
            tries++;
            Vector2 randomOffset = Random.insideUnitCircle * _spawnRadius;
            Vector2 attemptedPoint = (Vector2) transform.position + randomOffset;
            if (Physics2D.OverlapCircle(attemptedPoint, _enemyColliderRadius, _invalidSpawnLayers) != null) {
                continue;
            }
            return attemptedPoint;
        }
        // Failed. Reset and try again next time.
        Debug.LogWarning("EnemyHive: " + name + "repeatedly failed to spawn an enemy. Make sure they have room to spawn.");
        return transform.position;
    }

}