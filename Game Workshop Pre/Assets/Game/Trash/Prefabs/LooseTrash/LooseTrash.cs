using UnityEngine;

public class LooseTrash : Trash, ISweepable, ISwipeable
{
    [SerializeField] float _sweepDurationToBecomeBall;
    [SerializeField] float _playerEnterKnockback;
    [SerializeField] float _playerEnterTripForce;
    [SerializeField] float _playerExitKnockback;
    [SerializeField] float _playerExitTripForce;
    [SerializeField] float _randomDirectionRange;
    [SerializeField] bool _isSwipable;
    private float _sweepTimer;
    public void OnSweep(Vector2 direction, float force)
    {
        if (!isActiveAndEnabled) return;
        _sweepTimer += Time.deltaTime * 2;
        _rigidBody.AddForce(direction * force, ForceMode2D.Force);
        if (_sweepTimer > _sweepDurationToBecomeBall)
        {
            CreateTrashBall();
        }
    }

    public void OnSwipe(Vector2 direction, float force)
    {
        if (!_isSwipable) return;
        _rigidBody.AddForce(direction * force, ForceMode2D.Impulse);
    }

    void Update()
    {
        if (_sweepTimer >= 0) _sweepTimer -= Time.deltaTime / 2;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.TryGetComponent(out PlayerMovementController player))
        {
            Rigidbody2D playerRb = player.GetComponent<Rigidbody2D>();
            Quaternion randomRotation = Quaternion.Euler(0, 0, Random.Range(-_randomDirectionRange, _randomDirectionRange));
            Vector3 direction = randomRotation * (transform.position - player.transform.position).normalized;

            _rigidBody.AddForce(direction * _playerEnterKnockback * (1 + playerRb.velocity.magnitude/3));
            playerRb.AddForce(-direction * _playerEnterTripForce * (1 + playerRb.velocity.magnitude/3));
        }
    }
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.TryGetComponent(out PlayerMovementController player))
        {
            Rigidbody2D playerRb = player.GetComponent<Rigidbody2D>();
            Quaternion randomRotation = Quaternion.Euler(0, 0, Random.Range(-_randomDirectionRange, _randomDirectionRange));
            Vector3 direction = randomRotation * (transform.position - player.transform.position).normalized;

            _rigidBody.AddForce(direction * (1 + playerRb.velocity.magnitude/3));
            playerRb.AddForce(-direction * (1 + playerRb.velocity.magnitude/3));
        }
    }
}
