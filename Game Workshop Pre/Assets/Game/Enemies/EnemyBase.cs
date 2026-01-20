using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.Events;

public abstract class EnemyBase : MonoBehaviour, ITargetable, IAbsorbable, IHeatable
{
    [SerializeField] private BehaviourTree _behaviour;
    [SerializeField] private List<EnemyActionReference> _actions;

    // Properties (EnemyBase should only have UNIVERSAL properties. 
    // If a property is on every or almost every enemy, it can go here.)
    [SerializeField] protected float _moveSpeed;
    public float MoveSpeed { get { return _moveSpeed; } } 
    [SerializeField] protected float _size;
    [SerializeField] protected float _minSizeToAbsorb;
    [SerializeField] protected float _minVelocityToAbsorb;
    [SerializeField, Range(0,360)] protected float _facingRotation = 270f;
    public float FacingRotation { 
        get { return _facingRotation; } 
        set { _facingRotation = value; }
    }

    // Components
    protected Animator _animator;
    public Animator Animator { get { return _animator; } }
    private Rigidbody2D _rigidbody;
    public Rigidbody2D Rigidbody { get { return _rigidbody; } }
    private Collider2D _collider;
    public Collider2D Collider { get { return _collider; } }
    
    private EnemyPather _pather;
    public EnemyPather Pather { get { return _pather; } }

    // Fields
    private bool _isDying = false;

    // external methods (use in specific enemies!)
    protected abstract void OnStart();
    protected abstract void OnUpdate();

    //internal methods
    private void OnValidate()
    {
        if (_behaviour != null) _behaviour.Validate(this);
    }

    // Do NOT use Start or Update in child classes. Use OnStart and OnUpdate!!!!!! 
    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _collider = GetComponent<Collider2D>();
        _animator = GetComponentInChildren<Animator>();
        _pather = GetComponent<EnemyPather>();
        if (_behaviour != null) _behaviour.Initialize(this);
        OnStart();
    }

    private void Update()
    {
        if (_isDying) return;
        
        if (_behaviour != null)
        {
            _behaviour.Tick();
        }
        OnUpdate();
    }

    private void FixedUpdate()
    {
        if (_isDying) return;
    }

    // Get an action from _actions to use 
    public void PerformAction(int index, Action<bool> onComplete = null)
    {
        if (index >= 0 && index < _actions.Count)
        {
            StartCoroutine(_actions[index].Invoke(this, onComplete));
        } else
        {
            onComplete?.Invoke(false);
        }
    }

    private void OnDrawGizmos()
    {
        //cant figure out a good way to debug draw them
        if (_behaviour != null) _behaviour.DrawDebug();
    }

    public TargetType GetTargetType()
    {
        return TargetType.Enemy;
    }


    // IHeatable
    public void PrepareIgnite(HeatMechanic heat)
    {
        _isDying = true;
        _pather.Stop();
    }
    
    
    public void OnIgnite(HeatMechanic heat)
    {
        Destroy(gameObject);
        AudioManager.Instance.Play("enemyDeath", transform.position);
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

    public void OnAbsorbedByTrashBall(TrashBall trashBall, Vector2 ballVelocity, int ballSize, bool forcedAbsorb)
    {
        if (_isDying) return;
        if (forcedAbsorb || (ballSize > _minSizeToAbsorb && ballVelocity.magnitude > _minVelocityToAbsorb && isActiveAndEnabled))
        {
            gameObject.SetActive(false);
            trashBall.absorbedObjects.Add(this);
        }
    }
}
