using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// An abstract class for Trash. All types of Trash can inherit from this.
[RequireComponent(typeof(Rigidbody2D))]
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

    //Components
    protected Rigidbody2D _rigidBody;

    // Unity methods
    private void Awake()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
        TrashId = _nextId++;
    }

    // Override to do something when size changes
    protected virtual void OnSizeChanged() { }

    // These interfaces can be overriden in a Child class if we want trash that acts differently
    // ISweepable
    public virtual void OnSweep(Vector2 direction, float force)
    {
        _rigidBody.AddForce(direction * force, ForceMode2D.Force);
    }

    //ISwipeable
    public virtual void OnSwipe(Vector2 direction, float force)
    {
        _rigidBody.AddForce(direction * force, ForceMode2D.Impulse);
    }

}
