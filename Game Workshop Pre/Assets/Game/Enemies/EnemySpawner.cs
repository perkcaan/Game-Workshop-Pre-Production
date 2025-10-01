using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public class EnemySpawnData
{
    public BaseEnemy enemy; // Reference to the enemy prefab
    [Range(0f, 100f)] public float spawnChance; // Spawn chance
}

public class EnemySpawner : MonoBehaviour
{

    // Enemy Types
    [Header("Enemy Types")]
    [SerializeField] private List<EnemySpawnData> enemiesToSpawn; // Editable list of potential Enemies

    // Spawn Settings
    [Header("Spawn Settings")]
    public bool active; // Is spawner active?
    public float spawnInterval; // Spawn interval
    public float spawnRadius; // Spawn radius
    public float spawnAmount; // How many enemies does the spawner create at a time

    // Cleanliness Wave Manager
    [Header("Zone Wave Manager")]
    public ZoneWaveManager zwm; // Manages spawners in a specific zone (generally a room)

    // Time-Based Wave Manager
    private WaveManager wm;
    private bool wave; // Toggle wave activity
    private float wave_interval; // Duration between waves

    #region START_&_UPDATE
    private void Start()
    {
        // Connect spawner to WaveManager if the waves are time-based
        if (!zwm.cleanMeterWaves)
        {
            wm = FindFirstObjectByType<WaveManager>();
            wave_interval = wm.GetGlobalWaveInterval();

            // Start time-based wave coroutines
            StartCoroutine(WaveCycleTimer());
            StartCoroutine(EnemySpawnTimer());
        }
        
    }

    void Update()
    {

        if (zwm.cleanMeterWaves)
        {
            ReleaseWave();
        }

        AdjustSpawnChance(); // Keeps the sum of the spawn rates at 100% 
    }
    #endregion

    #region ENEMY_SPAWNING
    private void SpawnEnemies()
    {

        // Spawns a the specified amount of enemies

        for (int i = 0; i < spawnAmount; i++)
        {
            BaseEnemy newEnemy = GetRandomEnemy();

            Vector2 spawnPos = FindSpawnPos();

            Instantiate(newEnemy, spawnPos, Quaternion.identity);
            Debug.Log(newEnemy.name + " spawned");
        }

    }

    private Vector2 FindSpawnPos()
    {

        // Locates open spawn position within dedicated radius
        for (int i = 0; i < 20; i++)
        {
            Vector2 randomOffset = Random.insideUnitCircle * spawnRadius;
            Vector2 foundPos = (Vector2)transform.position + randomOffset;

            RaycastHit2D hit = Physics2D.Raycast(transform.position, randomOffset.normalized, spawnRadius);

            if (hit.collider == null || !hit.collider.CompareTag("Wall")) // Do not spawn enemies in areas of collision, chiefly Walls
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
        // then selects that enemy for spawning
        foreach (var enemyInfo in enemiesToSpawn)
        {
            if (randNum < enemyInfo.spawnChance)
                return enemyInfo.enemy;
            randNum -= enemyInfo.spawnChance;
        }

        return enemiesToSpawn[0].enemy;
    }


    private void ReleaseWave()
    {
        // Release a wave for every threshold that is passed
        foreach (var wt in zwm.waveThresholds)
        {
            if (zwm.cm.percentClean < wt.Key && wt.Value.Equals(false))
            {
                SpawnEnemies();
                wt.Value.Equals(true);
            }
        }
        
    }

    #endregion

    #region TIME_WAVE_TIMERS

    IEnumerator EnemySpawnTimer()
    {
        while (true)
        {
            if (active && wave)
                SpawnEnemies();

            yield return new WaitForSeconds(spawnInterval);
        }
    }

    IEnumerator WaveCycleTimer()
    {
        while (true)
        {
            // Start wave
            wave = true;
            float waveDuration = wave_interval; // Wave duration inherits from global wave interval
            Debug.Log("Wave Start (Timer)");
            yield return new WaitForSeconds(waveDuration);

            // End wave
            wave = false;
            Debug.Log("Wave End (Timer)");
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
        var enemyInfo = enemiesToSpawn.Find(eInfo => eInfo.enemy == enemy);
        if (enemyInfo == null)
        {
            return;
        }

        enemyInfo.spawnChance = Mathf.Clamp(newSpawnChance, 0f, 100f); // Sets new spawn rate
        AdjustSpawnChance(); // Readjusts all spawn rates
    }
    #endregion
}
