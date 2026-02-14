using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwarmSpawner : MonoBehaviour
{
    [SerializeField] CloseMeleeEnemy _swarmPrefab;
    [SerializeField] float _spawnInterval = 5f;
    private float _nextSpawnTime = 0f;
    private Room _parentRoom;
    private bool _canSpawn;

    // Start is called before the first frame update
    void Start()
    {
        _parentRoom = GetComponentInParent<Room>();
        _canSpawn = true;
    }

    // Update is called once per frame
    void Update()
    {
        //if (Time.time >= _nextSpawnTime && isActiveAndEnabled && _swarmPrefab.Size - _parentRoom.FreeTrashAmount > 0 && _canSpawn)
        //{
        //    Instantiate(_swarmPrefab, transform.position, Quaternion.identity);
        //    _parentRoom.AddCleanableToRoom(_swarmPrefab.GetComponent<ICleanable>());
        //    _nextSpawnTime = Time.time + _spawnInterval;
        //}
    }

}
