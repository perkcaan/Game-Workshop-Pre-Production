using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using System;
using FMOD.Studio;
using FMODUnity;
using Cinemachine.Utility;
using Unity.PlasticSCM.Editor.WebApi;

// Primary script for TrashBall gameobject. Acts as a container for IAbsorbable (primarily Trash).
public class TrashBall : MonoBehaviour, ISweepable, ISwipeable, IHeatable
{
    #region Fields/Properties

    [Header("Base Stats")]
    [SerializeField, ReadOnly] private int _size = 1;
    public int Size
    {
        get { return _size; }
        set { SetSize(value); }
    }

    [SerializeField] float _baseMaxHealth = 30f;
    [SerializeField] float _scaleMultiplier = 0.5f;
    [SerializeField] float _maxScale = 5f;

    [Header("Ball Decay Properties")]
    [SerializeField] float _idleDecayMultiplier;
    [SerializeField] float _timeUntilDecay = 30f;
    [SerializeField] float _decayTrashDropRate;


    [Header("Sweep Properties")]
    [SerializeField] float _vacuumForce = 1f;
    [SerializeField] float _minimumVacuumForce = 0.2f;
    [SerializeField] float _sizeSweepMultiplier = 1.2f;

    [Header("Trash Material Properties")]
    [SerializeField, ReadOnly] TrashMaterial _primaryTrashMaterial;
    [SerializeField, ReadOnly] TrashMaterial _secondaryTrashMaterial;
    [SerializeField] private TrashMaterial _baseMaterial;
    [SerializeField] private TrashMaterial _genericMaterial;
    [SerializeField] float _dominantThreshold = 8.5f;
    [SerializeField] float _primaryThreshold = 0.5f;
    [SerializeField] float _secondaryThreshold = 0.15f;

    // stats
    [Header("Modified Stats")]
    [SerializeField, ReadOnly] private float _maxHealth;
    [SerializeField, ReadOnly] private float _health;


    // state of trashball
    private static int _nextID = 0; // universal count for trash ball IDs
    private int _trashID; //individual identifier for this trash ball
    private bool _isBeingDestroyed = false;
    private bool _activelyDecaying = false;
    private float _decayTimer = 0f;

    // material stats
    private float _decayMultiplier = 0f;
    private float _damageMultiplier = 0f;
    private float _swipeForceMultiplier = 0f;

    // absorbed container data
    public List<IAbsorbable> AbsorbedObjects { get; private set; } = new List<IAbsorbable>();
    private List<TrashMaterial> _trashMaterialCounts = new List<TrashMaterial>();
    private List<int> _trashMaterialSize = new List<int>();

    // particles
    private float _particleTimer = 0f;

    // sound
    private EventInstance _sweepSoundInstance;

    // Components
    public Rigidbody2D Rigidbody { get; private set; }
    public CircleCollider2D Collider { get; private set; }
    [SerializeField] private Transform _ballTransform; // Reference to the transform of the Ball Mesh
    public CircleCollider2D AbsorbCollider { get; private set; }
    public CircleCollider2D MagnetCollider { get; private set; }
    [SerializeField] private SizeLabel _label;

    #endregion

    #region Unity methods
    private void Awake()
    {
        Rigidbody = GetComponent<Rigidbody2D>();
        Collider = GetComponent<CircleCollider2D>();
        AbsorbCollider = GetComponentInChildren<TrashBallAbsorb>().GetComponent<CircleCollider2D>();
        MagnetCollider = GetComponentInChildren<TrashBallMagnet>().GetComponent<CircleCollider2D>();

        _trashID = _nextID++;
        _maxHealth = _baseMaxHealth;
        _health = _maxHealth;
        _decayTimer = _timeUntilDecay;
        _primaryTrashMaterial = _baseMaterial;
        _secondaryTrashMaterial = _baseMaterial;
        Rigidbody.sharedMaterial = Instantiate(Rigidbody.sharedMaterial);
    }

    private void Start()
    {
        RuntimeManager.AttachInstanceToGameObject(_sweepSoundInstance, gameObject, Rigidbody);
        AudioManager.Instance.Play("TrashBall", transform);
        _label.Show();
    }

