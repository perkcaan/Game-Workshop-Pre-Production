using UnityEngine;
using System.Collections.Generic;

public abstract class SaveData
{

    protected string _name;

    protected SaveData(string name) 
    {
        _name = name;
    }

}


public class PlayerData : SaveData
{

    public Vector3 position;
    public Vector3 closestCleanablePosition;
    public float heat;
    public bool[] abilities;
    public string closestICleanable;


    public PlayerData(string name, GameObject gameObject, PlayerMovementController pmc, TrashRadarManager radar) : base(name)
    {
        position = gameObject.transform.position;
        closestCleanablePosition = radar.ClosestCleanable.transform.position;
        closestICleanable = radar.ClosestCleanable.name;
        heat = pmc.PlayerHeat.Heat;
    }
}

public class InventoryData : SaveData {

    public List<string> itemsData;

    public InventoryData(string name, List<Item> items) : base(name)
    {
        foreach (Item item in items)
        {
            itemsData.Add(item.displayName);
        }
    }
}

public class EnemyData : SaveData
{
    public string enemyType;
    public Vector3 position;
    public List<Vector2> patrolPoints;

    public EnemyData(string name, EnemyBase enemy) : base(name)
    {
        if (enemy as CloseMeleeEnemy != null){
            if (enemy.name.Contains("Imp"))
                enemyType = "Imp";
        }
    }


}

