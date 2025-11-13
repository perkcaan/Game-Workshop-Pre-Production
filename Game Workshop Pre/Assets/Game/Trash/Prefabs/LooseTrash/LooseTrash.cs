using UnityEngine;
using System.Drawing;

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

    public void OnSwipe(Vector2 direction, float force)
    {
        if (!_isSwipable) return;
        _rigidBody.AddForce(direction * force, ForceMode2D.Impulse);
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion particleRotation = Quaternion.Euler(0, 0, angle + 180);
        Color32 metalColor = new Color32(255,172,28,255);


        if (trashMaterial.name == "Metal")
        {
            ParticleManager.Instance.Modify("swipe", 0, 75, 0,"Subtract");
            ParticleManager.Instance.modified = true;
            ParticleManager.Instance.Play("swipe", transform.position, particleRotation, metalColor, transform);
        }
        else
        {
            ParticleManager.Instance.modified = false;
            ParticleManager.Instance.Modify("swipe", 0, 75, 0,"Add");
            ParticleManager.Instance.Play("swipe", transform.position, particleRotation, trashMaterial.color, transform);
        }
        

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
            Quaternion randomRotation = Quaternion.Euler(0, 0, UnityEngine.Random.Range(-_randomDirectionRange, _randomDirectionRange));
            Vector3 direction = randomRotation * (transform.position - player.transform.position).normalized;

            _rigidBody.AddForce(direction * _playerEnterKnockback * (1 + playerRb.velocity.magnitude/3), ForceMode2D.Impulse);
            playerRb.AddForce(-direction * _playerEnterTripForce * (1 + playerRb.velocity.magnitude/3), ForceMode2D.Impulse);
        }
    }
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.TryGetComponent(out PlayerMovementController player))
        {
            Rigidbody2D playerRb = player.GetComponent<Rigidbody2D>();
            Quaternion randomRotation = Quaternion.Euler(0, 0, UnityEngine.Random.Range(-_randomDirectionRange, _randomDirectionRange));
            Vector3 direction = randomRotation * (transform.position - player.transform.position).normalized;

            _rigidBody.AddForce(direction * _playerExitKnockback * (1 + playerRb.velocity.magnitude/3), ForceMode2D.Impulse);
            playerRb.AddForce(-direction * _playerExitTripForce * (1 + playerRb.velocity.magnitude/3), ForceMode2D.Impulse);
        }
    }
}
