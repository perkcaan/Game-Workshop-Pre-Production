using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class TrashBall : MonoBehaviour, ISweepable, ISwipeable, IHeatable
{

    [SerializeField] float _scaleMultiplier;
    [SerializeField] float _baseMaxHealth;
    [SerializeField] float _idleDecayMultiplier;
    [SerializeField] float _decayTrashDropRate;
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
    private float _absorbMultiplier;
    public List<IAbsorbable> absorbedObjects = new List<IAbsorbable>();
    private List<Trash> absorbedTrash = new List<Trash>();
    private List<TrashMaterial> _trashMaterialCounts = new List<TrashMaterial>();
    private List<int> _trashMaterialSize = new List<int>();
    private TrashMaterial _primaryTrashMaterial;
    private TrashMaterial _secondaryTrashMaterial;
    private PhysicsMaterial2D _physicsMaterial2D;
    
    // Trash IDs to solve trash merge ties
    private static int _nextId = 0;
    public int TrashId { get; private set; }
    [SerializeField] private float _size = 1f;

    Rigidbody2D _rigidBody;
    MeshRenderer _meshRenderer;
    public float Size
    {
        get { return _size; }
        set
        {
            _size = value;
            SetSize();
        }
    }
    void SetSize()
    {
        float newSize = _scaleMultiplier * Mathf.Pow(Size, 1f / 3f);
        transform.localScale = new Vector3(newSize, newSize, newSize);
    }

    public void Awake()
    {
     
        _rigidBody = GetComponent<Rigidbody2D>();
        _meshRenderer = GetComponentInChildren<MeshRenderer>();
        TrashId = _nextId++;
        _maxHealth = _baseMaxHealth;
        _health = _maxHealth;
        _primaryTrashMaterial = _baseMaterial;
        _primaryTrashMaterial = _baseMaterial;
        _physicsMaterial2D = Instantiate(_rigidBody.sharedMaterial);
    }

    public void Update()
    {
        _primaryTrashMaterial.whenBallRolls();

        // Trash ball rotation
        Vector3 rotationAxis = new Vector3(-_rigidBody.velocity.y, _rigidBody.velocity.x, 0);
        float distance = _rigidBody.velocity.magnitude * Time.deltaTime;
        float circumference = 2 * Mathf.PI * transform.localScale.x;
        float rotationAngle = (distance / circumference) * 360f;
        _meshRenderer.transform.Rotate(rotationAxis, rotationAngle, Space.World);

        if (_rigidBody.velocity.magnitude < 1)
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
        SetDecaying(false);
        _health = _maxHealth;

        Vector3 centerPoint = center + (direction * Mathf.Pow(Size, 1f / 3f) / Mathf.PI);
        float distance = Vector2.Distance(transform.position, centerPoint);
        float newForce = force * distance * (1 + (10 / Size));
        Vector2 directionToCenterPoint = (centerPoint - transform.position).normalized;
        _rigidBody.AddForce(directionToCenterPoint * newForce, ForceMode2D.Force);
    }
    public void OnSwipe(Vector2 direction, float force)
    {
        SetDecaying(false);
        _health = _maxHealth;
        _rigidBody.AddForce(direction * force, ForceMode2D.Impulse);
    }

    public void TakeDamage(int damage)
    {
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
        int randomTrashRemove = Random.Range(0, absorbedTrash.Count);

        for (int i = 0; i < _trashMaterialCounts.Count; i++)
        {
            if (absorbedTrash[randomTrashRemove].trashMaterial == _trashMaterialCounts[i])
            {
                _trashMaterialSize[i] -= absorbedTrash[randomTrashRemove].Size;
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
        if (isDecaying == _activelyDecaying) return;
        _activelyDecaying = isDecaying;
        LayerMask mask = _rigidBody.excludeLayers;
        int trashBit = 1 << LayerMask.NameToLayer("Trash");

        if (isDecaying) mask |= trashBit;
        else mask &= ~trashBit;
        _rigidBody.excludeLayers = mask;
    }

    public void AbsorbTrash(Trash trash)
    {
        absorbedObjects.Add(trash);
        absorbedTrash.Add(trash);
        trash.gameObject.SetActive(false);
        Size += trash.Size;
        _health = _maxHealth = Size + _baseMaxHealth;

        for (int i = 0; i < _trashMaterialCounts.Count; i++)
        {
            if (trash.trashMaterial == _trashMaterialCounts[i])
            {
                _trashMaterialSize[i] += trash.Size;
                CheckMaterial();
                return;
            }
        }
        _trashMaterialCounts.Add(trash.trashMaterial);
        _trashMaterialSize.Add(trash.Size);
        CheckMaterial();
    }

    
    void CheckMaterial()
    {
        _primaryTrashMaterial = _genericMaterial;
        _secondaryTrashMaterial = _genericMaterial;

        _physicsMaterial2D.bounciness = _baseMaterial.bounciness;
        //_spriteRenderer.color = _baseMaterial.color;
        _rigidBody.drag = _baseMaterial.drag;
        _rigidBody.mass = _baseMaterial.mass;
        _decayMultiplier = _baseMaterial.decayMultiplier;
        _damageMultiplier = _baseMaterial.damageMultiplier;
        _absorbMultiplier = _baseMaterial.absorbMultiplier;

        float highestPrecent = _primaryThreshold;
        float secondHighestPrecent = _secondaryThreshold;
        for (int i = 0; i < _trashMaterialSize.Count; i++)
        {
            if (_trashMaterialSize[i] / Size > highestPrecent)
            {
                if (highestPrecent != _primaryThreshold)
                {
                    secondHighestPrecent = highestPrecent;
                    _secondaryTrashMaterial = _primaryTrashMaterial;
                }
                highestPrecent = _trashMaterialSize[i] / Size;
                _primaryTrashMaterial = _trashMaterialCounts[i];
            }
            else if (_trashMaterialSize[i] / Size > secondHighestPrecent)
            {
                secondHighestPrecent = _trashMaterialSize[i] / Size;
                _secondaryTrashMaterial = _trashMaterialCounts[i];
            }
        }

        if (highestPrecent > _dominantThreshold)
        {
            ApplyTrashMaterial(_primaryTrashMaterial, 1f);
        }
        else
        {
            ApplyTrashMaterial(_primaryTrashMaterial, 0.66f);
            ApplyTrashMaterial(_secondaryTrashMaterial, 0.33f);
        }

        _rigidBody.sharedMaterial = _physicsMaterial2D;
    }
  

    private void ApplyTrashMaterial(TrashMaterial material, float precentOf)
    {
        //Debug.Log("I am " + precentOf * 100 + "% made of "+material.name);
        _physicsMaterial2D.bounciness += material.bounciness * precentOf;
        _rigidBody.drag += material.drag * precentOf;
        _rigidBody.mass += material.mass * precentOf;
        //_spriteRenderer.color += material.color * precentOf;
        _decayMultiplier += material.decayMultiplier * precentOf;
        _damageMultiplier += material.damageMultiplier * precentOf;
        _absorbMultiplier += material.absorbMultiplier * precentOf;
    }
    

    void OnTriggerEnter2D(Collider2D other)
    {
        if (_isBeingDestroyed) return;

        if (other.gameObject.TryGetComponent(out IAbsorbable absorbableObject))
        {
            if (_activelyDecaying) return;
            float absorbingPower = (_rigidBody.velocity.magnitude - 8) * Size * _absorbMultiplier;
            absorbableObject.OnAbsorbedByTrashBall(this, absorbingPower, false);
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

            if (_rigidBody.velocity.magnitude > otherTrashBall._rigidBody.velocity.magnitude)
            {
                OnTrashBallMerge(otherTrashBall);
                return;
            }
            else if (_rigidBody.velocity.magnitude < otherTrashBall._rigidBody.velocity.magnitude) return;

            // If same size and speed, force winner to be based on the arbitrary TrashId
            if (TrashId > otherTrashBall.TrashId)
            {
                OnTrashBallMerge(otherTrashBall);
                return;
            }
        }
    }

    void OnTrashBallMerge(TrashBall otherTrashBall)
    {
        if (_isBeingDestroyed || otherTrashBall._isBeingDestroyed) return;
        if (!otherTrashBall.isActiveAndEnabled) return;
        foreach (IAbsorbable absorbable in otherTrashBall.absorbedObjects)
        {
            absorbable.OnAbsorbedByTrashBall(this, 0, true);
        }

        otherTrashBall.enabled = false;
        otherTrashBall._isBeingDestroyed = true;
        otherTrashBall.absorbedObjects.Clear();
        otherTrashBall.absorbedTrash.Clear();
        Destroy(otherTrashBall.gameObject);
    }

    private void ExplodeTrashBall()
    {
        foreach (IAbsorbable absorbable in absorbedObjects)
        {

            MonoBehaviour trashMono = absorbable as MonoBehaviour;
            trashMono.gameObject.SetActive(true);
            absorbable.OnTrashBallExplode(this);
        }
        Destroy(gameObject);
    }
    
    public void OnIgnite(HeatMechanic heat)
    {
        if (_isBeingDestroyed) return;
        _isBeingDestroyed = true;
    
        foreach (IAbsorbable absorbable in absorbedObjects)
        {
            absorbable.OnTrashBallIgnite();
        }
        absorbedObjects.Clear();
        absorbedTrash.Clear();
        Destroy(gameObject);
    }
}
