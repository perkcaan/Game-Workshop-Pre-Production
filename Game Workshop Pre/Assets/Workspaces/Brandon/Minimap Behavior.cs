using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MinimapBehavior : MonoBehaviour
{

    [SerializeField] Tile baseMinimapTile;
    [SerializeField] Tile wallTile;
    [SerializeField] Tile floorTile;
    [SerializeField] Tile enemyTile;
    [SerializeField] Tile gateTile;
    [SerializeField] Tile checkpointTile;
    [SerializeField] Tile playerTile;
    [SerializeField] RenderTexture minimapUIRenderTexture;
    [SerializeField] float minimapCameraScale = 10f;
    [SerializeField] Vector4 minimapLavaColor = new (1f, 0.48f, 0f, 1f);
    [SerializeField] float mapUpdateRate = 0.10f;
    [SerializeField] GameObject[] enemies;

    GameObject minimapCameraObject;
    GameObject grid;
    GameObject minimapGameObject;
    GameObject pc;

    Tilemap[] tilemaps;
    static Tilemap minimap;

    Color tileDefault;
    Camera minimapCamera;

    float minimapZOffset = -50f;
    int minimapLayer;
    bool updating = false;

    static Dictionary<GameObject, Vector3Int> trackables = new Dictionary<GameObject, Vector3Int>();
    static Dictionary<GameObject, TileHistory> tileHistoryDict = new Dictionary<GameObject, TileHistory>();
    public static MinimapBehavior instance;

    void Start()
    {
        if (instance != null)
            Destroy(this);

        instance = this;
    }


    void Awake()
    {
        pc = GameObject.Find("PlayerController");
        AddToMinimapTrackables(pc);
        
        foreach (GameObject o in enemies)
            AddToMinimapTrackables(o);

        GameObject mc = GameObject.Find("Main Camera");
        minimapCameraObject = new GameObject("Minimap Camera");
        minimapCamera = minimapCameraObject.AddComponent<Camera>();
        minimapCamera.targetTexture = minimapUIRenderTexture;
        minimapCamera.orthographic = true;
        minimapCamera.clearFlags = CameraClearFlags.SolidColor;
        minimapCamera.backgroundColor = minimapLavaColor;

        minimapLayer = LayerMask.GetMask("Minimap");
        minimapCamera.cullingMask = minimapLayer;

        minimapCameraObject.transform.position = pc.transform.position;
        minimapCameraObject.transform.position += new Vector3(0, 0, mc.transform.position.z + minimapZOffset);
        minimapCameraObject.transform.parent = pc.transform;

        tileDefault = baseMinimapTile.color;
        grid = GameObject.Find("Grid");
        tilemaps = grid.GetComponentsInChildren<Tilemap>();
        MakeMinimap(tilemaps);
    }

    // Update is called once per frame
    void Update()
    {
        minimapCamera.orthographicSize = minimapCameraScale;

        if (!updating)
            StartCoroutine(UpdateMap(mapUpdateRate));
    }

    void MakeMinimap(Tilemap[] tilemaps)
    {
        minimapGameObject = new GameObject("World Minimap");
        minimapGameObject.layer = LayerMask.NameToLayer("Minimap");
        minimapGameObject.AddComponent<Grid>();
        minimapGameObject.transform.position = grid.transform.position;

        GameObject minimapTileMap = new GameObject("Minimap Tilemap");
        minimapTileMap.transform.SetParent(minimapGameObject.transform, true);
        minimapTileMap.layer = LayerMask.NameToLayer("Minimap");
        minimapTileMap.AddComponent<TilemapRenderer>();
        minimap = minimapTileMap.GetComponent<Tilemap>();

        foreach (Tilemap tilemap in tilemaps)
        {
            if (tilemap.name == "HotLava")
                continue;
            
            BoundsInt bounds = tilemap.cellBounds;
            for (int row = bounds.xMin; row < bounds.xMax; row++)
            {
                for (int col = bounds.yMin; col < bounds.yMax; col++)
                {
                    Vector3Int cellPos = minimap.WorldToCell(new Vector3(row, col, 0));
                    if (tilemap.HasTile(cellPos))
                    {
                        Vector3Int setTilePos = new Vector3Int(cellPos.x, cellPos.y, Mathf.FloorToInt(minimapGameObject.transform.position.z));
                        string tileName = tilemap.GetTile(cellPos).name;
                        if (tileName.Contains("Wall"))
                        {
                            minimap.SetTile(setTilePos, wallTile);
                        }
                        else if (tileName.Contains("Floor") || tileName.Contains("Hallway"))
                        {
                            minimap.SetTile(setTilePos, floorTile);
                        }
                    }
                }
            }
        }
        minimap.SetTile(GetPlayersPos(), playerTile);
        minimap.CompressBounds();
        minimap.RefreshAllTiles();
    }

    Vector3Int GetPlayersPos()
    {
        return new Vector3Int(
            Mathf.FloorToInt(pc.transform.position.x),
            Mathf.FloorToInt(pc.transform.position.y),
            0
        );
    }

    Vector3Int GetPosition(GameObject go)
    {
        return new Vector3Int(
            Mathf.FloorToInt(go.transform.position.x),
            Mathf.FloorToInt(go.transform.position.y),
            0
        );
    }


    public static void AddToMinimapTrackables(GameObject go)
    {
        trackables[go] = GetPositionStatic(go);
        //if (!tileHistoryDict.ContainsKey(go))
        //{
            //TileHistory th = new TileHistory(go);
            //tileHistoryDict.Add(go, th);
        //}
    }


    static Vector3Int GetPositionStatic(GameObject go)
    {
        return new Vector3Int(
            Mathf.FloorToInt(go.transform.position.x),
            Mathf.FloorToInt(go.transform.position.y),
            0
        );
    }

    public static void RemoveFromMinimapTrackables(GameObject go)
    {
        if (trackables.ContainsKey(go))
            trackables.Remove(go);

        if (tileHistoryDict.ContainsKey(go))
            tileHistoryDict.Remove(go);
    }

    int GetInstanceID(GameObject go)
    {
        return go.GetInstanceID();
    }

    IEnumerator UpdateMap(float mapUpdateRate)
    {
        updating = true;

        if (trackables.TryGetValue(pc, out var last))
        {
            var now = GetPlayersPos();
            if (now != last)
            {
                minimap.SetTile(last, floorTile);
                minimap.SetTile(now, playerTile);
                trackables[pc] = now;
            }
        }

        var keys = new List<GameObject>(trackables.Keys);

        foreach (var trackable in keys)
        {
            if (trackable == pc) continue;

            if (trackables.TryGetValue(trackable, out var lastKnown))
            {
                var now = GetPosition(trackable);
                //TileHistory
                if (now != lastKnown)
                {
                    minimap.SetTile(lastKnown, floorTile);
                    minimap.SetTile(now, enemyTile);
                    trackables[trackable] = now;
                }
            }
        }
        updating = false;
        yield return new WaitForSeconds(mapUpdateRate);
    }

    static string AssessTileType(string tileType)
    {
        if (tileType.Contains("Floor"))
            return "Floor";
        else if (tileType.Contains("Wall"))
            return "Wall";

        return "None";
    }
     class TileHistory
    {
        GameObject owner;
        bool wasFloor;
        bool nextFloor;

        public TileHistory(GameObject gameObject)
        {
            this.owner = gameObject;
            string tileType = minimap.GetTile(GetPositionStatic(gameObject)).name;
            tileType = AssessTileType(tileType);
            if (tileType == "Floor")
                wasFloor = true;
            else
                wasFloor = false;

            nextFloor = false;
        }

        static void UpdateHistory(GameObject go, string tileType)
        {

        }
    }
}
