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
    [SerializeField] List<GameObject> _startingStoredTrash;
    [SerializeField] Color color;
    private float _sweepTimer;
    private float _shakeSpeed = 0.125f;
    private Tween _shakeTween;

    protected override void Awake()
    {
        base.Awake();
        _size = 0;
        RecalculateSize(true);
    }

    private void OnValidate()
    {
        RecalculateSize(false);
    }

    private void RecalculateSize(bool printError)
    {
        _size = 0;
        foreach (GameObject trash in _startingStoredTrash)
        {
            
            if(trash == null)
            {
                if (printError) Debug.LogError(gameObject.name + " is missing trash");
                break;
            }
            if (trash.TryGetComponent(out ICleanable cleanable))
            {
                _size += cleanable.Size;
            }
        }
    }

    public void OnSweep(Vector2 position, Vector2 direction, float force)
    {
        if (_isDestroyed) return;
        if (!isActiveAndEnabled) return;
        _sweepTimer += Time.deltaTime * 2;
        if (_sweepTimer > _sweepDurationToTakeDamage)
        {
            TakeDamage(1, direction, force);
            _sweepTimer = 0;
        }
    }


    public void OnSwipe(Vector2 direction, float force, Collider2D collider)
    {
        if (_isDestroyed) return;
        TakeDamage(3, direction, force);
        if(this != null && trashMaterial != null)
            ParticleManager.Instance.Play("swipe", transform.position, Quaternion.Euler(0, 0, transform.rotation.eulerAngles.z + 90f), this.trashMaterial.color, transform);
    }

    public override bool OnAbsorbedByTrashBall(TrashBall trashBall, Vector2 ballVelocity, int ballSize, bool forcedAbsorb)
    {
        if (_isDestroyed) return false;
        if (Size <= trashBall.Size)
        {
            ReleaseTrash(ballVelocity.normalized, ballVelocity.magnitude);
        }
        else
        {
            TakeDamage(3, ballVelocity.normalized, ballVelocity.magnitude);
        }
        return false;
    }

    public void TakeDamage(int damage, Vector2 direction, float force)
    {
        if (_isDestroyed) return;
        _health -= damage;
        if (_health <= 0)
        {
            ReleaseTrash(direction, force);
        }
        else
        {
            if (_shakeTween != null && _shakeTween.IsActive()) _shakeTween.Complete();
            Sequence sequence = DOTween.Sequence();
            sequence.Append(_spriteRenderer.transform.DOLocalMove(direction.normalized * _onDamagedShakeForce * damage, _shakeSpeed));
            sequence.Append(_spriteRenderer.transform.DOLocalMove(-direction.normalized * _onDamagedShakeForce * damage / 4, _shakeSpeed));
            sequence.Append(_spriteRenderer.transform.DOLocalMove(Vector3.zero, _shakeSpeed));
            sequence.SetLink(_spriteRenderer.gameObject); 
            _shakeTween = sequence;
        }
    }

    private void ReleaseTrash(Vector2 direction, float force)
    {
        if (_isDestroyed) return;
        _isDestroyed = true;
        _rigidBody.simulated = false;
        _parentRoom.ObjectCleaned(this);

        if (_shakeTween != null && _shakeTween.IsActive()) _shakeTween.Kill();
        transform.DOScale(Vector3.zero, 0.2f)
                 .SetEase(Ease.OutQuad)
                 .SetLink(gameObject) 
                 .OnComplete(() => Destroy(gameObject));

        ScoreBehavior.SendScore?.Invoke(_pointValue);

        float angleRadians = Mathf.Atan2(direction.y, direction.x);
        Quaternion rotation = Quaternion.Euler(0f, 0f, (angleRadians * Mathf.Rad2Deg)-45f);
        ParticleManager.Instance.Play("DustBurst", transform.position, rotation, color);

        foreach (GameObject trash in _startingStoredTrash)
        {
            if(trash == null)
            {
                Debug.Log(gameObject.name + " is missing trash");
                break;
            }

            GameObject releasedTrash = Instantiate(trash);
            
            if (releasedTrash.TryGetComponent(out ICleanable cleanable))
            {
                _parentRoom.AddCleanableToRoom(cleanable);
            }
            
            releasedTrash.transform.position = transform.position;
            if (direction != null)
            {
                float randomAngle = UnityEngine.Random.Range(-_trashSpreadRange, _trashSpreadRange);
                Vector2 randomDirection = Quaternion.Euler(0, 0, randomAngle) * direction;

                float randomForce = UnityEngine.Random.Range(force * _onExplodeForce, force * _onExplodeForce * 2);
                releasedTrash.GetComponent<Rigidbody2D>().AddForce(randomDirection.normalized * randomForce, ForceMode2D.Impulse);
                releasedTrash.transform.localScale = Vector3.zero;
                
                releasedTrash.transform.DOScale(Vector3.one, 0.3f)
                             .SetEase(Ease.OutQuad)
                             .SetLink(releasedTrash.gameObject);
            }
        }
    }
}
