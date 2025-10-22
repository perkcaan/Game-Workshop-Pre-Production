using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class TrashPile : Trash, ISwipeable
{
    [SerializeField] int _health;
    [SerializeField] int _trashSpreadRange;
    [SerializeField] float _onDamagedShakeForce;
    [SerializeField] float _onExplodeForce;
    [SerializeField] List<Trash> _startingStoredTrash;
    private SpriteRenderer _sprite;

    void Awake()
    {
        _size = 0;
        foreach (Trash trash in _startingStoredTrash)
        {
            _size += trash.Size;
        }
        _sprite = GetComponentInChildren<SpriteRenderer>();
    }

    public void OnSwipe(Vector2 direction, float force)
    {
        TakeDamage(1, direction, force);
    }

    public override void OnAbsorbedByTrashBall(TrashBall trashBall, float absorbingPower, bool forcedAbsorb)
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
            sequence.Append(_sprite.transform.DOLocalMoveX(_onDamagedShakeForce, 0.05f));
            sequence.Append(_sprite.transform.DOLocalMoveX(-_onDamagedShakeForce, 0.1f));
            sequence.Append(_sprite.transform.DOLocalMoveX(0, 0.05f));
        }
    }

    private void ReleaseTrash(Vector2 direction, float force)
    {
        DOTween.KillAll();
        foreach (Trash trash in _startingStoredTrash)
        {
            Trash releasedTrash = Instantiate(trash);
            _parentRoom.AddCleanableToRoom(releasedTrash);
            releasedTrash.transform.position = transform.position;
            if (direction != null)
            {
                float randomAngle = Random.Range(-_trashSpreadRange, _trashSpreadRange);
                Vector2 randomDirection = Quaternion.Euler(0, 0, randomAngle) * direction;
                float randomForce = Random.Range(force * _onExplodeForce, force * _onExplodeForce * 3);
                releasedTrash.GetComponent<Rigidbody2D>().AddForce(randomDirection.normalized * randomForce, ForceMode2D.Force);
            }
        }
        _parentRoom.ObjectCleaned(this);
        Destroy(gameObject);
    }



}
