using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters;
using AYellowpaper.SerializedCollections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

public abstract class EnemyBase : MonoBehaviour, ITargetable, IAbsorbable, IHeatable, ICleanable, ISwipeable, ISweepable, IPokeable
{
    [Header("Enemy")]
    [SerializeField] private BehaviourTree _behaviour;
    [SerializeField] private List<EnemyActionReference> _actions;
    private Coroutine _currentAction;
    private Action<bool> _currentActionComplete;

    // Properties (EnemyBase should only have UNIVERSAL properties. 
    // If a property is on every or almost every enemy, it can go here.)
    [SerializeField] protected float _moveSpeed;
    public float MoveSpeed { get { return _moveSpeed * _speedModifier; } }
    protected float _speedModifier = 1f; // this may need to be expanded
    public float SpeedModifier { 
        get { return _speedModifier; }
        set { _speedModifier = value; } 
    } 
    [SerializeField] private bool _shouldFlipSprite = false;
    [SerializeField, Range(0,360)] protected float _facingRotation = 270f;
    public float FacingRotation { 
        get { return _facingRotation; } 
        set // wrap it within 0-360
        {
            float wrapped = value % 360f;
            if (wrapped < 0f) wrapped += 360f;
            _facingRotation = wrapped;
            FlipSpriteIfNeeded();
        }
    }

    [Header("Absorbed Properties")]
    [SerializeField] private int _size;
    public int Size { get { return _size; } }
    [SerializeField] private TrashMaterial _trashMaterial;
    public TrashMaterial TrashMat { get { return _trashMaterial; } }
    [SerializeField] private int _trashMaterialWeight = 1;
    public int TrashMatWeight { get { return _trashMaterialWeight; } }

    [SerializeField] protected float _minSizeToAbsorb;
    public float MinSizeToAbsorb
    {
        get { return _minSizeToAbsorb; }
        set { _minSizeToAbsorb = value; }
    }
    [SerializeField] protected float _minVelocityToAbsorb;
    [SerializeField] private float _trashBallEscapeForce = 1f;
    [SerializeField] private float _trashBallSquirmTime = 5f;
    [SerializeField] private float _trashBallSquirmForce = 5f;
    [SerializeField] private int _trashBallSquirmDamage = 5;
    [Header("Stun Properties")]
    [SerializeField] private float _stunTime = 2f;
    public float StunTime { get { return _stunTime; } }

    [Header("Pinball Properties")]
    [SerializeField] private float _pinballTime = 5f;
    public float PinballTime { get { return _pinballTime; } }
    [SerializeField] private PhysicsMaterial2D _pinballMaterial;
    public PhysicsMaterial2D PinballMaterial { get { return _pinballMaterial; } }

    public TargetType TargetType { get { return TargetType.Enemy; } } //ITargetable

    // Components
    protected Animator _animator;
    public Animator Animator { get { return _animator; } }
    private SpriteRenderer _renderer;
    private Rigidbody2D _rigidbody;
    public Rigidbody2D Rigidbody { get { return _rigidbody; } }
    private CircleCollider2D _collider;
    public CircleCollider2D Collider { get { return _collider; } }
    public float SizeRadius { get { return Collider.radius; } }
    private List<EnemySubCollider> _subcolliders;
    public GameObject HitParent { get { return gameObject; } }
    
    private EnemyPather _pather;
    public EnemyPather Pather { get { return _pather; } }

    


    protected EnemyStateMachine _state;

    private Room _parentRoom;

    // Actions
    public Action OnDestroy;

    // Fields
    private bool _isDying = false;

    // Timers
    private float _ballSquirmTimer = 0;

    // external methods (use in specific enemies!)
    protected abstract void OnStart();
    protected abstract void OnUpdate();
    protected abstract void ForceDisableHitboxes();

    //internal methods
    private void OnValidate()
    {
        if (_behaviour != null) _behaviour.Validate(this);
    }

