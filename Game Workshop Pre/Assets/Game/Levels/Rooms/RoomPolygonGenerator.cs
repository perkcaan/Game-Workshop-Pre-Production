using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RoomPolygonGenerator
{

    public static void GeneratePolygonColliders(Transform parent, Tilemap roomTilemap)
    {
        // Get prefab
        GameObject roomPrefab = Resources.Load<GameObject>("Prefabs/Levels/Room");
        if (roomPrefab == null)
        {
            Debug.LogError("Room prefab not found. Please assure it is located at 'Assets/Resources/Prefab/SourceLevels/Room'");
            return;
        }

        // Step 1 - Get positions of tiles and which tile.
        Dictionary<Vector2Int, TileBase> tilePositions = GetTilesFromTilemap(roomTilemap);

        // Step 2 - BFS floodfill tiles of the same type to generate regions
        List<HashSet<Vector2Int>> regions = GenerateRegionsFromTiles(tilePositions);


        // Step 3 - Outline tracing algorithm to generate vertices of outline of region and use that for a polygon collider
        for (int i = 0; i < regions.Count; i++)
        {
            GameObject newObject = (GameObject)PrefabUtility.InstantiatePrefab(roomPrefab, parent);
            newObject.transform.parent = parent;
            PolygonCollider2D polyCollider = newObject.GetComponent<PolygonCollider2D>();
            newObject.name = "Room" + i;
            if (polyCollider == null)
            {
                polyCollider = newObject.AddComponent<PolygonCollider2D>();
            }
            polyCollider.isTrigger = true;

            List<List<Vector2>> loops = GetLoops(regions[i]);

            polyCollider.pathCount = loops.Count;
            for (int p = 0; p < loops.Count; p++)
            {
                polyCollider.SetPath(p, loops[p].ToArray());
            }
        }
    }

    // Step 1 - Get positions of tiles and which tile.
    private static Dictionary<Vector2Int, TileBase> GetTilesFromTilemap(Tilemap tilemap)
    {
        BoundsInt bounds = tilemap.cellBounds;
        Dictionary<Vector2Int, TileBase> tilePositions = new Dictionary<Vector2Int, TileBase>();

        foreach (Vector3Int pos in bounds.allPositionsWithin)
        {
            TileBase tile = tilemap.GetTile(pos);
            if (tile != null)
                tilePositions[(Vector2Int)pos] = tile;

        }
        return tilePositions;
    }

    // Step 2 - BFS floodfill tiles of the same type to generate regions
    private static List<HashSet<Vector2Int>> GenerateRegionsFromTiles(Dictionary<Vector2Int, TileBase> tilePositions)
    {
        List<HashSet<Vector2Int>> regions = new List<HashSet<Vector2Int>>();
        HashSet<Vector2Int> visited = new HashSet<Vector2Int>();
        Vector2Int[] dirs = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };

        // check all positions with tiles
        foreach (KeyValuePair<Vector2Int, TileBase> pair in tilePositions)
        {
            Vector2Int pos = pair.Key;
            TileBase tile = pair.Value;
            if (visited.Contains(pos)) continue; // position is in an already defined region

            HashSet<Vector2Int> region = new HashSet<Vector2Int>();
            Queue<Vector2Int> queue = new Queue<Vector2Int>();
            queue.Enqueue(pos); // Enter this position as the starting point to the queue

            while (queue.Count > 0) // The queue is empty when the floodfill has finished
            {
                Vector2Int checkPos = queue.Dequeue();
                if (!visited.Add(checkPos)) continue; // If already visited, skip this
                region.Add(checkPos);

                // Check each direction for matching tiles nearby
                foreach (Vector2Int dir in dirs)
                {
                    Vector2Int nearbyPos = checkPos + dir;
                    if (tilePositions.TryGetValue(nearbyPos, out TileBase nearbyTile))
                    {
                        // If nearby tile matches, add to queue
                        if (nearbyTile == tile && !visited.Contains(nearbyPos)) queue.Enqueue(nearbyPos);
                    }
                }
            }

            regions.Add(region);
        }
        return regions;
    }



    // Helper struct for Step 3
    private struct Edge
    {
        public Vector2Int a, b;
        public Edge(Vector2Int a, Vector2Int b) { this.a = a; this.b = b; }
    }


    // Step 3 - From a region of tile coordinates, generate a list of polygon loops.
    private static List<List<Vector2>> GetLoops(HashSet<Vector2Int> region)
    {

        // Step 3a
        HashSet<Edge> edges = GetEdges(region);
        // Step 3b
        Dictionary<Vector2Int, List<Vector2Int>> adjacency = GetAdjacency(edges);

        //walk around edges to get closed loop
        HashSet<Edge> usedEdges = new HashSet<Edge>();
        List<List<Vector2>> loops = new List<List<Vector2>>();

        foreach (Edge startEdge in edges)
        {
            if (usedEdges.Contains(startEdge)) continue; //remove if already explored in previous loop
            List<Vector2Int> loopInts = new List<Vector2Int>();
            Vector2Int start = startEdge.a;
            Vector2Int current = startEdge.a;
            Vector2Int next = startEdge.b;

            //mark as used and record first edge
            usedEdges.Add(startEdge);
            loopInts.Add(current);
            current = next;

            //now follow around the loop until we get back to start;
            int currentWalk = 0;
            int maxWalks = 100000;
            while (true)
            {
                //safety
                currentWalk++;
                if (currentWalk > maxWalks) throw new Exception("Cannot generate room loops. It didn't work :(");

                loopInts.Add(current);
                if (!adjacency.TryGetValue(current, out List<Vector2Int> candidates)) break;

                // pick a candidate
                Vector2Int chosenCandidate = default;
                bool found = false;
                foreach (Vector2Int candidate in candidates)
                {
                    Edge candidateEdge = new Edge(current, candidate);
                    // if candidate hasnt already been used we choose it
                    if (!usedEdges.Contains(candidateEdge))
                    {
                        chosenCandidate = candidate;
                        found = true;
                        usedEdges.Add(candidateEdge);
                        break;
                    }
                }

                //no unused edge. Closed the loop or error
                if (!found)
                {
                    //If doesnt link to the start uhhhh  break.
                    if (current == start) break;
                    // try to close back to start if possible..
                    if (candidates.Contains(start))
                    {
                        current = start;
                        break;
                    }
                    break;
                }

                current = chosenCandidate;
                if (current == start) break; // end loop we got it boys
            }


            //convert integer corner points to Vector2 in tile map-space, create a loop
            List<Vector2> loop = new List<Vector2>();
            for (int i = 0; i < loopInts.Count; i++)
            {
                Vector2Int point = loopInts[i];
                // avoid duplicate consecutive points
                if (loop.Count > 0)
                {
                    Vector2 last = loop[loop.Count - 1];
                    if (Mathf.Approximately(last.x, point.x) && Mathf.Approximately(last.y, point.y))
                        continue;
                }
                loop.Add(new Vector2(point.x, point.y));
            }

            //Step 3c
            loop = RemoveColinear(loop);


            // make sure the loop is going clockwise. Counter clockwise breaks polygon colliders for some reason
            float area = 0f;
            for (int i = 0; i < loop.Count; i++)
            {
                Vector2 a = loop[i];
                Vector2 b = loop[(i + 1) % loop.Count];
                area += a.x * b.y - b.x * a.y;
            }
            if ((area * 0.5f) > 0) loop.Reverse();

            if (loop.Count >= 3) //edge case check for valid polygon
                loops.Add(loop);
        }

        return loops;
    }

    // Step 3a - Get edges of tiles that will be in the loop
    private static HashSet<Edge> GetEdges(HashSet<Vector2Int> region)
    {
        // Step 4 - Create a set of edges to be used
        HashSet<Edge> edges = new HashSet<Edge>();

        foreach (Vector2Int tilePos in region)
        {
            int x = tilePos.x;
            int y = tilePos.y;

            // If no tile to the right, add edge to the right of the tile
            if (!region.Contains(new Vector2Int(x + 1, y)))
            {
                Vector2Int a = new Vector2Int(x + 1, y);
                Vector2Int b = new Vector2Int(x + 1, y + 1);
                edges.Add(new Edge(a, b));
            }

            // If no tile above, add edge above the tile
            if (!region.Contains(new Vector2Int(x, y + 1)))
            {
                Vector2Int a = new Vector2Int(x + 1, y + 1);
                Vector2Int b = new Vector2Int(x, y + 1);
                edges.Add(new Edge(a, b));
            }

            // If no tile to the left, add edge to the left of the tile
            if (!region.Contains(new Vector2Int(x - 1, y)))
            {
                Vector2Int a = new Vector2Int(x, y + 1);
                Vector2Int b = new Vector2Int(x, y);
                edges.Add(new Edge(a, b));
            }

            // If no tile below, add edge below the tile
            if (!region.Contains(new Vector2Int(x, y - 1)))
            {
                Vector2Int a = new Vector2Int(x, y);
                Vector2Int b = new Vector2Int(x + 1, y);
                edges.Add(new Edge(a, b));
            }
        }

        return edges;
    }


    // Step 3b - build adjacency from start corner -> list of end corners
    private static Dictionary<Vector2Int, List<Vector2Int>> GetAdjacency(HashSet<Edge> edges)
    {
        Dictionary<Vector2Int, List<Vector2Int>> adjacency = new Dictionary<Vector2Int, List<Vector2Int>>();
        foreach (Edge edge in edges)
        {
            if (!adjacency.TryGetValue(edge.a, out var list))
            {
                list = new List<Vector2Int>();
                adjacency[edge.a] = list;
            }
            list.Add(edge.b);
        }
        return adjacency;
    }

    //Step 3c - Remove any colinear points. 
    private static List<Vector2> RemoveColinear(List<Vector2> points)
    {
        float epsilon = 1e-6f;
        if (points.Count <= 2) return new List<Vector2>(points);

        List<Vector2> trimmedPoints = new List<Vector2>();
        int n = points.Count;
        for (int i = 0; i < n; i++)
        {
            Vector2 prev = points[(i + n - 1) % n];
            Vector2 current = points[i];
            Vector2 next = points[(i + 1) % n];

            //cross product (current-prev) & (next-current)
            float cross = (current.x - prev.x) * (next.y - current.y) - (current.y - prev.y) * (next.x - current.x);
            if (Mathf.Abs(cross) > epsilon)
                trimmedPoints.Add(current);
        }
        return trimmedPoints;
    }

}