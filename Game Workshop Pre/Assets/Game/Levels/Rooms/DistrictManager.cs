using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public class DistrictManager : StaticInstance<DistrictManager>
{
    [SerializeField, Range(HeatMechanic.LOWEST_HEAT_VALUE, HeatMechanic.HIGHEST_HEAT_VALUE)] private int _baseTemperature;
    public int Temperature { get { return _baseTemperature; } }

    [SerializeField] private Tilemap _roomTilemap;
    private List<Room> _rooms = new List<Room>();
    [SerializeField] TextMeshProUGUI _coinText;
    // Rooms the player is in
    private List<Room> _focusedRooms = new List<Room>(); // focused rooms is a list since the player could be in multiple touching rooms.
    public Room FocusedRoom { get { return _focusedRooms.Count > 0 ? _focusedRooms[0] : null; } }

    //rooms that need to be safely exited.
    private List<Room> _roomsNeedingSafeExit = new List<Room>();

    // Rooms currently loaded
    private HashSet<Room> _loadedRooms = new HashSet<Room>();
    private int coinsEarned;

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
            if (room.IsRoomCleaned)
            {
                completeRooms++;
            }
        }

        return totalTrashRooms == 0 ? 1f : Mathf.Clamp01(completeRooms/ (float)totalTrashRooms);
    }

    public void PlayerEnterRoom(Room room)
    {
        if (_focusedRooms.Contains(room)) return;
        _focusedRooms.Add(room);
        if (_roomsNeedingSafeExit.Contains(room)) _roomsNeedingSafeExit.Remove(room);
        foreach (Room needyRoom in _roomsNeedingSafeExit)
        {
            needyRoom.SafeExit();
        }
        _roomsNeedingSafeExit.Clear();
        UpdateLoadedRooms();
    }
    public void PlayerExitRoom(Room room)
    {
        if (!_focusedRooms.Contains(room)) return;
        _focusedRooms.Remove(room);
        
        if (_focusedRooms.Count > 0) {
            room.SafeExit();
        } else
        {
            _roomsNeedingSafeExit.Add(room);
        }

        UpdateLoadedRooms(); 
    }

    private void Start()
    {
        _rooms = new List<Room>(FindObjectsOfType<Room>());
        DOTween.To(() => _coinText.alpha, x => _coinText.alpha = x, 0f, 0f);
        if (PlayerPrefs.HasKey("Coins"))
            coinsEarned = PlayerPrefs.GetInt("Coins");
        else
            coinsEarned = 0;
        _coinText.text = $"Coins: {PlayerPrefs.GetInt("Coins")}";
    }

    public void AwardCoins(int amount)
    {
        
        int coinsToAward = amount;
        //coinsToAward += amount;
        //int awardedCoins = coinsToAward + amount;
        coinsEarned += coinsToAward;
        PlayerPrefs.SetInt("Coins", coinsEarned);
        PlayerPrefs.Save();
        DOTween.To(() => _coinText.alpha, x => _coinText.alpha = x, 1f, 1f);
        _coinText.DOFade(1f, 1f).OnComplete(() => _coinText.DOFade(0f, 1f));        
        DOTween.To(() => _coinText.characterSpacing, x => _coinText.characterSpacing = x, 10f, 1f).OnComplete(() =>
            DOTween.To(() => _coinText.characterSpacing, x => _coinText.characterSpacing = x, 0f, 1f));
        _coinText.text = $"Coins: {coinsEarned}";
        

    }

    private void UpdateLoadedRooms()
    {
        // Update Loaded rooms should only be called if at least one room is focused. Otherwise it would cause problems when the player isn't in a room.
        if (_focusedRooms.Count <= 0) return; 

        HashSet<Room> newLoadedRooms = new HashSet<Room>();
        foreach (Room focusedRoom in _focusedRooms)
        {
            newLoadedRooms.Add(focusedRoom);
            foreach (Room nearbyRoom in focusedRoom.NearbyRooms)
            {
                newLoadedRooms.Add(nearbyRoom);
            }
        }
        // In New, not in Old > Activate Room
        HashSet<Room> roomsToActivate = new HashSet<Room>(newLoadedRooms);
        roomsToActivate.ExceptWith(_loadedRooms);

        // In Old, not in New > Deactivate Room
        HashSet<Room> roomsToDeactivate = new HashSet<Room>(_loadedRooms);
        roomsToDeactivate.ExceptWith(newLoadedRooms);

        foreach (Room room in roomsToActivate)
        {
            room.ActivateRoom();
        }
        foreach (Room room in roomsToDeactivate)
        {
            room.DeactivateRoom();
        }
        _loadedRooms = newLoadedRooms;
        
    }

    

}
