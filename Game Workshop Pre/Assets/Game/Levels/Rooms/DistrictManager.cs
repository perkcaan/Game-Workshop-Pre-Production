using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Tilemaps;

public class DistrictManager : StaticInstance<DistrictManager>
{
    [SerializeField, Range(HeatMechanic.LOWEST_HEAT_VALUE, HeatMechanic.HIGHEST_HEAT_VALUE)] private int _baseTemperature;
    public int Temperature { get { return _baseTemperature; } }

    [SerializeField] private Tilemap _roomTilemap;
    private List<Room> _rooms = new List<Room>();

    public bool AreGatesUp { get; private set; }
    public bool inRoom;
    public Action<bool> OnGateFlip;

    // Rooms the player is in
    private List<Room> _focusedRooms = new List<Room>(); // focused rooms is a list since the player could be in multiple touching rooms.
    public Room FocusedRoom { get { return _focusedRooms.Count > 0 ? _focusedRooms[0] : null; } }


    [ContextMenu("Generate Rooms")]
    private void GenerateRooms()
    {
        RoomPolygonGenerator.GeneratePolygonColliders(transform, _roomTilemap);
    }

    public float GetCleanCompletion()
    {
        int completeRooms = 0;
        int totalTrashRooms = 0;
        foreach (Room room in _rooms)
        {
            if (!room.IsTrashRoom) continue;
            totalTrashRooms++;
            if (room.Cleanliness >= 1f)
            {
                completeRooms++;
            }
        }

        return totalTrashRooms == 0 ? 1f : Mathf.Clamp01(completeRooms/ (float)totalTrashRooms);
    }

    public void PlayerEnterRoom(Room room)
    {
        if (!_focusedRooms.Contains(room))
        {
            _focusedRooms.Add(room);
            
        }
    }
    public void PlayerExitRoom(Room room)
    {
        if (_focusedRooms.Contains(room))
        {
            _focusedRooms.Remove(room);
            
        }
    }

    private void Start()
    {
        _rooms = new List<Room>(FindObjectsOfType<Room>());
    }

    private void Update()
    {
        CheckGateStatus();
        
    }


    private void CheckGateStatus()
    {
        bool gateStatus = true;
        if (FocusedRoom == null || FocusedRoom.Cleanliness >= 1f)
        {
            gateStatus = false;
        }

        if (AreGatesUp != gateStatus)
        {
            AreGatesUp = gateStatus;
            OnGateFlip?.Invoke(AreGatesUp);
        }
    }


    

}
