using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapDataGenerator : MonoBehaviour
{
    [SerializeField] PathfindingManager _pathfindingManager;
    [SerializeField] private Tilemap _floorTilemap;
    [SerializeField] private Tilemap _wallTilemap;
    [SerializeField] private Tilemap _extraWallTilemap;


    [ContextMenu("Generate Pathfinding Nodes")]
    private void GeneratePathfindingNotes()
    {
        if (_pathfindingManager == null) _pathfindingManager = PathfindingManager.Instance;

        List<PathfindingNode> pathfindingNodes = new List<PathfindingNode>();;
        BoundsInt bounds = _floorTilemap.cellBounds;
        foreach (Vector3Int pos in bounds.allPositionsWithin)
        {
            if (_floorTilemap.HasTile(pos) && !_wallTilemap.HasTile(pos) && !_extraWallTilemap.HasTile(pos))
            {
                Vector3 worldPos = _floorTilemap.GetCellCenterWorld(pos);
                Vector2Int v2Pos = new Vector2Int(pos.x, pos.y);
                pathfindingNodes.Add(new PathfindingNode(v2Pos, worldPos));
            }
        }
        _pathfindingManager.SetNodes(pathfindingNodes);
        _pathfindingManager.LayoutGrid = _floorTilemap.layoutGrid;
    }


}
