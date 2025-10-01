using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnemySpawnData
{
    public BaseEnemy enemy; // Reference to the enemy prefab
    [Range(0f, 5f)] // Max of 5 Enemy Types
    public float spawnChance; // Spawn chance
}

public class EnemySpawner : MonoBehaviour
{
    [Header("Enemy Types")]
    [SerializeField] private List<EnemySpawnData> enemiesToSpawn; // Editable list of potential Enemies

    [Header("Spawn Settings")]
    public bool active; // Is spawner active?
    public float spawnInterval; // Spawn interval
    public float spawnRadius; // Spawn radius

    // Wave Manager
    private WaveManager wm;
    private bool wave; // Toggle wave activity
    private float wave_interval; // Duration between waves

    #region START_&_UPDATE
    private void Start()
    {
        // Connect spawner to WaveManager
        wm = FindFirstObjectByType<WaveManager>();
        wave_interval = wm.GetGlobalWaveInterval();

        // Start coroutines for wave timing and enemy spawning
        StartCoroutine(WaveCycle());
        StartCoroutine(EnemySpawnTimer());
    }

    void Update()
    {
        AdjustSpawnChance();
    }
    #endregion

    #region ENEMY_SPAWNING
    private void SpawnEnemies()
    {
        BaseEnemy newEnemy = GetRandomEnemy();

        Vector2 spawnPos = FindSpawnPos(); // Locates open spawn position within dedicated radius

        Instantiate(newEnemy, spawnPos, Quaternion.identity);
        Debug.Log(newEnemy.name + " spawned");
    }

    private Vector2 FindSpawnPos()
    {
        for (int i = 0; i < 20; i++)
        {
            Vector2 randomOffset = Random.insideUnitCircle * spawnRadius;
            Vector2 foundPos = (Vector2)transform.position + randomOffset;

            RaycastHit2D hit = Physics2D.Raycast(transform.position, randomOffset.normalized, spawnRadius);

            if (hit.collider == null || !hit.collider.CompareTag("Wall"))
            {
                return foundPos;
            }
        }

        return Vector2.zero;
    }

    private BaseEnemy GetRandomEnemy()
    {
        float sum = 0f;
        foreach (var enemyInfo in enemiesToSpawn)
        {
            sum += enemyInfo.spawnChance;
        }

        float randNum = Random.value * sum;

        // Checks which enemy's spawn chance the randNum falls under
        foreach (var enemyInfo in enemiesToSpawn)
        {
            if (randNum < enemyInfo.spawnChance)
                return enemyInfo.enemy;
            randNum -= enemyInfo.spawnChance;
        }

        return enemiesToSpawn[0].enemy;
    }
    #endregion

    #region TIMERS

    IEnumerator EnemySpawnTimer()
    {
        while (true)
        {
            if (active && wave)
                SpawnEnemies();

            yield return new WaitForSeconds(spawnInterval);
        }
    }

    IEnumerator WaveCycle()
    {
        while (true)
        {
            // Start wave
            wave = true;
            float waveDuration = wave_interval; // Wave duration inherits from global wave interval
            Debug.Log("Wave start");
            yield return new WaitForSeconds(waveDuration);

            // End wave
            wave = false;
            Debug.Log("Wave end.");
            yield return new WaitForSeconds(wave_interval); // Time between waves
        }
    }
    #endregion

    #region SPAWN_CHANCE
    private void AdjustSpawnChance()
    {
        // Checks if collective spawn chance = 100
        float sum = 0f;
        foreach (var enemyInfo in enemiesToSpawn)
            sum += enemyInfo.spawnChance;
        if (Mathf.Approximately(sum, 100f))
            return;

        // If not, adjusts all enemy spawn chances proportional to others
        float diff = 100f - sum;

        foreach (var enemyInfo in enemiesToSpawn)
        {
            enemyInfo.spawnChance += (enemyInfo.spawnChance / sum) * diff;
        }
    }

    public void SetSpawnChance(BaseEnemy enemy, float newSpawnChance)
    {
        var data = enemiesToSpawn.Find(d => d.enemy == enemy);
        if (data == null) return;

        data.spawnChance = Mathf.Clamp(newSpawnChance, 0f, 100f);
        AdjustSpawnChance();
    }
    #endregion
}
