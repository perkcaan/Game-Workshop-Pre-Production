using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileSorter : MonoBehaviour
{
    [Header("Tilemaps")]
    public Tilemap unsortedTilemap; 
    public Tilemap lavaTilemap;
    public Tilemap floorTilemap;
    public Tilemap wallTilemap;
    public Tilemap depthTilemap;
    public Tilemap decorationTilemap;

    [Header("Tile Categories")]
    public List<TileBase> lavaTiles;
    public List<TileBase> floorTiles;
    public List<TileBase> wallTiles;
    public List<TileBase> depthTiles;
    public List<TileBase> decorationTiles;

    [ContextMenu("Sort All Tiles")]
    public void SortTiles()
    {
        BoundsInt bounds = unsortedTilemap.cellBounds;
        int movedCount = 0;
        for (int x = bounds.xMin; x < bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y < bounds.yMax; y++)
            {
                Vector3Int pos = new Vector3Int(x, y, 0);
                TileBase tile = unsortedTilemap.GetTile(pos);

                if (tile != null)
                {
                    if (MoveTile(tile, pos))
                    {
                        movedCount++;
                    }
                }
            }
        }

        Debug.Log("sorting complete");
    }

    private bool MoveTile(TileBase tile, Vector3Int pos)
    {
        if (lavaTiles.Contains(tile))
        {
            lavaTilemap.SetTile(pos, tile);
            unsortedTilemap.SetTile(pos, null);
            return true;
        }
        else if (floorTiles.Contains(tile))
        {
            floorTilemap.SetTile(pos, tile);
            unsortedTilemap.SetTile(pos, null);
            return true;
        }
        else if (wallTiles.Contains(tile))
        {
            wallTilemap.SetTile(pos, tile);
            unsortedTilemap.SetTile(pos, null);
            return true;
        }
        else if (depthTiles.Contains(tile))
        {
            depthTilemap.SetTile(pos, tile);
            unsortedTilemap.SetTile(pos, null);
            return true;
        }
        else if (decorationTiles.Contains(tile))
        {
            decorationTilemap.SetTile(pos, tile);
            unsortedTilemap.SetTile(pos, null);
            return true;
        }

        Debug.LogWarning("tile "+tile.name+" at "+pos+" is not assigned");
        return false;
    }
}