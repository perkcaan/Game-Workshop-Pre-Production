using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters;
using AYellowpaper.SerializedCollections;
using DG.Tweening;
using FMOD;
using UnityEngine;
using UnityEngine.Events;

public abstract class EnemyBase : MonoBehaviour, ITargetable, IAbsorbable, IHeatable, ICleanable, ISwipeable, ISweepable, IPokeable
{
    [Header("Enemy")]
    [SerializeField] protected BehaviourTree _behaviour;
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
    [Header("Stunned Properties")]
    [SerializeField] private float _stunTime = 2f;
    public float StunTime { get { return _stunTime; } }
    [Header("Pinball Properties")]
    [SerializeField] EnemyPinballProperties _pinballProps;
    public EnemyPinballProperties PinballProps { get { return _pinballProps; } }
    [Header("Interaction Properties")]
    [SerializeField] EnemyInteractionProperties _interactProps;

    //public TargetType TargetType { get { return TargetType.Enemy; } } //ITargetable

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

    [SerializeField] private TargetType _targetType = TargetType.Enemy;

    public TargetType TargetType
    {
        get => _targetType;
        set => _targetType = value;
    }

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
    protected abstract void ForceCancelAction();

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
        ForceCancelAction();
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
    protected virtual void ModifyAbsorb(ref EnemyAbsorbData data) { }
    public bool OnAbsorbedByTrashBall(TrashBall trashBall, Vector2 ballVelocity, int ballSize, bool forcedAbsorb)
    {
        if (_isDying) return false;
        
        EnemyAbsorbData data = new EnemyAbsorbData()
        {
            CanAbsorb = _interactProps.CanAbsorbBase
        };

        ModifyAbsorb(ref data); // Enemy type modifies sweep data
        _state.ModifyAbsorb(ref data); // State modifies sweep data

        if (forcedAbsorb || (data.CanAbsorb && ballSize >= _minSizeToAbsorb && ballVelocity.magnitude > _minVelocityToAbsorb && isActiveAndEnabled && _rigidbody.simulated))
        {
            _state.ChangeState(EnemyStateEnum.Absorbed);
            _rigidbody.simulated = false;

            if (forcedAbsorb) return true;
            PopupLabel.CreatePlusLabel(transform.position, TrashMat.color, Size);
            _ballSquirmTimer = _trashBallSquirmTime;
            AudioManager.Instance.PlayOnInstance(gameObject, "enemyPickup");
            return true;
            
        }

        // Could do bouncing or any fail reactions here
        return false;
    }

    // Update method for when inside trashball
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

    // ICleanable
    public void SetRoom(Room room)
    {
        _parentRoom = room;
    }

    // ISweepable
    protected virtual void ModifySweep(ref EnemySweepData data) { }

    public void OnSweep(Vector2 position, Vector2 direction, float force)
    {
        EnemySweepData data = new EnemySweepData()
        {
            CanSweep = _interactProps.CanSweepBase 
        };

        ModifySweep(ref data); // Enemy type modifies sweep data
        _state.ModifySweep(ref data); // State modifies sweep data

        //If can't sweep, do nothing
        if (!data.CanSweep) return;

        Vector2 springForce = direction * force;
        Vector2 dampingForce = -Rigidbody.linearVelocity * 4f;
        Rigidbody.AddForce(springForce + dampingForce, ForceMode2D.Force);
    }

    // ISwipeable
    protected virtual void ModifySwipe(ref EnemySwipeData data) { }

    public void OnSwipe(Vector2 direction, float force, Collider2D collider, ref float knockbackMultiplier)
    {
        EnemySwipeData data = new EnemySwipeData
        {
            IsVulnerable = _interactProps.IsBaseVulnerableToSwipe,
            SwipeMultiplier = _interactProps.SwipeBaseMultiplier,
            KnockbackMultiplier = _interactProps.SwipeBaseKnockbackMultiplier
        };

        ModifySwipe(ref data); // Enemy type modifies swipe data
        _state.ModifySwipe(ref data); // State modifies swipe data

        //If swipe is processed as vulnerable, become Pinball
        if (data.IsVulnerable)
        {
            data.SwipeMultiplier *= _interactProps.SwipeVulnerableMultiplier;
            _state.ChangeState(EnemyStateEnum.Pinball);
        }
        
        //Add swipe force
        Rigidbody.AddForce(direction * force * data.SwipeMultiplier, ForceMode2D.Impulse);
        knockbackMultiplier = data.KnockbackMultiplier;
    }


    // IPokeable
    protected virtual void ModifyPoke(ref EnemyPokeData data) { }

    public void OnPoke(Vector2 direction, float force, Collider2D collider, ref float knockbackMultiplier)
    {
        EnemyPokeData data = new EnemyPokeData()
        {
            IsVulnerable = _interactProps.IsBaseVulnerableToPoke,
            PokeMultiplier = _interactProps.PokeBaseMultiplier,
            KnockbackMultiplier = _interactProps.PokeBaseKnockbackMultiplier
        };

        ModifyPoke(ref data); // Enemy type modifies poke data
        _state.ModifyPoke(ref data); // State modifies poke data

        //If poke is processed as vulnerable, become Pinball
        if (data.IsVulnerable)
        {
            data.PokeMultiplier *= _interactProps.PokeVulnerableMultiplier;
            _state.ChangeState(EnemyStateEnum.Pinball);
        }
        
        //Add poke force
        Rigidbody.AddForce(direction * force * data.PokeMultiplier, ForceMode2D.Impulse);
        knockbackMultiplier = data.KnockbackMultiplier;
    }    
}