    private void Update()
    {
        if (_isBeingDestroyed) return;

        // Material
        ActionOnMaterials((material, amount) => material.whenBallRolls(this, amount));

        // Particles
        _particleTimer -= Time.deltaTime * Rigidbody.velocity.magnitude / 10f;
        if (_particleTimer <= 0 && Rigidbody.velocity.magnitude > 0.5f)
        {
            _particleTimer = 0.1f;
            ParticleManager.Instance.Play("TrashDustTrail", transform.position, Quaternion.identity, force: Mathf.Pow(Size, 1f / 3f));
        }

        // Sound
        AudioManager.Instance.ModifyParameter(gameObject,"TrashBall","RPM2", Rigidbody.velocity.magnitude * 10);
        FMOD.ATTRIBUTES_3D attributes = RuntimeUtils.To3DAttributes(gameObject, Rigidbody);
        _sweepSoundInstance.set3DAttributes(attributes);

        // 3D Ball rotation
        Vector3 rotationAxis = new Vector3(Rigidbody.velocity.y, -Rigidbody.velocity.x, 0);
        float distance = Rigidbody.velocity.magnitude * Time.deltaTime;
        float circumference = 2 * Mathf.PI * _ballTransform.localScale.x;
        float rotationAngle = (distance / circumference) * 360f;
        _ballTransform.Rotate(rotationAxis, rotationAngle, Space.World);

        // Decay
        if (Rigidbody.velocity.magnitude < 1)
        {
            _decayTimer -= Time.deltaTime * _idleDecayMultiplier * _decayMultiplier;
            if (_decayTimer < 0)
            {
                DegradeTrashBall();
                _decayTimer += _decayTrashDropRate;
            }
        }
    }

    private void FixedUpdate()
    {
        Collider.radius = _ballTransform.localScale.x * 0.5f;
    }

    #endregion

    #region Public methods

    public void AbsorbObject(IAbsorbable absorbable)
    {
        if (_isBeingDestroyed) return;
        MonoBehaviour absorbableMono = absorbable as MonoBehaviour;
        if (absorbableMono == null) return;

        bool isPlayer = false;
        if (absorbable is PlayerMovementController player)
        {
            isPlayer = true;
        }

        ScoreBehavior.SendScore?.Invoke(0);

        AbsorbedObjects.Add(absorbable);
        if (Rigidbody.simulated && !isPlayer)
        {
            AbsorbAnimation(absorbableMono.gameObject);
        }

        ActionOnMaterials((material, amount) => material.whenAbsorbTrash(this, amount));

        Size += absorbable.Size;
        DistrictManager.Instance.FocusedRoom.NewTrashBallSize(Size);
        _health = _maxHealth = Size + _baseMaxHealth;

        if (absorbable.TrashMat == null) return;
        for (int i = 0; i < _trashMaterialCounts.Count; i++)
        {
            if (absorbable.TrashMat == _trashMaterialCounts[i])
            {
                _trashMaterialSize[i] += absorbable.Size * absorbable.TrashMatWeight;
                UpdateMaterial();
                return;
            }
        }
        _trashMaterialCounts.Add(absorbable.TrashMat);
        _trashMaterialSize.Add(absorbable.Size * absorbable.TrashMatWeight);
        UpdateMaterial();
    }

    public void TakeDamage(int damage)
    {
        if (_isBeingDestroyed) return;

        float damageTaken = damage * _damageMultiplier;
        _health -= damageTaken;
        if (_health <= 0) ExplodeTrashBall();
        else DegradeTrashBall();
    }

    // Puts the trashball into a destroyed but still around state. (For animations)
    public void PrepareDelete()
    {
        foreach (IAbsorbable absorbable in AbsorbedObjects)
        {
            absorbable.OnTrashBallDestroy(); // notify absorbed object
        }

        _isBeingDestroyed = true;
        AbsorbedObjects.Clear();
        ScoreBehavior.SendScore?.Invoke(Size);
    }

    // Removes the trash ball entirely
    public void Delete()
    {
        AudioManager.Instance.ModifyGlobalParameter("RPM", 0f);
        AudioManager.Instance.Stop(gameObject,"TrashBall");
        Destroy(gameObject);
    }

    #endregion


    #region Private methods

    private void SetDecaying(bool isDecaying)
    {
        if (_isBeingDestroyed) return;
        if (isDecaying == _activelyDecaying) return;

        _activelyDecaying = isDecaying;
        LayerMask mask = Rigidbody.excludeLayers;
        int trashBit = 1 << LayerMask.NameToLayer("Trash");
        int enemyBit = 1 << LayerMask.NameToLayer("Enemy");
        int combinedBit = trashBit | enemyBit;

        if (isDecaying)
        {
            mask |= combinedBit;
        }
        else
        {
            mask &= ~combinedBit;
        }
        Rigidbody.excludeLayers = mask;

        //AudioManager.Instance.Play("Decay", transform.position);
        //AudioManager.Instance.ModifyParameter("Decay", "Size", Size, "Global");
    }

