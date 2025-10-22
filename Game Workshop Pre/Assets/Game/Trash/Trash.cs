<<<<<<< HEAD
using System.Collections;
using System.Collections.Generic;
=======
using System;
>>>>>>> main
using UnityEngine;


// An abstract class for Trash. All types of Trash can inherit from this.
[RequireComponent(typeof(Rigidbody2D))]
<<<<<<< HEAD
public abstract class Trash : MonoBehaviour, ISweepable, ISwipeable
{

    // Trash IDs to solve trash merge ties
    private static int _nextId = 0;
    public int TrashId { get; private set; }
    protected virtual bool MergePriority { get { return false; } }


    [Header("Trash")]
    [SerializeField] private float _size = 1f;
    public float Size
    {
        get { return _size; }
        set
        {
            _size = value;
            OnSizeChanged();
        }
    }
=======
public abstract class Trash : MonoBehaviour, IAbsorbable, IHeatable, ICleanable
{
    [SerializeField] protected GameObject _trashBallPrefab;

    [SerializeField] protected float _explosionMultiplier;

    [Header("Trash")]
    [SerializeField] protected int _size;
    public int Size { get { return _size; } }
    public TrashMaterial trashMaterial;
>>>>>>> main


    protected Room _parentRoom;
    //Components
    protected Rigidbody2D _rigidBody;
    // Unity methods
    private void Awake()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
<<<<<<< HEAD
        TrashId = _nextId++;
    }

    // Override to do something when size changes
    protected virtual void OnSizeChanged() { }

    // These interfaces can be overriden in a Child class if we want trash that acts differently
    // ISweepable
    public virtual void OnSweep(Vector2 direction, float force)
    {
        _rigidBody.AddForce(direction * force, ForceMode2D.Force);
=======
    }
    protected void CreateTrashBall()
    {
        GameObject trashBallObject = Instantiate(_trashBallPrefab);
        trashBallObject.transform.position = transform.position;
        TrashBall trashBall = trashBallObject.GetComponent<TrashBall>();

        if (trashBall == null)
        {
            Debug.LogWarning("TrashBall prefab prepared incorrectly");
            Destroy(trashBallObject);
            return;
        }

        trashBall.AbsorbTrash(this);
        trashBall.GetComponent<Rigidbody2D>().velocity = _rigidBody.velocity;
>>>>>>> main
    }

    //ISwipeable
    public virtual void OnSwipe(Vector2 direction, float force)
    {
<<<<<<< HEAD
        _rigidBody.AddForce(direction * force, ForceMode2D.Impulse);
    }

=======
        if (forcedAbsorb || (Size <= trashBall.Size && isActiveAndEnabled))
        {
            trashBall.AbsorbTrash(this);
        }
    }

    public void OnTrashBallExplode(TrashBall trashBall)
    {
        gameObject.SetActive(true);
        transform.position = trashBall.transform.position;
        float explosionForce = (float)(Math.Sqrt(trashBall.Size) * _explosionMultiplier);
        Vector2 randomForce = new Vector2(UnityEngine.Random.Range(-explosionForce, explosionForce), UnityEngine.Random.Range(-explosionForce, explosionForce));
        _rigidBody.velocity = randomForce;
    }

    public void OnIgnite(HeatMechanic heat)
    {
        _parentRoom.ObjectCleaned(this);
        Destroy(gameObject);
    }

    public void OnTrashBallIgnite()
    {
        _parentRoom.ObjectCleaned(this);
        Destroy(gameObject);
    }

    public void SetRoom(Room room)
    {
        _parentRoom = room;
    }
    
>>>>>>> main
}
