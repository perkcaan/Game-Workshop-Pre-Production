using UnityEngine;

public class LooseTrash : Trash, ISweepable, ISwipeable
{
    [SerializeField] float _sweepDurationToBecomeBall;
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
        _rigidBody.AddForce(direction * force, ForceMode2D.Impulse);
    }

    void Update()
    {
        if (_mergableDelay >= 0f)
        {
            _mergableDelay -= Time.deltaTime;
        }
    }
    
}