    // Removes a single random item from AbsorbedTrash
    private void DegradeTrashBall()
    {
        SetDecaying(true);

        if (AbsorbedObjects.Count <= 1)
        {
            ExplodeTrashBall();
            return;
        }

        bool _needTrash = true;
        int randomTrashRemove = -1;
        while (_needTrash) // get random trash, ensure its not player
        {
            randomTrashRemove = UnityEngine.Random.Range(0, AbsorbedObjects.Count);
            if ((AbsorbedObjects[randomTrashRemove] as MonoBehaviour).TryGetComponent(out PlayerMovementController player)) continue;
            _needTrash = false;
        }


        for (int i = 0; i < _trashMaterialCounts.Count; i++)
        {
            if (AbsorbedObjects[randomTrashRemove].TrashMat == _trashMaterialCounts[i])
            {
                _trashMaterialSize[i] -= AbsorbedObjects[randomTrashRemove].Size * AbsorbedObjects[randomTrashRemove].TrashMatWeight;
                break;
            }
        }
        Size -= AbsorbedObjects[randomTrashRemove].Size;
        AbsorbedObjects[randomTrashRemove].OnTrashBallRelease(this);
        AbsorbedObjects.Remove(AbsorbedObjects[randomTrashRemove]);
    }

    // Removes all Trash Ball objects remaining
    private void ExplodeTrashBall()
    {
        if (_isBeingDestroyed) return;

        foreach (IAbsorbable absorbable in AbsorbedObjects)
        {
            MonoBehaviour trashMono = absorbable as MonoBehaviour;
            trashMono.gameObject.SetActive(true);
            absorbable.OnTrashBallRelease(this);
        }
        AudioManager.Instance.ModifyGlobalParameter("RPM", 0f);
        AudioManager.Instance.Stop(gameObject, "TrashBall");
        AudioManager.Instance.Stop(gameObject, "Decay");
        Destroy(gameObject);
    }

    //On size change, update size and scale
    private void SetSize(int newSize)
    {
        if (_isBeingDestroyed) return;

        _size = newSize;
        _label.UpdateSizeLabel(_size);

        // Formula for size -> scale
        // scale = CubeRoot(size) * multiplier
        float scale = _scaleMultiplier * Mathf.Pow(Size, 1f / 3f);
        float newScale = Mathf.Min(scale, _maxScale);
        _ballTransform.DOScale(new Vector3(newScale, newScale, newScale), 0.25f).SetLink(gameObject);
    }

    private void OnTrashBallMerge(TrashBall otherTrashBall)
    {
        if (_isBeingDestroyed || otherTrashBall._isBeingDestroyed) return;
        if (!otherTrashBall.isActiveAndEnabled) return;
        foreach (IAbsorbable absorbable in otherTrashBall.AbsorbedObjects)
        {
            if (absorbable.OnAbsorbedByTrashBall(this, Vector2.zero, 0, true))
            {
                AbsorbObject(absorbable);
            }

        }


        // particles
        Vector2 direction = otherTrashBall.transform.position - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion particleRotation = Quaternion.Euler(0f, 0f, angle - 45);
        if (otherTrashBall.Size > 10)
        {
            ParticleManager.Instance.Play("TrashSwiped", transform.position, particleRotation, force: 1f);
        }
        else
        {
            ParticleManager.Instance.Play("TrashSwiped", transform.position, particleRotation, force: 0.5f);
        }

        otherTrashBall._isBeingDestroyed = true;
        otherTrashBall.AbsorbedObjects.Clear();

        otherTrashBall.Rigidbody.simulated = false;

        Sequence seq = DOTween.Sequence();
        seq.SetLink(otherTrashBall.gameObject);
        seq.Join(otherTrashBall._ballTransform.DOScale(Vector3.zero, 0.2f).SetEase(Ease.InQuad));
        seq.Join(otherTrashBall.transform.DOMove(transform.position, 0.3f).SetEase(Ease.InQuad));
        seq.OnComplete(() => Destroy(otherTrashBall?.gameObject));
    }

    // Animation of thing getting sucked up.
    private void AbsorbAnimation(GameObject absorbedObject)
    {
        if (absorbedObject == null) return;
        DOTween.Kill(absorbedObject.transform);
        Sequence absorbSequence = DOTween.Sequence();
        absorbSequence.SetLink(absorbedObject);
        absorbSequence.Join(absorbedObject.transform.DOScale(Vector3.zero, 0.2f).SetEase(Ease.InQuad));
        absorbSequence.Join(absorbedObject.transform.DOMove(transform.position, 0.3f).SetEase(Ease.InQuad));
        absorbSequence.OnKill(() => absorbedObject?.SetActive(false));
    }

