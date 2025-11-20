using System;
using DG.Tweening;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;


// An abstract class for Trash. All types of Trash can inherit from this.
[RequireComponent(typeof(Rigidbody2D))]
public abstract class Trash : MonoBehaviour, IAbsorbable, IHeatable, ICleanable
{
    [SerializeField] protected GameObject _trashBallPrefab;
    [SerializeField] protected float _explosionMultiplier;

    [Header("Trash")]
    [SerializeField] protected int _size;
    
    public int Size { get { return _size; } }
    public TrashMaterial trashMaterial;
    public int trashMaterialWeight = 1;

    [SerializeField] protected int _pointValue;
    private bool _pointsConsumed = false;
    public static Action<int> SendScore;

    protected Room _parentRoom;
    public Rigidbody2D _rigidBody;
    protected SpriteRenderer _spriteRenderer;

    private bool _isDestroyed = false;

    private void Awake()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        if (_pointValue <= 0) _pointValue = 1;
    }
    protected void CreateTrashBall()
    {
        if (!_rigidBody.simulated) return;
        GameObject trashBallObject = Instantiate(_trashBallPrefab);
        trashBallObject.transform.position = transform.position;
        TrashBall trashBall = trashBallObject.GetComponent<TrashBall>();

        if (trashBall == null)
        {
            Debug.LogWarning("TrashBall prefab prepared incorrectly");
            Destroy(trashBallObject);
            return;
        }

        
        


        GivePoints();
        _rigidBody.simulated = false;
        gameObject.SetActive(false);
        trashBall.AbsorbTrash(this);
        trashBall.GetComponent<Rigidbody2D>().velocity = _rigidBody.velocity;
    }

    public virtual void OnAbsorbedByTrashBall(TrashBall trashBall, float ballVelocity, int ballSize, bool forcedAbsorb)
    {   
        if (forcedAbsorb || (Size <= trashBall.Size && isActiveAndEnabled && _rigidBody.simulated))
        {
            GivePoints();
            _rigidBody.simulated = false;
            trashBall.AbsorbTrash(this);
        }
    }

    public void OnTrashBallExplode(TrashBall trashBall)
    {
        gameObject.SetActive(true);
        transform.DOScale(Vector3.one, 0.2f).SetEase(Ease.OutQuad);
        _rigidBody.simulated = true;
        transform.position = trashBall.transform.position;
        float explosionForce = (float)(Math.Sqrt(trashBall.Size) * _explosionMultiplier);
        Vector2 randomForce = new Vector2(UnityEngine.Random.Range(-explosionForce, explosionForce), UnityEngine.Random.Range(-explosionForce, explosionForce));
        _rigidBody.velocity = randomForce;
    }

    public void PrepareIgnite(HeatMechanic heat)
    {
        foreach (Collider2D col in GetComponentsInChildren<Collider2D>()) col.enabled = false;
        _rigidBody.velocity = Vector2.zero;
        _rigidBody.simulated = false;
    }
    
    public void OnIgnite(HeatMechanic heat)
    {
        if (_isDestroyed) return;
        _isDestroyed = true;

        _parentRoom.ObjectCleaned(this);
        Destroy(gameObject);
    }

    public void OnTrashBallIgnite()
    {;
        if (_isDestroyed) return;
        _isDestroyed = true;
        
        _parentRoom.ObjectCleaned(this);
        Destroy(gameObject);
    }

    public void SetRoom(Room room)
    {
        _parentRoom = room;
    }
    
    private void GivePoints()
    {
        if (!_pointsConsumed)
        {
            SendScore?.Invoke(_pointValue);
            _pointsConsumed = true;
        }
    }
}
