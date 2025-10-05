using System;
using System.Collections.Generic;
using UnityEngine;

public class TrashBall : MonoBehaviour, ISweepable, ISwipeable, IHeatable
{
    [SerializeField] float _scaleMultiplier;
    [SerializeField] float _baseMaxHealth;
    [SerializeField] float _healthGainedPerSizeIncrease;
    [SerializeField] float _idleDecayMultiplier;
    [SerializeField] float _onDamagedShakeForce;
    private float _maxHealth;
    private float _health;
    private SpriteRenderer _sprite;
    public List<IAbsorbable> absorbedObjects = new List<IAbsorbable>();

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
            OnSizeChanged();
        }
    }
    [SerializeField] List<TrashBallSize> trashBallSizes;
    public void Awake()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
        _sprite = GetComponent<SpriteRenderer>();
        _maxHealth = _baseMaxHealth;
        TrashId = _nextId++;
        _health = _maxHealth;
    }

    public void Update()
    {
        transform.localScale = Vector2.Lerp(transform.localScale, Vector2.one, 5f);
        if (_rigidBody.velocity.magnitude < 1)
        {
            if (_health < 0) ExplodeTrashBall();
        }
        else
        {
            _health = _maxHealth;
        }
    }

    public void OnSweep(Vector2 direction, float force)
    {
        _rigidBody.AddForce(direction * force, ForceMode2D.Force);
    }
    public void OnSwipe(Vector2 direction, float force)
    {
        _rigidBody.AddForce(direction * force, ForceMode2D.Impulse);
    }

    public void TakeDamage(int damage)
    {
        _health -= damage;
        transform.localScale *= 1.1f;
        if (_health < 0)
        {
            ExplodeTrashBall();
        }
    }

    protected void OnSizeChanged()
    {
        _maxHealth = Size;
        for (int i = 0; i < trashBallSizes.Count; i++)
        {
            if (Size <= trashBallSizes[i].trashSize )
            {
                if (_sprite.sprite == trashBallSizes[i].ballSprite) return;
                _sprite.sprite = trashBallSizes[i].ballSprite;
                foreach (CircleCollider2D cc in GetComponents<CircleCollider2D>()) cc.radius = 0.125f * (i + 2);
                return;
            }
        }
        
    }

    protected void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.TryGetComponent(out IAbsorbable absorbableObject))
        {
            float absorbingPower = (2 - _rigidBody.velocity.magnitude / 2) * Size;
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

    protected void OnTrashBallMerge(TrashBall otherTrashBall)
    {
        if (!otherTrashBall.isActiveAndEnabled) return;
        foreach (IAbsorbable absorbable in otherTrashBall.absorbedObjects)
        {
            absorbable.OnAbsorbedByTrashBall(this, 0, true);
        }

        otherTrashBall.enabled = false;
        Destroy(otherTrashBall.gameObject);
    }

    private void BurnTrashBall()
    {
        foreach (IAbsorbable absorbable in absorbedObjects)
        {
            MonoBehaviour trashMono = absorbable as MonoBehaviour;
            if (trashMono != null) trashMono.gameObject.SetActive(true);
            absorbable.OnTrashBallExplode(this);
        }
        Destroy(gameObject);
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

    public void TempMelt() //TODO: temporary rework melting / burning to destroy objects inside
    {
        foreach (IAbsorbable absorbable in absorbedObjects)
        {
            MonoBehaviour trashMono = absorbable as MonoBehaviour;
            if (trashMono == null) continue;

            if (absorbable is PlayerMovementController)
            {
                trashMono.transform.position = new Vector2(-6.5f, 2);
            }
            else
            {
                Destroy(trashMono.gameObject);
            }
        }
        absorbedObjects.Clear();
        Destroy(gameObject);
    }
    
    public void OnIgnite(HeatMechanic heat)
    {
        foreach (IAbsorbable absorbable in absorbedObjects)
        {
            MonoBehaviour trashMono = absorbable as MonoBehaviour;
            Destroy(trashMono.gameObject);
        }
        absorbedObjects.Clear();
        Destroy(gameObject);
    }
}
