using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TrashCubeObject : MonoBehaviour, IHeatable, ISweepable, ISwipeable
{
    public List<IAbsorbable> absorbedObjects = new List<IAbsorbable>();
    public Rigidbody2D rigidBody;
    public float trashPlatformDuration;
    public int maxNumberOfTrashPlatforms;
    [SerializeField] TrashPlatform _trashPlatformPrefab;
    private List<Vector2Int> _trashPlatformLocations = new List<Vector2Int>();
    private int _size = 1;
    private float _particleTimer = 0f;

    [Header("OnSweep Properties")]
    [SerializeField] float _vacuumForce;
    [SerializeField] float _minimumVacuumForce;
    [SerializeField] float _sizeMultiplier;

    public int Size
    {
        get { return _size; }
        set { _size = value; }
    }
    public void Awake()
    {
        rigidBody = GetComponent<Rigidbody2D>();
    }

    public void OnSweep(Vector2 center, Vector2 direction, float force)
    {
        Vector3 centerPoint = center + (direction * Mathf.Pow(Size, 1f / 3f) / Mathf.PI);
        float distance = Vector2.Distance(transform.position, centerPoint);
        float newForce = force * distance * (_minimumVacuumForce + (_vacuumForce / Size * _sizeMultiplier));
        Vector2 directionToCenterPoint = (centerPoint - transform.position).normalized;
        rigidBody.AddForce(directionToCenterPoint * newForce, ForceMode2D.Force);
    }

    public void OnSwipe(Vector2 direction, float force, Collider2D collider)
    {
        rigidBody.AddForce(direction * force, ForceMode2D.Impulse);

        // particles
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion particleRotation = Quaternion.Euler(0, 0, angle + 135);
        if (Size > 10)
            ParticleManager.Instance.Play("TrashSwiped", transform.position, particleRotation, null, null, 1f);
        else
            ParticleManager.Instance.Play("TrashSwiped", transform.position, particleRotation, null, null, 0.5f);
    }

    public void OnIgnite(HeatMechanic heat)
    {
        foreach (IAbsorbable absorbable in absorbedObjects)
        {
            absorbable.OnTrashBallIgnite();
        }
        absorbedObjects.Clear();
        Destroy(gameObject);
    }

    public void PrepareIgnite(HeatMechanic heat)
    {

    }

    public void Update()
    {
        _particleTimer -= Time.deltaTime * rigidBody.velocity.magnitude / 10f;
        if (_particleTimer <= 0 && rigidBody.velocity.magnitude > 0.5f)
        {
            _particleTimer = 0.1f;
            ParticleManager.Instance.Play("TrashDustTrail", transform.position, Quaternion.identity, force: 2f);
        }
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        if (_trashPlatformLocations.Count < maxNumberOfTrashPlatforms)
        {
            if (collision.gameObject.TryGetComponent(out Lava lava))
            {
                //Vector2 realPosition = new Vector2(transform.position.x, transform.position.y);
                Grid grid = lava.GetComponentInParent<Grid>();
                Tilemap lavaTilemap = lava.GetComponent<Tilemap>();
                Vector3Int cellPos = grid.WorldToCell(transform.position);
                if (lavaTilemap.GetTile(cellPos) == null) return;
                if (_trashPlatformLocations.Contains((Vector2Int)cellPos)) return;

                Vector3 realPosition = new Vector2(cellPos.x + 0.5f, cellPos.y + 0.5f);
                Instantiate(_trashPlatformPrefab, realPosition, Quaternion.identity).floatingDuration = trashPlatformDuration;
                _trashPlatformLocations.Add((Vector2Int)cellPos);
            }
        }
    }
}
