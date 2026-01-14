using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwarmSpawner : MonoBehaviour
{
    [SerializeField] GameObject _swarmPrefab;
    [SerializeField] float _spawnInterval = 5f;
    private float _nextSpawnTime = 0f;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time >= _nextSpawnTime && this.isActiveAndEnabled)
        {
            Instantiate(_swarmPrefab, transform.position, Quaternion.identity);
            _nextSpawnTime = Time.time + _spawnInterval;
        }
    }

}
