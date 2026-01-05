using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.Events;

public abstract class EnemyBase : MonoBehaviour, ITargetable, IAbsorbable, IHeatable
{
    [SerializeReference] private BehaviourTreeNode _behaviour;
    [SerializeField] private bool _enableBehaviourDebug;
    [SerializeField] private List<UnityEvent> _actionMethods;

    // Properties (EnemyBase should only have UNIVERSAL properties. 
    // If a property is on every or almost every enemy, it can go here.)
    [SerializeField] protected float _moveSpeed;
    [SerializeField] protected float _size;
    [SerializeField] protected float _minSizeToAbsorb;
    [SerializeField] protected float _minVelocityToAbsorb;

    // Components
    protected EnemyBlackboard _blackboard;
    protected Animator _animator;
    private Rigidbody2D _rigidbody;
    public Rigidbody2D Rigidbody { get { return _rigidbody; } }
    private Collider2D _collider;
    public Collider2D Collider { get { return _collider; } }

    // Fields
    private bool _isDying = false;

    // external methods (use in specific enemies!)
    protected abstract void OnStart();
    protected abstract void OnUpdate();

    //internal methods
    private void OnValidate()
    {
        if (_behaviour != null) _behaviour.CheckRequiredComponents(this);
    }

    // Do NOT use Start or Update in child classes. Use OnStart and OnUpdate!!!!!! 
    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _collider = GetComponent<Collider2D>();
        _animator = GetComponentInChildren<Animator>();
        _blackboard = new EnemyBlackboard(this);
        PrepareBlackboard();
        if (_behaviour != null) _behaviour.Initialize(_blackboard, this);
        OnStart();
    }

    private void Update()
    {
        if (_isDying) return;
        
        if (_behaviour != null)
        {
            _behaviour.Evaluate();
        }
        OnUpdate();
    }

    private void FixedUpdate()
    {
        if (_isDying) return;

        UpdateMovement();
    }

    private void PrepareBlackboard()
    {
        _blackboard.Set<string>("name", gameObject.name);
        _blackboard.Set<Vector2>("frameVelocity", Vector2.zero);
        _blackboard.Set<float>("rotation", 90f);

        _blackboard.Set<float>("moveSpeed", _moveSpeed);
    }

    private void UpdateMovement()
    {
        //movement
        if (!_blackboard.TryGet("frameVelocity", out Vector2 frameVelocity)) { }

        Vector2 velocityDelta = frameVelocity - _rigidbody.velocity;
        Vector2 clampedForce = Vector2.ClampMagnitude(velocityDelta, frameVelocity.magnitude);
        _rigidbody.AddForce(clampedForce, ForceMode2D.Force);
        _blackboard.Set<Vector2>("frameVelocity", Vector2.zero); // This is for clean up if behaviour node switches
        _animator.SetFloat("Speed", frameVelocity.magnitude);

        // rotation
        if (!_blackboard.TryGet("rotation", out float rotation)) { }

        _animator.SetFloat("Rotation", rotation);
    }

    public void PerformAction(int actionIndex)
    {
        for (int i = 0; i < _actionMethods.Count; i++)
        {
            if (actionIndex == i) _actionMethods[i]?.Invoke();
        }
    }

    private void OnDrawGizmos()
    {
        if (_enableBehaviourDebug && _behaviour != null) _behaviour.DrawDebug();
    }

    public TargetType GetTargetType()
    {
        return TargetType.Enemy;
    }


    // IHeatable
    public void PrepareIgnite(HeatMechanic heat)
    {
        _isDying = true;
    }
    
    
    public void OnIgnite(HeatMechanic heat)
    {
        Destroy(gameObject);
    }


    // IAbsorbable

    public void OnTrashBallExplode(TrashBall trashBall)
    {
        gameObject.SetActive(true);
        transform.position = trashBall.transform.position;
    }

    public void OnTrashBallIgnite()
    {
        Destroy(gameObject);
    }

    public void OnAbsorbedByTrashBall(TrashBall trashBall, float ballVelocity, int ballSize, bool forcedAbsorb)
    {
        if (_isDying) return;
        if (forcedAbsorb || (ballSize > _minSizeToAbsorb && ballVelocity > _minVelocityToAbsorb && isActiveAndEnabled))
        {
            gameObject.SetActive(false);
            trashBall.absorbedObjects.Add(this);
        }
    }
}
