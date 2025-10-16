using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DistrictManager : StaticInstance<DistrictManager>
{
    [SerializeField, Range(HeatMechanic.LOWEST_HEAT_VALUE, HeatMechanic.HIGHEST_HEAT_VALUE)] private int _baseTemperature;
    public int Temperature { get { return _baseTemperature; } }

    [SerializeField] private Tilemap _roomTilemap;

    [ContextMenu("Generate Rooms")]
    private void GenerateRooms()
    {
        RoomPolygonGenerator.GeneratePolygonColliders(transform, _roomTilemap);
    }
}
