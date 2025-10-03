using UnityEngine;

public class FlatTrash : Trash, ISweepable
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

    void Update()
    {
        if (_mergableDelay >= 0f)
        {
            _mergableDelay -= Time.deltaTime;
        }
    }
}
