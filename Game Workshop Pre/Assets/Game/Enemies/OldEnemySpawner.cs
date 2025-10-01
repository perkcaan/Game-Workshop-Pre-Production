using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OldEnemySpawner : MonoBehaviour
{

    [SerializeField] private BaseEnemy[] enemyTypes; // Types of enemies instantiated by the spawner

    [Header("Spawn Settings")]

    public bool active; // Is spawner active?

    public float spawnInterval; // Spawn interval

    public float spawnRadius;

    private Dictionary<BaseEnemy, float> eSpawnChance = new Dictionary<BaseEnemy, float>(); // Tracks enemy spawn chances

    // Wave Manager

    private WaveManager wm;

    private bool wave; // Toggle wave activity

    private float wave_interval;

    #region START_&_UPDATE

    private void Start()
    {

        // Connects spawner to WaveManager
        wm = FindFirstObjectByType<WaveManager>();
        wave_interval = wm.GetGlobalWaveInterval();

        // Assigns each enemy a base spawn chance
        foreach (BaseEnemy enemy in enemyTypes)
        {
            eSpawnChance.Add(enemy, 100 / enemyTypes.Length);
        }


        StartCoroutine(WaveTimer(wave_interval));
        StartCoroutine(EnemySpawnTimer(spawnInterval));
    }

    void Update()
    {

        AdjustSpawnChance();

        if (wave && active)
        {
            SpawnEnemies();
        }

    }

    #endregion

    #region ENEMY_SPAWNING
    private void SpawnEnemies()
    {
        BaseEnemy newEnemy = GetRandomEnemy();

        // Find spawn position
        Vector2 spawnPos = FindSpawnPos();

        Instantiate(newEnemy, spawnPos, Quaternion.identity);
        Debug.Log(newEnemy.name + " spawned");

    }

    private Vector2 FindSpawnPos()
    {

        for (int i = 0; i < 20; i++) // Will attempt twenty times
        {

            // Locate random position in the radius
            Vector2 randomOffset = Random.insideUnitCircle * spawnRadius;
            Vector2 foundPos = (Vector2)transform.position + randomOffset;

            // Check position for collision
            RaycastHit2D hit = Physics2D.Raycast(transform.position, randomOffset.normalized, spawnRadius);

            // If no collision found, return pos
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
        foreach (var chance in eSpawnChance.Values)
        {
            sum += chance;
        }

        float randNum = Random.value * sum;

        foreach (var enemySpawnChance in eSpawnChance)
        {
            if (randNum < enemySpawnChance.Value)
                return enemySpawnChance.Key;
            randNum -= enemySpawnChance.Value;
        }

        return enemyTypes[0];
    }

    #endregion

    #region TIMERS

    IEnumerator EnemySpawnTimer(float time)
    {
        while (true)
        {
            SpawnEnemies();
            yield return new WaitForSeconds(time);
        }

    }

    IEnumerator WaveTimer(float time)
    {
        while (true)
        {
            yield return new WaitForSeconds(time);
            if (wave)
            {
                wave = false;
            }
            else
            {
                wave = true;
            }
        }

    }

    #endregion

    #region SPAWN_CHANCE

    private void AdjustSpawnChance()
    {

        // Ensure the sum of all chances is 100
        float sum = 0f;
        foreach (var chance in eSpawnChance.Values)
        {
            sum += chance;
        }

        if (Mathf.Approximately(sum, 100f))
        {
            return;
        }

        float diff = 100f - sum;

        // Adjust each enemy's spawn chance proportionally to the others
        List<BaseEnemy> keys = new List<BaseEnemy>(eSpawnChance.Keys);
        for (int i = 0; i < keys.Count; i++)
        {
            BaseEnemy enemy = keys[i];
            float adjustment = (eSpawnChance[enemy] / sum) * diff;
            eSpawnChance[enemy] += adjustment;
        }

    }

    public void SetSpawnChance(BaseEnemy enemy, float newSpawnChance)
    {
        if (!eSpawnChance.ContainsKey(enemy))
        {
            return;
        }

        eSpawnChance[enemy] = Mathf.Clamp(newSpawnChance, 0f, 100f); // Set new spawn chance
        AdjustSpawnChance(); // Adjust the other enemy spawn chances
    }

    #endregion
}
