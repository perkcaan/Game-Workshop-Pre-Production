using UnityEngine;

public class LooseTrash : Trash, ISweepable, ISwipeable
{
    [SerializeField] float _sweepDurationToBecomeBall;
    [SerializeField] float _playerEnterKnockback;
    [SerializeField] float _playerEnterTripForce;
    [SerializeField] float _playerExitKnockback;
    [SerializeField] float _playerExitTripForce;
    [SerializeField] bool _isSwipable;
    private float _sweepTimer;
    public void OnSweep(Vector2 direction, float force)
    {
        if (!isActiveAndEnabled) return;
        _sweepTimer += Time.deltaTime;
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
        if (_mergableDelay >= 0f)
        {
            _mergableDelay -= Time.deltaTime;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.TryGetComponent(out PlayerMovementController player))
        {
            Rigidbody2D playerRb = player.GetComponent<Rigidbody2D>();
            Vector2 direction = (transform.position - player.transform.position).normalized;
            _rigidBody.AddForce(direction * _playerEnterKnockback * (1 + playerRb.velocity.magnitude/3));
            playerRb.AddForce(-direction * _playerEnterTripForce * (1 + playerRb.velocity.magnitude/3));
        }
    }
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.TryGetComponent(out PlayerMovementController player))
        {
            Rigidbody2D playerRb = player.GetComponent<Rigidbody2D>();
            Vector2 direction = (transform.position - player.transform.position).normalized;
            _rigidBody.AddForce(direction * _playerExitKnockback * (1 + playerRb.velocity.magnitude/3));
            playerRb.AddForce(-direction * _playerExitTripForce * (1 + playerRb.velocity.magnitude/3));
        }
    }
}