    private void UpdateMaterial()
    {
        TrashMaterial _newPrimaryTrashMaterial = _genericMaterial;
        TrashMaterial _newSecondaryTrashMaterial = _genericMaterial;

        // Get total Trash Material Size
        float totalSize = 0f;
        for (int i = 0; i < _trashMaterialSize.Count; i++)
        {
            totalSize += _trashMaterialSize[i];
        }

        // Get highest and second highest percents
        float highestPercent = _primaryThreshold;
        float secondHighestPercent = _secondaryThreshold;
        for (int i = 0; i < _trashMaterialSize.Count; i++)
        {
            float percent = _trashMaterialSize[i] / totalSize;
            if (percent > highestPercent)
            {
                if (highestPercent != _primaryThreshold)
                {
                    // Highest > Second Highest
                    secondHighestPercent = highestPercent;
                    _newSecondaryTrashMaterial = _newPrimaryTrashMaterial;
                }
                // New Highest
                highestPercent = percent;
                _newPrimaryTrashMaterial = _trashMaterialCounts[i];
            }
            else if (_trashMaterialSize[i] / Size > secondHighestPercent)
            {
                // New Second Highest
                secondHighestPercent = percent;
                _newSecondaryTrashMaterial = _trashMaterialCounts[i];
            }
        }

        // Alert trash materials when they end
        if (_primaryTrashMaterial != _newPrimaryTrashMaterial)
        {
            if (_primaryTrashMaterial != null) _primaryTrashMaterial.materialEnd(this);
        }
        if (_secondaryTrashMaterial != _newSecondaryTrashMaterial)
        {
            if (_secondaryTrashMaterial != null) _secondaryTrashMaterial.materialEnd(this);
        }

        _primaryTrashMaterial = _newPrimaryTrashMaterial;
        _secondaryTrashMaterial = _newSecondaryTrashMaterial;

        // Apply Trash material
        ResetTrashMaterial();
        if (highestPercent > _dominantThreshold)
        {
            ApplyTrashMaterial(_primaryTrashMaterial, 1f);
        }
        else
        {
            ApplyTrashMaterial(_primaryTrashMaterial, 0.66f);
            ApplyTrashMaterial(_secondaryTrashMaterial, 0.33f);
        }

        // Update trash material on audio
        if (_primaryTrashMaterial == null)
        {
            AudioManager.Instance.ModifyGlobalParameter("Generic", highestPercent);
            return;
        }
        AudioManager.Instance.ModifyGlobalParameter(_primaryTrashMaterial.name, highestPercent);

        // Update label color
        _label.SetColor(_primaryTrashMaterial.color);
    }

    // Resets trash material to base
    private void ResetTrashMaterial()
    {
        Rigidbody.sharedMaterial.bounciness = _baseMaterial.bounciness;
        Rigidbody.drag = _baseMaterial.drag;
        Rigidbody.mass = _baseMaterial.mass;
        _decayMultiplier = _baseMaterial.decayMultiplier;
        _damageMultiplier = _baseMaterial.damageMultiplier;
        _swipeForceMultiplier = _baseMaterial.swipeForceMultiplier;

        float currentHealthPercent = _health / _maxHealth;
        _maxHealth = _baseMaxHealth;
        _health = currentHealthPercent * _maxHealth;
    }

    // Apply the material properties to the trash ball. 1f - Dominant, 0.66 - Primary, 0.33 - 
    private void ApplyTrashMaterial(TrashMaterial material, float percentOf)
    {

        Rigidbody.sharedMaterial.bounciness += material.bounciness * percentOf;
        Rigidbody.drag += material.drag * percentOf;
        Rigidbody.mass += material.mass * percentOf;
        _decayMultiplier += material.decayMultiplier * percentOf;
        _damageMultiplier += material.damageMultiplier * percentOf;
        _swipeForceMultiplier += material.swipeForceMultiplier * percentOf;

        float currentHealthPercent = _health / _maxHealth;
        _maxHealth += material.ballMaxHealthChange * percentOf;
        _health = currentHealthPercent * _maxHealth;
    }


    private void ActionOnMaterials(Action<TrashMaterial, TrashMaterialAmount> actionToPerform)
    {
        if (_secondaryTrashMaterial == _genericMaterial)
        {
            actionToPerform?.Invoke(_primaryTrashMaterial, TrashMaterialAmount.Dominant);
        }
        else
        {
            actionToPerform?.Invoke(_primaryTrashMaterial, TrashMaterialAmount.Primary);
            actionToPerform?.Invoke(_secondaryTrashMaterial, TrashMaterialAmount.Secondary);
        }
    }

