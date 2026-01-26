using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using UnityEngine;

public class Room : MonoBehaviour
{
    [SerializeField, Range(HeatMechanic.LOWEST_HEAT_VALUE, HeatMechanic.HIGHEST_HEAT_VALUE)] private int _baseTemperature;
    public int Temperature { get { return _baseTemperature; } } // Return _baseTemperature + anything that modifies room temperature

    private List<ICleanable> _containedCleanable = new List<ICleanable>();
<<<<<<< HEAD
    private int _roomTrashAmount;
    private int _roomTrashCount;
    //Starting room trash size total
    private int _roomTrashSizeAmount;
    //Running total of total trash size in the room
    private int _currentTrashSizeAmount;
=======
    private int _roomCurrentTrashAmount; // Current amount of trash in the room
    private int _roomTotalTrashCount; // Starting amount of trash / Max trash allowed in room
>>>>>>> main
    public bool IsTrashRoom { get; set; } = false;
    public float Cleanliness
    {
        get { return _roomTotalTrashCount == 0 ? 1f : 1f - Mathf.Clamp01(_roomCurrentTrashAmount / (float)_roomTotalTrashCount); }
    }

<<<<<<< HEAD
    public int StartTrash
    {
        get { return _roomTrashSizeAmount; }
    }

    public int CurrentTrash
    {
        get { return _currentTrashSizeAmount; }
    }

    private void Awake()
=======
    public int FreeTrashAmount
    {
        get { return Mathf.Max(0, _roomTotalTrashCount - _roomCurrentTrashAmount); }
    }

    private void Start()
>>>>>>> main
    {
        // All trash is assigned its room at start
        ICleanable[] cleanableChildren = GetComponentsInChildren<ICleanable>();

        foreach (ICleanable cleanable in cleanableChildren)
        {
            cleanable.SetRoom(this);
            _containedCleanable.Add(cleanable);
            this._roomTrashSizeAmount += cleanable.Size;
            _currentTrashSizeAmount = this._roomTrashSizeAmount;
            
        }
        UpdateRoomCleanliness();
        _roomTotalTrashCount = _roomCurrentTrashAmount;
        if (_roomTotalTrashCount > 0) IsTrashRoom = true;
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
        //if (cleanable is MonoBehaviour mb)
        //{
        //    mb.transform.parent = transform;
        //}
        _containedCleanable.Add(cleanable);
        _currentTrashSizeAmount += cleanable.Size;
        UpdateRoomCleanliness();
    }


    public void ObjectCleaned(ICleanable cleanable)
    {
        if (_containedCleanable.Contains(cleanable))
        {
            _containedCleanable.Remove(cleanable);
            _currentTrashSizeAmount -= cleanable.Size;
        }
        UpdateRoomCleanliness();
    }

    private void UpdateRoomCleanliness()
    {
        int amountToClean = 0;
        foreach (ICleanable cleanable in _containedCleanable)
        {
            amountToClean += cleanable.Size;
        }
        _roomCurrentTrashAmount = amountToClean;
    }

}
