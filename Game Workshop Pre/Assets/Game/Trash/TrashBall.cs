using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using System;
using FMOD.Studio;
using FMODUnity;
using Cinemachine.Utility;

public class TrashBall : MonoBehaviour, ISweepable, ISwipeable, IHeatable
{
    [Header("Base Stats")]
    [SerializeField] float _scaleMultiplier;
    [SerializeField] float _baseMaxHealth;
    [SerializeField] float _minimumSpeedToAbsorbPlayer;
    [SerializeField] private int _size = 1;
    public int Size
    {
        get { return _size; }
        set
        {
            _size = value;
            SetSize();
        }
    }

    [Header("Decay Properties")]

    [SerializeField] float _idleDecayMultiplier;
    [SerializeField] float _decayTrashDropRate;

    [Header("OnSweep Properties")]
    [SerializeField] float _vacuumForce = 1f;
    [SerializeField] float _minimumVacuumForce= 0.2f;
    [SerializeField] float _sizeMultiplier= 1.2f;

    [Header("Trash Material Properties")]
    [SerializeField] TrashMaterial _baseMaterial;
    [SerializeField] TrashMaterial _genericMaterial;
    [SerializeField] float _dominantThreshold;
    [SerializeField] float _primaryThreshold;
    [SerializeField] float _secondaryThreshold;
    private bool _isBeingDestroyed = false;
    private float _maxHealth;
    private float _health;
    private bool _activelyDecaying = false;
    private float _damageMultiplier;
    private float _decayMultiplier;
    private float _sizeToAbsorbChange;
    private float _swipeForceMultiplier;
    private float _knockbackMultiplier;

    public List<IAbsorbable> absorbedObjects = new List<IAbsorbable>();
    private List<Trash> absorbedTrash = new List<Trash>();
    private List<TrashMaterial> _trashMaterialCounts = new List<TrashMaterial>();
    private List<int> _trashMaterialSize = new List<int>();
    private TrashMaterial _primaryTrashMaterial;
    private TrashMaterial _secondaryTrashMaterial;
    private PhysicsMaterial2D _physicsMaterial2D;
    private EventInstance _sweepSoundInstance;
    public PlayerMovementController _movementController;

    private FMODUnity.StudioEventEmitter _emitter;

    // Trash IDs to solve trash merge ties
    private static int _nextId = 0;
    public int TrashId { get; private set; }

    public static Action<int> SendScore;

    [SerializeField] Transform _circleBorder;
    public CircleCollider2D magnetTrashCollider;
    public Rigidbody2D rigidBody;
    MeshRenderer _meshRenderer;
    float _particleTimer = 0f;
    void SetSize()
    {
        if (_isBeingDestroyed) return;
        float newSize = _scaleMultiplier * Mathf.Pow(Size, 1f / 3f);
        transform.DOScale(new Vector3(newSize, newSize, newSize), 0.25f).SetLink(gameObject);
        _circleBorder.localScale = new Vector3(1 + (0.15f / newSize), 1 + (0.15f / newSize), 1 + (0.15f / newSize));
    }

