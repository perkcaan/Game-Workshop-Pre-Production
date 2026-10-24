using UnityEngine;
using UnityEngine.Tilemaps;

public class GridManager : StaticInstance<GridManager>
{
    public enum GridType
    {
        Floor,
        Depth,
        Walls,
        Lava,
        Hidden
    }

    [Header("Base Tilemaps")]
    [SerializeField] private Tilemap _floor;
    [SerializeField] private Tilemap _depth;
    [SerializeField] private Tilemap _walls;
    [SerializeField] private Tilemap _lava;
    [SerializeField] private Tilemap _hidden;
    [SerializeField] private Tilemap _rooms;

    private Tilemap TypeToTilemap(GridType gridType)
    {
        return gridType switch
        {
            GridType.Floor => _floor,
            GridType.Depth => _depth,
            GridType.Walls => _walls,
            GridType.Lava => _lava,
            GridType.Hidden => _hidden,
            _ => null
        };
    }

    private static GridManager GetInstance()
    {
        GridManager instance = Instance;
        if (Instance == null)
        {
            Debug.LogError("GridManager has not been properly set up. Some behavior is likely broken. Please fix.");
            return null;
        }
        return instance;
    } 

    public static bool HasTile(GridType gridType, Vector3Int pos)
    {
        GridManager instance = GetInstance();
        Tilemap tilemap = instance.TypeToTilemap(gridType);
        return tilemap.HasTile(pos);
    }

    public static Vector3 GetCellCenterWorld(GridType gridType, Vector3Int pos)
    {
        GridManager instance = GetInstance();
        Tilemap tilemap = instance.TypeToTilemap(gridType);
        return tilemap.GetCellCenterWorld(pos);
    }

    public static Vector3Int WorldToCell(GridType gridType, Vector3 pos)
    {
        GridManager instance = GetInstance();
        Tilemap tilemap = instance.TypeToTilemap(gridType);
        return tilemap.WorldToCell(pos);
    }
    
}
