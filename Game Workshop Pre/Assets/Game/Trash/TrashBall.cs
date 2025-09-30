using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class TrashBall : Trash
{
    [SerializeField] float _scaleMultiplier;
    [SerializeField] float _baseMaxHealth;
    [SerializeField] float _healthGainedPerSizeIncrease;
    [SerializeField] float _idleDecayMultiplier;
    private float _maxHealth;
    private float _health;
    public List<IAbsorbable> absorbedObjects = new List<IAbsorbable>();

    protected override bool MergePriority { get { return true; } }

    public void Start()
    {
        _maxHealth = _baseMaxHealth;
        _health = _maxHealth;
    }

    public void Update()
    {
        if (_rigidBody.velocity.magnitude < 2)
        {
            _health -= Time.deltaTime * _idleDecayMultiplier;
            if (_health < 0) ExplodeTrashBall();
        }
        else if (_health < _maxHealth)
        {
            _health += Time.deltaTime * _idleDecayMultiplier;
        }
    }

    public void TakeDamage(int damage)
    {
        _health -= damage;
        if (_health < 0) ExplodeTrashBall();
    }

    protected override void OnSizeChanged()
    {
        float newSize = _scaleMultiplier * Mathf.Sqrt(Size);

        _health += (_baseMaxHealth + newSize * _healthGainedPerSizeIncrease) - _maxHealth;
        _maxHealth = _baseMaxHealth + (newSize * _healthGainedPerSizeIncrease);
        transform.localScale = new Vector3(newSize, newSize, 1);
    }

    protected void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.isTrigger) return;

        if (other.gameObject.TryGetComponent(out IAbsorbable absorbableObject))
        {
            float absorbingPower = _rigidBody.velocity.magnitude * Size;
            absorbableObject.OnAbsorbedByTrashBall(this, absorbingPower, false);
            return;
        }

        if (other.gameObject.TryGetComponent(out TrashBall otherTrashBall))
        {
            if (!otherTrashBall.isActiveAndEnabled) return;
            // Priority- only one of the colliders can run OnTrashMerge
            // first check forced merge priority

            if (MergePriority && !otherTrashBall.MergePriority)
            {
                OnTrashBallMerge(otherTrashBall);
                return;
            }
            // make sure to return if this loses
            if (MergePriority != otherTrashBall.MergePriority)
                return;

            if (Size > otherTrashBall.Size)
            {
                OnTrashBallMerge(otherTrashBall);
                return;
            }

            if (Size < otherTrashBall.Size)
                return;

            // If same speed, force winner to be based on the arbitrary TrashId
            if (TrashId > otherTrashBall.TrashId)
            {
                OnTrashBallMerge(otherTrashBall);
            }
        }
    }

    protected void OnTrashBallMerge(TrashBall otherTrashBall)
    {
        //if (!otherTrashBall.isActiveAndEnabled) return;

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
            trashMono.gameObject.SetActive(true);
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
}