    public void Awake()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        _meshRenderer = GetComponentInChildren<MeshRenderer>();
        TrashId = _nextId++;
        _maxHealth = _baseMaxHealth;
        _health = _maxHealth;
        _primaryTrashMaterial = _baseMaterial;
        _secondaryTrashMaterial = _baseMaterial;
        _physicsMaterial2D = Instantiate(rigidBody.sharedMaterial);
        //_sweepSoundInstance = RuntimeManager.CreateInstance("event:/TrashBall/TrashBall");
        _emitter = GetComponent<StudioEventEmitter>();
    }

    public void Start()
    {
        //RuntimeManager.AttachInstanceToGameObject(_sweepSoundInstance, this.gameObject, rigidBody);
        AudioManager.Instance.Play("TrashBall",transform.position);
        //AudioManager.Instance.Play("Ignite", transform.position);
    }

    public void Update()
    {
        if (_isBeingDestroyed) return;

        if (_secondaryTrashMaterial == _genericMaterial)
        _primaryTrashMaterial.whenBallRolls(this, TrashMaterialAmount.Dominant);
        else
        _primaryTrashMaterial.whenBallRolls(this, TrashMaterialAmount.Primary);
        _secondaryTrashMaterial.whenBallRolls(this, TrashMaterialAmount.Secondary);

        _particleTimer -= Time.deltaTime * rigidBody.velocity.magnitude / 10f;
        if (_particleTimer <= 0 && rigidBody.velocity.magnitude > 0.5f)    
        {
            _particleTimer = 0.1f;
            ParticleManager.Instance.Play("TrashDustTrail", transform.position, Quaternion.identity, force: Mathf.Pow(Size, 1f / 3f));
        }
        
        AudioManager.Instance.ModifyParameter("TrashBall", "RPM", rigidBody.velocity.magnitude * 10, "Global");
        // _emitter.Play();

        // Trash ball rotation
        Vector3 rotationAxis = new Vector3(rigidBody.velocity.y, -rigidBody.velocity.x, 0);
        float distance = rigidBody.velocity.magnitude * Time.deltaTime;
        float circumference = 2 * Mathf.PI * transform.localScale.x;
        float rotationAngle = (distance / circumference) * 360f;
        _meshRenderer.transform.Rotate(rotationAxis, rotationAngle, Space.World);
        _circleBorder.Rotate(rotationAxis, rotationAngle, Space.World);

        FMOD.ATTRIBUTES_3D attributes = RuntimeUtils.To3DAttributes(gameObject, rigidBody);
        _sweepSoundInstance.set3DAttributes(attributes);

        if (rigidBody.velocity.magnitude < 1)
        {
            _health -= Time.deltaTime * _idleDecayMultiplier * _decayMultiplier;
            if (_health < 0)
            {
                DegradeTrashBall();
                _health += _decayTrashDropRate;
            }
        }
    }

    public void OnSweep(Vector2 center, Vector2 direction, float force)
    {
        if (_isBeingDestroyed) return;
        SetDecaying(false);
        _health = _maxHealth;

        Vector3 centerPoint = center + (direction * Mathf.Pow(Size, 1f / 3f) / Mathf.PI);
        float distance = Vector2.Distance(transform.position, centerPoint);
        float newForce = force * distance * (_minimumVacuumForce + (_vacuumForce / Size * _sizeMultiplier));
        Vector2 directionToCenterPoint = (centerPoint - transform.position).normalized;
        rigidBody.AddForce(directionToCenterPoint * newForce, ForceMode2D.Force);
    }

    public void OnSwipe(Vector2 direction, float force, Collider2D collision)
    {
        if (_isBeingDestroyed) return;

        if (_secondaryTrashMaterial == _genericMaterial)
        _primaryTrashMaterial.whenBallSwiped(this, TrashMaterialAmount.Dominant);
        else
        _primaryTrashMaterial.whenBallSwiped(this, TrashMaterialAmount.Primary);
        _secondaryTrashMaterial.whenBallSwiped(this, TrashMaterialAmount.Secondary);

        SetDecaying(false);
        _health = _maxHealth;
        rigidBody.AddForce(direction * force, ForceMode2D.Impulse);

        // particles
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Vector3 contactPoint = collision.ClosestPoint(transform.position);
        Quaternion particleRotation = Quaternion.Euler(0, 0, angle);
        float sizeForce = Mathf.Pow(Size, 1f / 5f) - 0.5f;
        ParticleManager.Instance.Play("TrashSwiped", transform.position, particleRotation, force: sizeForce);
        ParticleManager.Instance.Play("ImpactLines", contactPoint, particleRotation, force: 1.4f);
        ParticleManager.Instance.Play("ImpactCircleS", contactPoint, force: 1.25f);
    }

    public void TakeDamage(int damage)
    {
        if (_isBeingDestroyed) return;
        float damageTaken = damage * _damageMultiplier;
        _health -= damageTaken;
        _maxHealth -= damageTaken;
        if (_health < 0) ExplodeTrashBall();
        else DegradeTrashBall();
    }

    public void DegradeTrashBall()
    {
        SetDecaying(true);

        if (absorbedTrash.Count <= 1)
        {
            ExplodeTrashBall();
            return;
        }
        int randomTrashRemove = UnityEngine.Random.Range(0, absorbedTrash.Count);

        for (int i = 0; i < _trashMaterialCounts.Count; i++)
        {
            if (absorbedTrash[randomTrashRemove].trashMaterial == _trashMaterialCounts[i])
            {
                _trashMaterialSize[i] -= absorbedTrash[randomTrashRemove].Size * absorbedTrash[randomTrashRemove].trashMaterialWeight;
                break;
            }
        }
        Size -= absorbedTrash[randomTrashRemove].Size;
        absorbedTrash[randomTrashRemove].OnTrashBallExplode(this);
        absorbedObjects.Remove(absorbedTrash[randomTrashRemove]);
        absorbedTrash.RemoveAt(randomTrashRemove);
    }

    private void SetDecaying(bool isDecaying)
    {
        if (_isBeingDestroyed) return;

        if (isDecaying == _activelyDecaying) return;
        _activelyDecaying = isDecaying;
        LayerMask mask = rigidBody.excludeLayers;
        int trashBit = 1 << LayerMask.NameToLayer("Trash");

        if (isDecaying) mask |= trashBit;
        else mask &= ~trashBit;
        rigidBody.excludeLayers = mask;

        //AudioManager.Instance.Play("Decay", transform.position);
        //AudioManager.Instance.ModifyParameter("Decay", "Size", Size, "Global");
    }

    public void AbsorbTrash(Trash trash)
    {
        if (_isBeingDestroyed) return;

        SendScore?.Invoke(0);

        if (_secondaryTrashMaterial == _genericMaterial)
        _primaryTrashMaterial.whenAbsorbTrash(this, TrashMaterialAmount.Dominant);
        else
        _primaryTrashMaterial.whenAbsorbTrash(this, TrashMaterialAmount.Primary);
        _secondaryTrashMaterial.whenAbsorbTrash(this, TrashMaterialAmount.Secondary);
        
        absorbedObjects.Add(trash);
        absorbedTrash.Add(trash);
        if (rigidBody.simulated)
        {
            AbsorbAnimation(trash.gameObject);
        }

        Size += trash.Size;
        _health = _maxHealth = Size + _baseMaxHealth;

        for (int i = 0; i < _trashMaterialCounts.Count; i++)
        {
            if (trash.trashMaterial == _trashMaterialCounts[i])
            {
                _trashMaterialSize[i] += trash.Size * trash.trashMaterialWeight;
                CheckMaterial();
                return;
            }
        }
        _trashMaterialCounts.Add(trash.trashMaterial);
        _trashMaterialSize.Add(trash.Size * trash.trashMaterialWeight);
        CheckMaterial();
    }

    void CheckMaterial()
    {
        TrashMaterial _newPrimaryTrashMaterial = _genericMaterial;
        TrashMaterial _newSecondaryTrashMaterial = _genericMaterial;

        _physicsMaterial2D.bounciness = _baseMaterial.bounciness;
        rigidBody.drag = _baseMaterial.drag;
        rigidBody.mass = _baseMaterial.mass;
        _decayMultiplier = _baseMaterial.decayMultiplier;
        _damageMultiplier = _baseMaterial.damageMultiplier;
        _sizeToAbsorbChange = _baseMaterial.sizeToAbsorbChange;
        _swipeForceMultiplier = _baseMaterial.swipeForceMultiplier;
        _knockbackMultiplier = _baseMaterial.knockbackMultiplier;

        float highestPrecent = _primaryThreshold;
        float secondHighestPrecent = _secondaryThreshold;
        for (int i = 0; i < _trashMaterialSize.Count; i++)
        {
            if (_trashMaterialSize[i] / Size > highestPrecent)
            {
                if (highestPrecent != _primaryThreshold)
                {
                    secondHighestPrecent = highestPrecent;
                    _newSecondaryTrashMaterial = _newPrimaryTrashMaterial;
                }
                highestPrecent = _trashMaterialSize[i] / Size;
                _newPrimaryTrashMaterial = _trashMaterialCounts[i];
            }
            else if (_trashMaterialSize[i] / Size > secondHighestPrecent)
            {
                secondHighestPrecent = _trashMaterialSize[i] / Size;
                _newSecondaryTrashMaterial = _trashMaterialCounts[i];
            }
        }

        if (_primaryTrashMaterial != _newPrimaryTrashMaterial)
        {
            if (_primaryTrashMaterial != null)
                _primaryTrashMaterial.materialEnd(this);
        }
        if (_secondaryTrashMaterial != _newSecondaryTrashMaterial)
        {
            if (_secondaryTrashMaterial != null)
                _secondaryTrashMaterial.materialEnd(this);
        }

        _primaryTrashMaterial = _newPrimaryTrashMaterial;
        _secondaryTrashMaterial = _newSecondaryTrashMaterial;
        if (highestPrecent > _dominantThreshold)
        {
            ApplyTrashMaterial(_primaryTrashMaterial, 1f);
        }
        else
        {
            ApplyTrashMaterial(_primaryTrashMaterial, 0.66f);
            ApplyTrashMaterial(_secondaryTrashMaterial, 0.33f);
        }

        if (_primaryTrashMaterial == null)
        {
            AudioManager.Instance.ModifyParameter("TrashBall", "Generic", highestPrecent, "Global");
        }
        AudioManager.Instance.ModifyParameter("TrashBall", _primaryTrashMaterial.name, highestPrecent, "Global");
        //Debug.Log(_primaryTrashMaterial.name+" sound applied");

        rigidBody.sharedMaterial = _physicsMaterial2D;
    }


    private void ApplyTrashMaterial(TrashMaterial material, float precentOf)
    {
        _physicsMaterial2D.bounciness += material.bounciness * precentOf;
        rigidBody.drag += material.drag * precentOf;
        rigidBody.mass += material.mass * precentOf;
        _decayMultiplier += material.decayMultiplier * precentOf;
        _damageMultiplier += material.damageMultiplier * precentOf;
        _sizeToAbsorbChange = _baseMaterial.sizeToAbsorbChange;
        _swipeForceMultiplier = _baseMaterial.swipeForceMultiplier;
        _knockbackMultiplier = _baseMaterial.knockbackMultiplier;
    }

    public void MagnetCollide(Collider2D other)
    {
        if (_isBeingDestroyed) return;

        if (other.gameObject.TryGetComponent(out Trash trash))
        {
            if (trash._rigidBody == null) return;
            Vector2 targetDirection = ((Vector2)trash.transform.position - (Vector2)transform.position).normalized;
            trash._rigidBody.AddForce(targetDirection * -1f);
        }
    }

    public void AbsorbCollide(Collider2D other)
    {
        if (_isBeingDestroyed) return;

        if (other.gameObject.TryGetComponent(out IAbsorbable absorbableObject))
        {
            if (_activelyDecaying) return;
            absorbableObject.OnAbsorbedByTrashBall(this, rigidBody.velocity, (int)(Size + _sizeToAbsorbChange), false);
            _health = _maxHealth;
            return;
        }

        if (other.gameObject.TryGetComponent(out TrashBall otherTrashBall))
        {
            if (otherTrashBall._isBeingDestroyed) return;

            if (otherTrashBall == null || gameObject == null) return;
            if (!otherTrashBall.isActiveAndEnabled || !isActiveAndEnabled) return;
            _health = _maxHealth;

            if (Size > otherTrashBall.Size)
            {
                OnTrashBallMerge(otherTrashBall);
                return;
            }
            else if (Size < otherTrashBall.Size) return;

            if (rigidBody.velocity.magnitude > otherTrashBall.rigidBody.velocity.magnitude)
            {
                OnTrashBallMerge(otherTrashBall);
                return;
            }
            else if (rigidBody.velocity.magnitude < otherTrashBall.rigidBody.velocity.magnitude) return;

            // If same size and speed, force winner to be based on the arbitrary TrashId
            if (TrashId > otherTrashBall.TrashId)
            {
                OnTrashBallMerge(otherTrashBall);
                return;
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (_isBeingDestroyed) return;

        if (other.gameObject.TryGetComponent(out Wall wall))
        {
            if (_secondaryTrashMaterial == _genericMaterial)
            _primaryTrashMaterial.whenBallHitsWall(this, TrashMaterialAmount.Dominant);
            else
            _primaryTrashMaterial.whenBallHitsWall(this, TrashMaterialAmount.Primary);
            _secondaryTrashMaterial.whenBallHitsWall(this, TrashMaterialAmount.Secondary);

            //if ()
            //ParticleManager.Instance.Play("WallCollide", transform.position, particleRotation);
        }

    }

    void OnTrashBallMerge(TrashBall otherTrashBall)
    {
        if (_isBeingDestroyed || otherTrashBall._isBeingDestroyed) return;
        if (!otherTrashBall.isActiveAndEnabled) return;
        foreach (IAbsorbable absorbable in otherTrashBall.absorbedObjects)
        {
            absorbable.OnAbsorbedByTrashBall(this, Vector2.zero, 0, true);
        }

        Vector2 direction = otherTrashBall.transform.position - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion particleRotation = Quaternion.Euler(0f, 0f, angle-45);

        // particles
        if (otherTrashBall.Size > 10)
            ParticleManager.Instance.Play("TrashSwiped", transform.position, particleRotation, force: 1f);
        else
            ParticleManager.Instance.Play("TrashSwiped", transform.position, particleRotation, force: 0.5f);

        otherTrashBall._isBeingDestroyed = true;
        otherTrashBall.absorbedObjects.Clear();
        otherTrashBall.absorbedTrash.Clear();

        otherTrashBall.rigidBody.simulated = false;

        Sequence seq = DOTween.Sequence();
        seq.SetLink(otherTrashBall.gameObject); 
        seq.Join(otherTrashBall.transform.DOScale(Vector3.zero, 0.2f).SetEase(Ease.InQuad));
        seq.Join(otherTrashBall.transform.DOMove(transform.position, 0.3f).SetEase(Ease.InQuad));
        seq.OnComplete(() => Destroy(otherTrashBall?.gameObject));

        //AudioManager.Instance.PlayOnInstance(gameObject,"Trash Pickup");
        //TODO: FIX AUDIO ERRORS
    }

    private void ExplodeTrashBall()
    {
        if (_isBeingDestroyed) return;
        
        foreach (IAbsorbable absorbable in absorbedObjects)
        {
            MonoBehaviour trashMono = absorbable as MonoBehaviour;
            trashMono.gameObject.SetActive(true);
            absorbable.OnTrashBallExplode(this);
        }
        AudioManager.Instance.ModifyParameter("TrashBall", "RPM", 0f, "Global");
        AudioManager.Instance.Stop("TrashBall");
        AudioManager.Instance.Stop("Decay");
        Destroy(gameObject);
    }

    public void PrepareIgnite(HeatMechanic heat)
    {
        _isBeingDestroyed = true;
        //AudioManager.Instance.Play("Ignite", transform.position);
        //AudioManager.Instance.ModifyParameter("Ignite", "Size", (Size / 10), "Global");
        //foreach (Collider2D col in GetComponentsInChildren<Collider2D>()) col.enabled = false;
    }


    public void OnIgnite(HeatMechanic heat)
    {
        if (_secondaryTrashMaterial == _genericMaterial)
        _primaryTrashMaterial.whenBallIgnite(this, TrashMaterialAmount.Dominant);
        else
        _primaryTrashMaterial.whenBallIgnite(this, TrashMaterialAmount.Primary);
        _secondaryTrashMaterial.whenBallIgnite(this, TrashMaterialAmount.Secondary);

        foreach (IAbsorbable absorbable in absorbedObjects)
        {
            absorbable.OnTrashBallIgnite();
        }
        absorbedObjects.Clear();
        absorbedTrash.Clear();
        SendScore?.Invoke((int)Size);
        StartCoroutine(PointSound());

        AudioManager.Instance.ModifyParameter("TrashBall", "RPM", 0f, "Global");
        AudioManager.Instance.Stop("TrashBall");
        Destroy(gameObject);
        
        //AudioManager.Instance.ModifyParameter("Ignite", "Size", (Size / 10), "Global");
    }

    // Non-fire destroy
    public void PrepareDelete()
    {
        _isBeingDestroyed = true;
    }

    // Non-fire destroy
    public void Delete()
    {
        foreach (IAbsorbable absorbable in absorbedObjects)
        {
            absorbable.OnTrashBallIgnite();
        }
        absorbedObjects.Clear();
        absorbedTrash.Clear();
        SendScore?.Invoke((int)Size);
        StartCoroutine(PointSound());

        AudioManager.Instance.ModifyParameter("TrashBall", "RPM", 0f, "Global");
        AudioManager.Instance.Stop("TrashBall");
        Destroy(gameObject);
    }


    private void OnDestroy()
    {
        
    }

    private void AbsorbAnimation(GameObject absorbedObject)
    {
        if (absorbedObject == null) return;
        DOTween.Kill(absorbedObject.transform);
        Sequence absorbSequence = DOTween.Sequence();
        absorbSequence.SetLink(absorbedObject);
        absorbSequence.Join(absorbedObject.transform.DOScale(Vector3.zero, 0.2f).SetEase(Ease.InQuad));
        absorbSequence.Join(absorbedObject.transform.DOMove(transform.position, 0.3f).SetEase(Ease.InQuad));
        absorbSequence.OnKill(() => absorbedObject?.SetActive(false));
        //AudioManager.Instance.PlayOnInstance(gameObject, "Trash Pickup");
        //TODO: FIX AUDIO ERRORS
    }

    public IEnumerator PointSound()
    {
        bool playing = false;

        if (!playing)
        {
            AudioManager.Instance.Play("Points", transform.position);
            yield return new WaitForSeconds(1);


        }


        switch ((int)Size)
        {
            case int n when (n <= 10):
                AudioManager.Instance.ModifyParameter("Points", "Point", 10, "Local");
                Debug.Log("Played Points Sound: 10");
                playing = false;
                break;
            case int n when (n <= 20):
                AudioManager.Instance.ModifyParameter("Points", "Point", 20, "Local");
                Debug.Log("Played Points Sound: 20");
                break;

            case int n when (n <= 30):
                AudioManager.Instance.ModifyParameter("Points", "Point", 30, "Local");
                Debug.Log("Played Points Sound: 30");
                break;

            case int n when (n <= 40):
                AudioManager.Instance.ModifyParameter("Points", "Point", 40, "Local");
                Debug.Log("Played Points Sound: 40");
                break;

            case int n when (n <= 50):
                AudioManager.Instance.ModifyParameter("Points", "Point", 30, "Local");
                Debug.Log("Played Points Sound: 40");
                break;

            default:
                if ((int)Size > 50)
                {
                    AudioManager.Instance.ModifyParameter("Points", "Point", 50, "Local");
                    Debug.Log("Played Points Sound: 50");
                }
                break;
        }
        //soundCooldown = 1f;
    }
}
