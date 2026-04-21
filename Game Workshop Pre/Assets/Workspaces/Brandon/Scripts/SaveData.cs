using UnityEngine;
using System.Collections.Generic;


public abstract class SaveData
{

    public  string loadID;
    public Vector3 position;
    public Vector3 rotation;

    protected SaveData(SaveableObject obj) 
    {
        loadID = obj.loadID;
        position = obj.gameObjectReference.transform.position;
        rotation = obj.gameObjectReference.transform.rotation.eulerAngles;
    }
}


[System.Serializable]
public class PlayerData : SaveData
{

    public float heat;
    public bool[] abilities;
    public bool coolingOff;

    public PlayerData(SaveableObject obj) : base(obj)
    {
        PlayerMovementController pmc = obj.gameObjectReference.GetComponent<PlayerMovementController>();
        HeatMechanic heatMechanic = obj.gameObjectReference.GetComponent<HeatMechanic>();

        heat = pmc.PlayerHeat.Heat;
        coolingOff = pmc.PlayerHeat.coolingOff;
        abilities = new bool[] { pmc.canSweep, pmc.canSwipe, pmc.canDash, pmc.canPoke, pmc.canHook };
    }
}


[System.Serializable]
public class EnemyData : SaveData
{
    public float heat;

    public EnemyData(SaveableObject obj) : base(obj)
    {
        HeatMechanic hm = obj.gameObjectReference.GetComponent<HeatMechanic>();
    }
}

[System.Serializable]

public class RoomData : SaveData
{
    public bool isTrashRoom;
    public bool isRoomCleaned;
    public bool isPlayerInRoom;
    public float clenliness;
    public int freeTrashAmount;


    public RoomData(SaveableObject obj) : base(obj)
    {
        //Needs to create it's a new Load ID
        string name = obj.gameObjectReference.name;
        int lastLetterIndex = name.Length - 1;
        loadID += name[lastLetterIndex];

        Room room = obj.gameObjectReference.GetComponent<Room>();
        isTrashRoom = room.IsTrashRoom;
        isRoomCleaned = room.IsRoomCleaned;
        isPlayerInRoom = room.IsPlayerInRoom;
        clenliness = room.Cleanliness;
        freeTrashAmount = room.FreeTrashAmount;
    }
}

[System.Serializable]

public class DistrictManagerData : SaveData
{

    public List<RoomData> rooms;
    public List<RoomData> focusedRooms;
    public List<RoomData> roomsNeedingSafeExit;
    public int temperature;
    public int coinsEarned;
    
    public DistrictManagerData(SaveableObject obj) : base(obj)
    {
        DistrictManager dm = obj.gameObjectReference.GetComponent<DistrictManager>();
        foreach (Room room in dm.AllRooms)
        {
            SaveableObject roomSaveableObject = new SaveableObject(room.gameObject);
            RoomData roomData = new RoomData(roomSaveableObject);
            if (roomData.isPlayerInRoom == true)
                focusedRooms.Add(roomData);
        }
    }

}
