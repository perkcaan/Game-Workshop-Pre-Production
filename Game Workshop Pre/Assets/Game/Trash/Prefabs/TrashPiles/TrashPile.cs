using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using System;

public class TrashPile : Trash, ISweepable, ISwipeable
{
    [SerializeField] int _health;
    [SerializeField] int _trashSpreadRange;
    [SerializeField] float _onDamagedShakeForce;
    [SerializeField] float _onExplodeForce;
    [SerializeField] float _sweepDurationToTakeDamage;
    [SerializeField] List<Trash> _startingStoredTrash;
    private SpriteRenderer _sprite;
    private float _sweepTimer;
    private float _shakeSpeed = 0.125f;

    void Awake()
    {
        _size = 0;
        foreach (Trash trash in _startingStoredTrash)
        {
            _size += trash.Size;
        }
        _sprite = GetComponentInChildren<SpriteRenderer>();
    }

    public void OnSweep(Vector2 position, Vector2 direction, float force)
    {
        if (!isActiveAndEnabled) return;
        _sweepTimer += Time.deltaTime * 2;
        if (_sweepTimer > _sweepDurationToTakeDamage)
        {
            TakeDamage(1, direction, force);
            _sweepTimer = 0;
        }
    }


    public void OnSwipe(Vector2 direction, float force)
    {
        TakeDamage(3, direction, force);
    }

    public override void OnAbsorbedByTrashBall(TrashBall trashBall, float ballVelocity, int ballSize, bool forcedAbsorb)
    {
        if (Size <= trashBall.Size)
        {
            Rigidbody2D trashBallRB = trashBall.GetComponent<Rigidbody2D>();
            ReleaseTrash(trashBallRB.velocity.normalized, trashBallRB.velocity.magnitude);
        }
        else
        {
            Rigidbody2D trashBallRB = trashBall.GetComponent<Rigidbody2D>();
            TakeDamage(1, trashBallRB.velocity.normalized, trashBallRB.velocity.magnitude);
        }
    }

    public void TakeDamage(int damage, Vector2 direction, float force)
    {
        _health -= damage;
        if (_health <= 0)
        {
            ReleaseTrash(direction, force);
        }
        else
        {
            Sequence sequence = DOTween.Sequence();
            sequence.Append(_sprite.transform.DOLocalMove(direction.normalized * _onDamagedShakeForce * damage, _shakeSpeed));
            sequence.Append(_sprite.transform.DOLocalMove(-direction.normalized * _onDamagedShakeForce * damage / 4, _shakeSpeed));
            sequence.Append(_sprite.transform.DOLocalMove(Vector3.zero, _shakeSpeed));
        }
    }

    private void ReleaseTrash(Vector2 direction, float force)
    {
        DOTween.KillAll();
        SendScore?.Invoke(_pointValue);

        foreach (Trash trash in _startingStoredTrash)
        {
            Trash releasedTrash = Instantiate(trash);
            _parentRoom.AddCleanableToRoom(releasedTrash);
            releasedTrash.transform.position = transform.position;
            if (direction != null)
            {
                float randomAngle = UnityEngine.Random.Range(-_trashSpreadRange, _trashSpreadRange);
                Vector2 randomDirection = Quaternion.Euler(0, 0, randomAngle) * direction;

                float randomForce = UnityEngine.Random.Range(force * _onExplodeForce, force * _onExplodeForce * 3);
                releasedTrash.GetComponent<Rigidbody2D>().AddForce(randomDirection.normalized * randomForce, ForceMode2D.Impulse);
            }
        }
        _parentRoom.ObjectCleaned(this);
        Destroy(gameObject);
    }
}
