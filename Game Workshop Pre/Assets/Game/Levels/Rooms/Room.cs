using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using TMPro;
using UnityEngine;

public class Room : MonoBehaviour
{
    // Room Drawer
    public GameObject RoomDrawerPrefab; // This is a prefab of all the room's data. Make sure its a prefab.
    [HideInInspector] public GameObject ActiveRoomDrawer; // This is the instantiated RoomDrawerPrefab at runtime
    // Temperature
    [SerializeField, Range(HeatMechanic.LOWEST_HEAT_VALUE, HeatMechanic.HIGHEST_HEAT_VALUE)] private int _baseTemperature;
    public int Temperature { get { return _baseTemperature; } } // Return _baseTemperature + anything that modifies room temperature

    // Trash
    [SerializeField] private bool _isTrashRoom = true;
    public bool IsTrashRoom { get { return _isTrashRoom; } }

    private List<ICleanable> _containedCleanable = new List<ICleanable>();
    private int _roomCurrentTrashAmount = 0; // Current amount of trash in the room
    private int _roomTotalTrashCount = 0; // Starting amount of trash / Max trash allowed in room
    public float Cleanliness
    {
        get { return _roomTotalTrashCount == 0 ? 1f : 1f - Mathf.Clamp01((float) _roomCurrentTrashAmount / _roomTotalTrashCount); }
    }

    public int FreeTrashAmount
    {
        get {
            if (_isRoomCleaned) return 0; 
            return Mathf.Max(0, _roomTotalTrashCount - _roomCurrentTrashAmount); }
    }
    
    // Nearby rooms to load
    [Tooltip("Put any rooms here you want loaded while the player is in this room.")]
    [SerializeField] private List<Room> _nearbyRooms;
    public List<Room> NearbyRooms
    {
        get
        {
            if (_nearbyRooms == null) return new List<Room>();
            return _nearbyRooms;
        }
    }

    // Gates tied to this room
    [SerializeField] private List<Gate> _connectedGates;
    [SerializeField] private bool _openGatesOnClean = true; 

    // Room State
    private bool _isRoomCleaned = false;
    public bool IsRoomCleaned { get { return _isRoomCleaned; } }
    private bool _isDrawerOut = false;
    private bool _isRoomClosed = false;

    private void Awake()
    {
        _isDrawerOut = ActiveRoomDrawer != null;
        if (_isDrawerOut) 
        { 
            OnDrawerOpen();
        }
    }

    public void ActivateRoom()
    {
        //if Room is clean or drawer is out, you don't need to activate it.
        if (_isRoomCleaned || _isDrawerOut) return;
        if (RoomDrawerPrefab != null) {
            ActiveRoomDrawer = Instantiate(RoomDrawerPrefab);
        } else
        {
            ActiveRoomDrawer = new GameObject();
        }
        ActiveRoomDrawer.transform.parent = transform;
        ActiveRoomDrawer.name = name + "Drawer";
        _isDrawerOut = true;
        OnDrawerOpen();
    }

    public void DeactivateRoom()
    {
        //if drawer is in, it doesn't need to be deactivated
        if (!_isDrawerOut) return;
        if (ActiveRoomDrawer != null)
        {
            Destroy(ActiveRoomDrawer);
        }
        _isDrawerOut = false;
    }

    public void TriggerRoomClose()
    {
        if (_isRoomCleaned || _isRoomClosed) return;
        _isRoomClosed = true;
        foreach (Gate gate in _connectedGates)
        {
            gate.Close(this);
        }
    }

    // Called when player exits the room, BUT the player is safely inside of a different room.
    public void SafeExit()
    {
        if (!_isRoomClosed) return;

        _isRoomClosed = false;
        foreach (Gate gate in _connectedGates)
        {
            gate.Open(this);
        }
    }

    private void OnDrawerOpen()
    {
        // All trash is assigned its room at start 
        _containedCleanable = new List<ICleanable>();
        ICleanable[] cleanableChildren = GetComponentsInChildren<ICleanable>();
        foreach (ICleanable cleanable in cleanableChildren)
        {
            cleanable.SetRoom(this);
            _containedCleanable.Add(cleanable);
        }
        UpdateRoomCleanliness();
        _roomTotalTrashCount = _roomCurrentTrashAmount;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.gameObject.TryGetComponent(out PlayerMovementController player))
        {
            if (DistrictManager.Instance != null) DistrictManager.Instance.PlayerEnterRoom(this);
        }

        if (collision.gameObject.TryGetComponent(out HeatMechanic heatable))
        {
            heatable.EnterRoom(this);
        }

    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent(out PlayerMovementController player))
        {
            if (DistrictManager.Instance != null) DistrictManager.Instance.PlayerExitRoom(this);
        }

        if (collision.gameObject.TryGetComponent(out HeatMechanic heatable))
        {
            heatable.ExitRoom(this);
        }
    }

    public void AddCleanableToRoom(ICleanable cleanable)
    {
        cleanable.SetRoom(this);
        if (cleanable is MonoBehaviour mb)
        {
            if (ActiveRoomDrawer != null)
            {
                mb.transform.parent = ActiveRoomDrawer.transform;
            } else
            {
                Debug.LogWarning("Cleanable attempted to be added to an unactive room (" + name + "). This shouldn't happen. Make sure Room Drawers are set up properly.");
                mb.transform.parent = transform;
            }
        }
        _containedCleanable.Add(cleanable);
        UpdateRoomCleanliness();
    }


    public void ObjectCleaned(ICleanable cleanable)
    {
        if (_containedCleanable.Contains(cleanable))
        {
            _containedCleanable.Remove(cleanable);
        }
        UpdateRoomCleanliness();
    }

    private void UpdateRoomCleanliness()
    {
        if (_isRoomCleaned) return; // don't need to update cleanliness if room is already clean.

        int amountToClean = 0;
        foreach (ICleanable cleanable in _containedCleanable)
        {
            amountToClean += cleanable.Size;
        }
        _roomCurrentTrashAmount = amountToClean;
        if (amountToClean <= 0) {
            _isRoomCleaned = true;
            OnRoomClean();
        }
    }

    private void OnRoomClean()
    {
        if (_openGatesOnClean)
        {
            _isRoomClosed = false;
            foreach (Gate gate in _connectedGates) gate.Open(this);
        }
    }

}
