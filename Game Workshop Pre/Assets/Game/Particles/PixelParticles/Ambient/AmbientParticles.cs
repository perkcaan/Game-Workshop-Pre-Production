using UnityEngine;
using UnityEngine.Tilemaps;

public class LavaParticleManager : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] Tilemap lavaTilemap;
    [SerializeField] Camera mainCam;
    [SerializeField] float bubbleChance = 0.2f;
    [SerializeField] float emissionChance = 0.5f;
    [SerializeField] float emissionInterval = 1f;
    private float emissionTimer = 0f;

    void Awake()
    {
        if (lavaTilemap == null)
        {
            Debug.Log("LavaTileMap is not serialized so ambient lava particles cant spawn, check AmbientParticles under MainCamera on the player.");
        }
    }

void Update()
    {
        float height = 2f * mainCam.orthographicSize;
        float width = height * mainCam.aspect;
        
        Vector3 camPos = mainCam.transform.position;
        Vector2 min = new Vector2(camPos.x - width / 2f, camPos.y - height / 2f);
        Vector2 max = new Vector2(camPos.x + width / 2f, camPos.y + height / 2f);

        emissionTimer += Time.deltaTime;
        if (emissionTimer >= emissionInterval)
        {
            emissionTimer = 0f;
            if (Random.value < emissionChance)
            {
                SpawnRandomLavaPop(min, max);
            }
        }
    }
void SpawnRandomLavaPop(Vector2 minBound, Vector2 maxBound)
    {
        float randX = Random.Range(minBound.x, maxBound.x);
        float randY = Random.Range(minBound.y, maxBound.y);
        Vector3 checkPos = new Vector3(randX, randY, 0);

        Vector3Int cellPos = lavaTilemap.WorldToCell(checkPos);

        if (lavaTilemap.HasTile(cellPos)) 
        {
            if (Random.value < bubbleChance)
            {
                bool hasRight = lavaTilemap.HasTile(cellPos + Vector3Int.right);
                bool hasUp   = lavaTilemap.HasTile(cellPos + Vector3Int.up);
                bool hasLeft  = lavaTilemap.HasTile(cellPos + Vector3Int.left);
                bool hasDown  = lavaTilemap.HasTile(cellPos + Vector3Int.down);
                if (hasRight && hasUp && hasLeft && hasDown) // this is to make sure that the lava bubbles only spawns in big lava pools
                {
                    ParticleManager.Instance.Play("LavaBubble", lavaTilemap.GetCellCenterWorld(cellPos));
                }
                else
                {
                    ParticleManager.Instance.Play("LavaAmbient", lavaTilemap.GetCellCenterWorld(cellPos));
                    ParticleManager.Instance.Play("CinderBeads", lavaTilemap.GetCellCenterWorld(cellPos));
                }
            }
            else
            {
                ParticleManager.Instance.Play("LavaAmbient", lavaTilemap.GetCellCenterWorld(cellPos));
            }
        }
    }
}