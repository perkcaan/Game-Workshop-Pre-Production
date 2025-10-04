using UnityEngine;

public class LooseTrash : Trash, ISweepable, ISwipeable
{
    [SerializeField] float _sweepDurationToBecomeBall;
    [SerializeField] float _playerShoveForce;
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
            Vector2 direction = (transform.position - player.transform.position).normalized;
            _rigidBody.AddForce(direction * _playerShoveForce * player.GetComponent<Rigidbody2D>().velocity.magnitude);
        }
    }
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.TryGetComponent(out PlayerMovementController player))
        {
            Vector2 direction = (transform.position - player.transform.position).normalized;
            _rigidBody.AddForce(direction * (_playerShoveForce / 2) * player.GetComponent<Rigidbody2D>().velocity.magnitude);
        }
    }
}
