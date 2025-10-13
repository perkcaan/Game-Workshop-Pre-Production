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
    private float _maxHealth;
    private float _health;
    private bool _activelyDecaying = false;
    public List<IAbsorbable> absorbedObjects = new List<IAbsorbable>();
    private List<Trash> absorbedTrash = new List<Trash>();
    private List<TrashMaterial> _trashMaterialCounts = new List<TrashMaterial>();
    private List<int> _trashMaterialSize = new List<int>();
    private TrashMaterial _trashMaterial;

    // Trash IDs to solve trash merge ties
    private static int _nextId = 0;
    public int TrashId { get; private set; }
    [SerializeField] private float _size = 1f;

    Rigidbody2D _rigidBody;
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
        transform.localScale = new Vector3(newSize, newSize, 1);
    }

    public void Awake()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
        _maxHealth = _baseMaxHealth;
        TrashId = _nextId++;
        _health = _maxHealth;
        _trashMaterial = _baseMaterial;
    }

    public void Update()
    {
        if (_rigidBody.velocity.magnitude < 1)
        {
            _health -= Time.deltaTime * _idleDecayMultiplier * _trashMaterial.decayMultiplier;
            if (_health < 0)
            {
                DegradeTrashBall();
                _health += _decayTrashDropRate;
            }
        }
    }

    public void OnSweep(Vector2 direction, float force)
    {
        SetDecaying(false);
        _health = _maxHealth;
        _rigidBody.AddForce(direction * force, ForceMode2D.Force);
    }
    public void OnSwipe(Vector2 direction, float force)
    {
        SetDecaying(false);
        _health = _maxHealth;
        _rigidBody.AddForce(direction * force, ForceMode2D.Impulse);
    }

    public void TakeDamage(int damage)
    {
        float damageTaken = damage * _trashMaterial.damageMultiplier;
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
        TrashMaterial mostTrashMaterial = _baseMaterial;
        float highestPrecent = 0f;
        for (int i = 0; i < _trashMaterialSize.Count; i++)
        {
            if (_trashMaterialSize[i] / Size > highestPrecent)
            {
                highestPrecent = _trashMaterialSize[i] / Size;
                mostTrashMaterial = _trashMaterialCounts[i];
            }
        }
        _trashMaterial = mostTrashMaterial;
        _rigidBody.sharedMaterial = mostTrashMaterial.material;
        _rigidBody.drag = mostTrashMaterial.drag;
        _rigidBody.mass = mostTrashMaterial.mass;
        GetComponent<SpriteRenderer>().color = mostTrashMaterial.color;
    }


    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.TryGetComponent(out IAbsorbable absorbableObject))
        {
            if (_activelyDecaying) return;
            float absorbingPower = (_rigidBody.velocity.magnitude - 2) * Size * _trashMaterial.absorbMultiplier;
            absorbableObject.OnAbsorbedByTrashBall(this, absorbingPower, false);
            _health = _maxHealth;
            return;
        }

        if (other.gameObject.TryGetComponent(out TrashBall otherTrashBall))
        {
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
        if (!otherTrashBall.isActiveAndEnabled) return;
        foreach (IAbsorbable absorbable in otherTrashBall.absorbedObjects)
        {
            absorbable.OnAbsorbedByTrashBall(this, 0, true);
        }

        otherTrashBall.enabled = false;
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
        foreach (IAbsorbable absorbable in absorbedObjects)
        {
            absorbable.OnTrashBallIgnite();
        }
        absorbedObjects.Clear();
        absorbedTrash.Clear();
        Destroy(gameObject);
    }
}
