using UnityEngine;


// An abstract class for Trash. All types of Trash can inherit from this.
[RequireComponent(typeof(Rigidbody2D))]
public abstract class Trash : MonoBehaviour, IAbsorbable
{
    [SerializeField] protected GameObject _trashBallPrefab;
    [SerializeField] protected float _explosionMultiplier;
    // Trash IDs to solve trash merge ties
    private static int _nextId = 0;
    public int TrashId { get; private set; }


    [Header("Trash")]
    [SerializeField] public float Size;
    protected float _mergableDelay;

    //Components
    protected Rigidbody2D _rigidBody;

    // Unity methods
    private void Awake()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
        _mergableDelay = 1f;
        TrashId = _nextId++;
    }

    protected void CreateTrashBall()
    {
        if (_mergableDelay > 0) return;
        GameObject trashBallObject = Instantiate(_trashBallPrefab);
        trashBallObject.transform.position = transform.position;
        TrashBall trashBall = trashBallObject.GetComponent<TrashBall>();

        if (trashBall == null)
        {
            Debug.LogWarning("TrashBall prefab prepared incorrectly");
            Destroy(trashBallObject);
            return;
        }

        trashBall.absorbedObjects.Add(this);
        trashBall.Size = Size;
        gameObject.SetActive(false);
    }

    public virtual void OnAbsorbedByTrashBall(TrashBall trashBall, float absorbingPower, bool forcedAbsorb)
    {
        if (forcedAbsorb || (Size <= trashBall.Size && isActiveAndEnabled))
        {
            trashBall.absorbedObjects.Add(this);
            trashBall.Size += Size;
            gameObject.SetActive(false);
        }
    }

    public void OnTrashBallExplode(TrashBall trashBall)
    {
        _mergableDelay = 2f;
        transform.position = trashBall.transform.position;
        float explosionForce = (1 - trashBall.Size) * _explosionMultiplier;
        Vector2 randomForce = new Vector2(Random.Range(-explosionForce, explosionForce), Random.Range(-explosionForce, explosionForce));
        _rigidBody.velocity = randomForce;
    }

    public void OnIgnite()
    {
        Destroy(gameObject);
    }
}
