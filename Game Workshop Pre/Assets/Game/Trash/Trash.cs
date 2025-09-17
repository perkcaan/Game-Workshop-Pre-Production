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
    public virtual float Size
    {
        get { return _size; }
        protected set
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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Trash otherTrash = collision.gameObject.GetComponent<Trash>();
        if (otherTrash == null) return;

        // Priority- only one of the colliders can run OnTrashMerge
        // first check forced merge priority
        if (MergePriority && !otherTrash.MergePriority)
        {
            OnTrashMerge(otherTrash);
            return;
        }
        // make sure to return if this loses
        if (MergePriority != otherTrash.MergePriority)
            return;

        // since they have equal priority- check speed
        float speed = _rigidBody.velocity.sqrMagnitude;
        float otherSpeed = collision.rigidbody.velocity.sqrMagnitude;

        if (speed > otherSpeed)
        {
            OnTrashMerge(otherTrash);
            return;
        }

        if (speed < otherSpeed)
            return;

        // If same speed, force winner to be based on the arbitrary TrashId
        if (TrashId > otherTrash.TrashId)
        {
            OnTrashMerge(otherTrash);
        }

    }


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

    // Override to do something when size changes
    protected virtual void OnSizeChanged() { }

    // Trash merging
    protected virtual void OnTrashMerge(Trash otherTrash)
    {
        Size += otherTrash.Size;
        Destroy(otherTrash.gameObject);
    }
    
}