    #endregion

    #region Collision

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (_isBeingDestroyed) return;

        if (collision.gameObject.TryGetComponent(out Wall wall))
        {
            ActionOnMaterials((material, amount) => material.whenBallHitsWall(this, amount));
        }
    }

    public void OnAbsorbTrigger(Collider2D collider)
    {
        if (_isBeingDestroyed) return;

        if (collider.gameObject.TryGetComponent(out IAbsorbable absorbableObject))
        {
            if (_activelyDecaying) return;
            if (absorbableObject.OnAbsorbedByTrashBall(this, Rigidbody.velocity, Size, false))
            {
                AbsorbObject(absorbableObject);
            }
            _health += Mathf.Min(absorbableObject.Size, _maxHealth); //health 
            _decayTimer = _timeUntilDecay;
            return;
        }

        if (collider.TryGetComponent(out TrashBall otherTrashBall))
        {
            if (otherTrashBall._isBeingDestroyed) return;

            if (otherTrashBall == null || gameObject == null) return;
            if (!otherTrashBall.isActiveAndEnabled || !isActiveAndEnabled) return;
            _health = Mathf.Min((_health + otherTrashBall._health) * 0.5f, _maxHealth);
            _decayTimer = _timeUntilDecay;

            if (Size > otherTrashBall.Size)
            {
                OnTrashBallMerge(otherTrashBall);
                return;
            }
            else if (Size < otherTrashBall.Size) return;

            if (Rigidbody.velocity.magnitude > otherTrashBall.Rigidbody.velocity.magnitude)
            {
                OnTrashBallMerge(otherTrashBall);
                return;
            }
            else if (Rigidbody.velocity.magnitude < otherTrashBall.Rigidbody.velocity.magnitude) return;

            // If same size and speed, force winner to be based on the arbitrary TrashId
            if (_trashID > otherTrashBall._trashID)
            {
                OnTrashBallMerge(otherTrashBall);
                return;
            }
        }
    }

    public void OnMagnetStay(Collider2D collider)
    {
        if (_isBeingDestroyed) return;

        if (collider.TryGetComponent(out Trash trash))
        {
            if (trash._rigidBody == null) return;
            Vector2 targetDirection = ((Vector2)trash.transform.position - (Vector2)transform.position).normalized;
            trash._rigidBody.AddForce(targetDirection * -1f);
        }
    }


    #endregion


    #region Interfaces
    //IHeatable
    public void PrepareIgnite(HeatMechanic heat)
    {
        _label.Hide();
        PrepareDelete();
    }

    public void OnIgnite(HeatMechanic heat)
    {
        ActionOnMaterials((material, amount) => material.whenBallIgnite(this, amount));

        Delete(); // Deletes the trash object
    }

    //ISweepable
    public void OnSweep(Vector2 center, Vector2 direction, float force)
    {
        if (_isBeingDestroyed) return;
        SetDecaying(false);
        _decayTimer = _timeUntilDecay;

        Vector3 centerPoint = center + (direction * Mathf.Pow(Size, 1f / 3f) / Mathf.PI);
        float distance = Vector2.Distance(transform.position, centerPoint);
        float newForce = force * distance * (_minimumVacuumForce + (_vacuumForce / Size * _sizeSweepMultiplier));
        Vector2 directionToCenterPoint = (centerPoint - transform.position).normalized;
        Rigidbody.AddForce(directionToCenterPoint * newForce, ForceMode2D.Force);
    }

    //ISwipeable
    public void OnSwipe(Vector2 direction, float force, Collider2D collision)
    {
        if (_isBeingDestroyed) return;

        ActionOnMaterials((material, amount) => material.whenBallSwiped(this, amount));

        SetDecaying(false);
        _health = _maxHealth;
        _decayTimer = _timeUntilDecay;

        Rigidbody.AddForce(direction * force * _swipeForceMultiplier, ForceMode2D.Impulse);

        // particles
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Vector3 contactPoint = collision.ClosestPoint(transform.position);
        Quaternion particleRotation = Quaternion.Euler(0, 0, angle);
        float sizeForce = Mathf.Pow(Size, 1f / 5f) - 0.5f;
        ParticleManager.Instance.Play("TrashSwiped", transform.position, particleRotation, force: sizeForce);
        ParticleManager.Instance.Play("ImpactLines", contactPoint, particleRotation, force: 1.4f);
        ParticleManager.Instance.Play("ImpactCircleS", contactPoint, force: 1.25f);

    }

    #endregion
}
