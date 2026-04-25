using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Cinemachine;
using System;

public class Quake : MonoBehaviour
{

    // Spawn Count
    [SerializeField] private int spawnAmount;

    // Is quake allowed?
    [SerializeField] private bool _canQuake;
    public bool CanQuake
    {
        get { return _canQuake; }
        set { _canQuake = value; }
    }

    // Bool to check if quake can activate
    [SerializeField] private bool _quakeTrigger;

    public bool QuakeTrigger // Trigger to activate Quake
    {
        get { return _quakeTrigger; }
        set { _quakeTrigger = value; }
    }

    // This Room
    private Room _room;

    // Trash Prefabs
    [SerializeField] private Trash[] _earthquakeTrashList;

    // Quake FX
    private CinemachineImpulseSource _impulseSource;    
    private AudioSource _source;
    [SerializeField] private AudioClip _rumbleSound;
    
    void Awake()
    {
        _impulseSource = GetComponentInParent<CinemachineImpulseSource>();
        _source = GetComponent<AudioSource>();

        if (!gameObject.TryGetComponent<Room>(out _room))
        {
            Debug.LogError("Room component not found on GameObject");
        }

    }
    
    void Update()
    {
        if (_canQuake) {
            if (_room == null){
                Debug.LogWarning("Room Is Null");
                return;
            }

                Debug.Log(_room.totalMinSizeToAbsorb);
                Debug.Log(_room.RoomCurrentTrashAmount - _room.totalMinSizeToAbsorb);
            // See if quake should be enabled
            if (_room.totalMinSizeToAbsorb >= (_room.RoomCurrentTrashAmount - _room.totalMinSizeToAbsorb))
            {
                QuakeTrigger = true;

                Debug.LogWarning("Room Can Quake");
            }
            else
            {
                Debug.LogWarning("Room Cannot Quake");
            }

            // If quake can be enable, execute the remaining logic
            if (QuakeTrigger)
            {
                QuakeTrigger = false;

                ShakeScreen();

                for (int i = 0; i < spawnAmount; i++)
                {
                    SpawnTrashObject();
                }
            }

            // TEST CODE - Uncomment the below code and 
            // comment out the above code 
            // to allow Quake at will with zero conditions
            // aside from manually activating the boolean
            
            /*if (_room != null && canQuake)
            {
                canQuake = false;
                ShakeScreen();

                for (int i = 0; i < spawnAmount; i++)
                {
                    SpawnTrashObject();
                }

            }*/
        }

    }


    private void SpawnTrashObject()
    {
        // Find all trash objects of the appropriate size
        List<Trash> validTrash = new List<Trash>();
        foreach (Trash t in _earthquakeTrashList)
        {
            
            if (t.Size < _room.FreeTrashAmount)
            {
                validTrash.Add(t);
            }
        
            // TEST CODE - Uncomment the below code and 
            // comment out the above code 
            // to allow Quake at will with zero conditions
            //validTrash.Add(t);
        }

        // If no valid trash exists, throw an error
        if (validTrash.Count == 0)
        {
            Debug.LogError("No valid trash found for the room. FreeTrashAmount: " 
                        + _room.FreeTrashAmount);
            return;
        }

        // Pick a random valid trash
        int randomIndex = UnityEngine.Random.Range(0, validTrash.Count);
        Trash trashObjectToSpawn = validTrash[randomIndex];

        // Get spawn position
        UnityEngine.Vector3 spawnPoint = FindFreePoint(trashObjectToSpawn.GetComponent<CircleCollider2D>().radius);

        // Instantiate the trash
        Instantiate(trashObjectToSpawn, spawnPoint, UnityEngine.Quaternion.identity);

        Debug.Log("Spawning " + trashObjectToSpawn);
    }

    private void ShakeScreen()
    {
        CamShakeManager.instance.CameraShake(_impulseSource);

        if (_rumbleSound != null && _source != null)
        {
            _source.PlayOneShot(_rumbleSound);
        }

    }

    // Finds free space to spawn trash
    private UnityEngine.Vector3 FindFreePoint(float trashObjectRadius)
    {
        PolygonCollider2D roomBounds = _room.GetComponent<PolygonCollider2D>();

        // Obstruction mask
        int obstructionMask = LayerMask.GetMask("Wall", "Trash", "Player", "Lava");

        for (int attempts = 0; attempts < 50; attempts++)
        {
            float randX = UnityEngine.Random.Range(roomBounds.bounds.min.x, roomBounds.bounds.max.x);
            float randY = UnityEngine.Random.Range(roomBounds.bounds.min.y, roomBounds.bounds.max.y);

            // Find random point
            UnityEngine.Vector3 randomPoint = new UnityEngine.Vector3(randX, randY, 0f);

            if (!roomBounds.OverlapPoint(randomPoint))
                continue;

            // Check for obstructions - Overlap
            Collider2D overlap = Physics2D.OverlapCircle(randomPoint, trashObjectRadius, obstructionMask);
            if (overlap != null)
                continue;

            // check for obstructions - RayCast
            UnityEngine.Vector2 origin = roomBounds.bounds.center;
            UnityEngine.Vector2 direction = ((UnityEngine.Vector2)randomPoint - origin).normalized;
            float distance = UnityEngine.Vector2.Distance(origin, randomPoint);
            RaycastHit2D hit = Physics2D.Raycast(origin, direction, distance, obstructionMask);

            if (hit.collider != null)
                continue;

            return randomPoint;
        }

        Debug.LogWarning("No free point found after 50 attempts, spawning at center");
        return roomBounds.bounds.center;
    }


}
