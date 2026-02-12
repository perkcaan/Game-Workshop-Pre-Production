using UnityEngine;
using System.Drawing;
using System;

public class LooseTrash : Trash, ISweepable, ISwipeable
{
    [SerializeField] bool _hasImpactParticle = false;
    [SerializeField] float _sweepDurationToBecomeBall;
    [SerializeField] float _playerEnterKnockback;
    [SerializeField] float _playerEnterTripForce;
    [SerializeField] float _playerExitKnockback;
    [SerializeField] float _playerExitTripForce;
    [SerializeField] float _randomDirectionRange;
    [SerializeField] float maximumVelocity = 2f;
    [SerializeField] bool _isSwipable;
    private float _sweepTimer;

    public void OnSweep(Vector2 position, Vector2 direction, float force)
    {
        if (!isActiveAndEnabled) return;
        _sweepTimer += Time.deltaTime * 2;
        _rigidBody.AddForce(direction * force, ForceMode2D.Force);
        if (_sweepTimer > _sweepDurationToBecomeBall)
        {
            CreateTrashBall();
        }
    }

    public void OnSwipe(Vector2 direction, float force, Collider2D collider)
    {
        if (!_isSwipable) return;
        _rigidBody.AddForce(direction * force, ForceMode2D.Impulse);
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        
        Quaternion particleRotation = Quaternion.Euler(0, 0, angle + 180);
        ParticleManager.Instance.Play("swipe", transform.position, particleRotation, trashMaterial.color, transform);
    }

    void Update()
    {
        if (_sweepTimer >= 0) _sweepTimer -= Time.deltaTime / 2;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.TryGetComponent(out PlayerMovementController player))
        {
            if (_hasImpactParticle) {
                Vector3 contactPoint = transform.position; // other.ClosestPoint(transform.position);
                ParticleManager.Instance.Play("ImpactCircleS", contactPoint);
            }
            Rigidbody2D playerRb = player.GetComponent<Rigidbody2D>();
            Quaternion randomRotation = Quaternion.Euler(0, 0, UnityEngine.Random.Range(-_randomDirectionRange, _randomDirectionRange));
            Vector3 direction = randomRotation * (transform.position - player.transform.position).normalized;
            float velocityMultiplier = Math.Min(1 + playerRb.velocity.magnitude/3, maximumVelocity);

            _rigidBody.AddForce(direction * _playerEnterKnockback * velocityMultiplier, ForceMode2D.Impulse);
            playerRb.AddForce(-direction * _playerEnterTripForce * velocityMultiplier, ForceMode2D.Impulse);
        }
    }
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.TryGetComponent(out PlayerMovementController player))
        {
            Rigidbody2D playerRb = player.GetComponent<Rigidbody2D>();
            Quaternion randomRotation = Quaternion.Euler(0, 0, UnityEngine.Random.Range(-_randomDirectionRange, _randomDirectionRange));
            Vector3 direction = randomRotation * (transform.position - player.transform.position).normalized;
            float velocityMultiplier = Math.Min(1 + playerRb.velocity.magnitude/3, maximumVelocity);

            _rigidBody.AddForce(direction * _playerExitKnockback * velocityMultiplier, ForceMode2D.Impulse);
            playerRb.AddForce(-direction * _playerExitTripForce * velocityMultiplier, ForceMode2D.Impulse);
        }
    }
}