    // Do NOT use Start or Update in child classes. Use OnStart and OnUpdate!!!!!! 
    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _collider = GetComponent<CircleCollider2D>();
        _animator = GetComponentInChildren<Animator>();
        _renderer = GetComponentInChildren<SpriteRenderer>();
        _pather = GetComponent<EnemyPather>();
        _state = new EnemyStateMachine(this);
        _subcolliders = new List<EnemySubCollider>(GetComponentsInChildren<EnemySubCollider>());
        foreach (EnemySubCollider subcollider in _subcolliders)
        {
            subcollider.Initialize(this);
        }
        if (_behaviour != null) _behaviour.Initialize(this);
        OnStart();
    }

    private void Update()
    {
        if (_isDying) return;

        _state.Update();
        if (!_state.HasBehaviour) return;

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
            _currentActionComplete = onComplete;
            _currentAction = StartCoroutine(_actions[index].Invoke(this, CompleteAction));
        } else
        {
            onComplete?.Invoke(false);
        }
    }
    // Run to complete an action properly
    public void CompleteAction(bool completeStatus)
    {
        _currentActionComplete?.Invoke(completeStatus);
        _currentAction = null;
        _currentActionComplete = null;
    }

    // Cancels current action safely
    public void CancelAction()
    {
        _currentActionComplete?.Invoke(false);
        if (_currentAction != null)
        {
            StopCoroutine(_currentAction);
            _currentAction = null;
        }
        _currentActionComplete = null;
        ForceDisableHitboxes();
    }

    // Cancels all behaviour. Includes coroutines, pathing, actions
    public void CancelBehaviour()
    {
        _pather.Stop();
        CancelAction();
        StopAllCoroutines();
    }

    // Simple attack is a basic attack template that has startup, an attack, and endlag
    protected IEnumerator SimpleAttack(SimpleAttackProperties properties, 
        Action attackStart = null, Action attack = null, Action attackEnd = null)
    {

        attackStart?.Invoke();
        yield return new WaitForSeconds(properties.Startup);

        attack?.Invoke();
        yield return new WaitForSeconds(properties.Duration);
        
        attackEnd?.Invoke();
        yield return new WaitForSeconds(properties.Endlag);
    }

    private void FlipSpriteIfNeeded()
    {
        if (!_shouldFlipSprite) return;

        float radians = _facingRotation * Mathf.Deg2Rad;
        bool faceRight = Mathf.Cos(radians) >= 0f;

        _renderer.flipX = !faceRight;
    }

    private void OnDrawGizmos()
    {
        //cant figure out a good way to debug draw them
        if (_behaviour != null) _behaviour.DrawDebug();
    }

    // IHeatable
    public void PrepareIgnite(HeatMechanic heat)
    {
        _isDying = true;
        CancelBehaviour();
    }


    public void OnIgnite(HeatMechanic heat)
    {
        if(_parentRoom != null) _parentRoom.ObjectCleaned(this);
        AudioManager.Instance.PlayOnInstance(gameObject,"impDeath");

        OnDestroy?.Invoke();
        Destroy(gameObject);
    }


    // IAbsorbable
    public bool OnAbsorbedByTrashBall(TrashBall trashBall, Vector2 ballVelocity, int ballSize, bool forcedAbsorb)
    {
        if (_isDying) return false;
        if (forcedAbsorb || (ballSize >= _minSizeToAbsorb && ballVelocity.magnitude > _minVelocityToAbsorb && isActiveAndEnabled && _rigidbody.simulated))
        {
            _state.ChangeState(EnemyStateEnum.Absorbed);
            _rigidbody.simulated = false;

            if (forcedAbsorb) return true;
            PopupLabel.CreatePlusLabel(transform.position, TrashMat.color, Size);
            _ballSquirmTimer = _trashBallSquirmTime;
            AudioManager.Instance.PlayOnInstance(gameObject, "enemyPickup");
            return true;
            
        }
        return false;
    }

    public void OnTrashBallRelease(TrashBall trashBall, Vector2 unitVectorAngle)
    {
        gameObject.SetActive(true);
        
        List<Collider2D> colliders = new List<Collider2D>();
        Rigidbody.GetAttachedColliders(colliders);
        foreach (Collider2D collider in colliders)
        {
            Physics2D.IgnoreCollision(collider, trashBall.Collider, true);
            Physics2D.IgnoreCollision(collider, trashBall.MagnetCollider, true);
        }

        transform.position = trashBall.transform.position;

        transform.localScale = Vector3.zero;
        transform.DOScale(Vector3.one, 0.2f).SetEase(Ease.OutQuad);

        StartCoroutine(ExplodeOutOfBall(trashBall, unitVectorAngle));
    }


    private IEnumerator ExplodeOutOfBall(TrashBall trashBall, Vector2 unitVectorAngle)
    {
        int size = trashBall.Size; //store size early for safety

        // Wait a frame to ensure collision is ignored, then explode out of the ball
        yield return new WaitForEndOfFrame();
        Rigidbody.simulated = true;
        // This is a sloppy way of doing it... but it should properly keep magnitude the same as before while letting ball control the angle
        float explosionForce = (float)(Math.Sqrt(size) * _trashBallEscapeForce);
        float randomForce = new Vector2(UnityEngine.Random.Range(-explosionForce, explosionForce), UnityEngine.Random.Range(-explosionForce, explosionForce)).magnitude;
        Rigidbody.linearVelocity = randomForce * unitVectorAngle;

        yield return new WaitForSeconds(0.3f);
        _state.ChangeState(EnemyStateEnum.Default);
        if (trashBall != null && trashBall.isActiveAndEnabled)
        {
            List<Collider2D> colliders = new List<Collider2D>();
            Rigidbody.GetAttachedColliders(colliders);
            foreach (Collider2D collider in colliders) {
                Physics2D.IgnoreCollision(collider, trashBall.Collider, false);
                Physics2D.IgnoreCollision(collider, trashBall.MagnetCollider, false);
            }
        }

    }

    public void OnTrashBallDestroy()
    {
        if(_parentRoom != null) _parentRoom.ObjectCleaned(this);

        OnDestroy?.Invoke();
        Destroy(gameObject);
    }

    // Update method for trashball
    public void TrashBallUpdate(TrashBall trashBall)
    {

        if (_ballSquirmTimer <= 0)
        {
            float angle = UnityEngine.Random.Range(0f, 2f * Mathf.PI);
            Vector2 randomDir = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)).normalized;

            trashBall.TakeDamage(_trashBallSquirmDamage, _trashBallSquirmForce, randomDir);
            _ballSquirmTimer += _trashBallSquirmTime;
        }
        _ballSquirmTimer -= Time.deltaTime;
    }

    // ICleanable
    public void SetRoom(Room room)
    {
        _parentRoom = room;
    }

    // ISwipeable
    public void OnSwipe(Vector2 direction, float force, Collider2D collider)
    {
        //if swipe is processed as vulnerable
        _state.ChangeState(EnemyStateEnum.Pinball);

        //pinball affects swipe more
        Rigidbody.AddForce(direction * force, ForceMode2D.Impulse);
    }

    // IPokeable
    public void OnPoke(Vector2 direction, float force, Collider2D collider)
    {
        //pinball affects poke more
        Rigidbody.AddForce(direction * force, ForceMode2D.Impulse);
    }    

    // ISweepable
    public void OnSweep(Vector2 position, Vector2 direction, float force)
    {
        //if sweep is processed as pinball
        Vector2 springForce = direction * force;
        Vector2 dampingForce = -Rigidbody.linearVelocity * 4f;
        Rigidbody.AddForce(springForce + dampingForce, ForceMode2D.Force);
    }
}