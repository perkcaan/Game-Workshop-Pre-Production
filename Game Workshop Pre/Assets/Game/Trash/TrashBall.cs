using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class TrashBall : Trash
{
    [SerializeField] float _scaleMultiplier;
    [SerializeField] float _baseMaxHealth;
    [SerializeField] float _idleDecayMultiplier;
    [SerializeField] float _minimumSizeToExplode;
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
        if (Size >= _minimumSizeToExplode)
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
    }

    public void TakeDamage(int damage)
    {
        _health -= damage;
        if (_health < 0) ExplodeTrashBall();
    }

    protected override void OnSizeChanged()
    {
        float newSize = _scaleMultiplier * Mathf.Sqrt(Size);
        _maxHealth = _baseMaxHealth + newSize;
        transform.localScale = Vector3.one * newSize;
    }

    protected void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.TryGetComponent(out IAbsorbable absorbableObject))
        {
            float absorbingPower = _rigidBody.velocity.magnitude * Size;
            absorbableObject.OnAbsorbedByTrashBall(this, absorbingPower, false);
        }

        if (other.gameObject.TryGetComponent(out TrashBall otherTrashBall))
        {
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

            // since they have equal priority- check speed
            float speed = _rigidBody.velocity.sqrMagnitude;
            float otherSpeed = otherTrashBall.GetComponent<Rigidbody2D>().velocity.sqrMagnitude;

            if (speed > otherSpeed)
            {
                OnTrashBallMerge(otherTrashBall);
                return;
            }

            if (speed < otherSpeed)
                return;

            // If same speed, force winner to be based on the arbitrary TrashId
            if (TrashId > otherTrashBall.TrashId)
            {
                OnTrashBallMerge(otherTrashBall);
            }
        }
    }

    protected void OnTrashBallMerge(TrashBall otherTrash)
    {
        Size += otherTrash.Size;

        foreach (IAbsorbable absorbable in otherTrash.absorbedObjects)
        {
            absorbable.OnAbsorbedByTrashBall(this, 0, true);
        }

        Destroy(otherTrash.gameObject);
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
