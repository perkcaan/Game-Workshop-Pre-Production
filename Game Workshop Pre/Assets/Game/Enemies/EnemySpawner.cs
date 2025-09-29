using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{

    [SerializeField] private BaseEnemy[] enemyTypes; // Types of enemies instantiated by the spawner

    public float interval; // Spawn interval

    public float spawnRadius;


    private void Start()
    {
        StartCoroutine(EnemySpawnTimer(interval));
    }

    void Update()
    {
        SpawnEnemies();
    }

    private void SpawnEnemies()
    {
        // Pick random enemy
        int randSelect = Random.Range(0, (enemyTypes.Length - 1));

        // Find spawn position
        Vector2 spawnPos = FindSpawnPos();

        // Spawn enemy at spawn pos
        BaseEnemy newEnemy = Instantiate(enemyTypes[randSelect], spawnPos, Quaternion.identity);

        Debug.Log("Enemy Spawned");

    }

    private Vector2 FindSpawnPos()
    {

        for (int i = 0; i < 20; i++) // Will attempt twenty times
        {

            // Fin random position in radius
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

    IEnumerator EnemySpawnTimer(float time)
    {
        while (true)
        {
            SpawnEnemies();
            yield return new WaitForSeconds(time);
        }
        
    }
}
