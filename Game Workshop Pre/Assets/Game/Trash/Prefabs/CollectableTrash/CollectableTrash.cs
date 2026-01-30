using UnityEngine;
using System;

public class CollectableTrash : Trash, ISweepable, ISwipeable
{
    [SerializeField] float _sweepDurationToBecomeBall;
    [SerializeField] bool _swipesIntoTrashBall;
    private float _sweepTimer;
    private ShaderManager _shaderManager;

    
    public void Start()
    {
        
        _shaderManager = GetComponentInChildren<ShaderManager>();
    }
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
        _rigidBody.AddForce(direction * force, ForceMode2D.Impulse);
        if (_swipesIntoTrashBall) CreateTrashBall();
        _shaderManager.SquashnStrech(.5f,.1f);

    }

    void Update()
    {
        if (_sweepTimer >= 0) _sweepTimer -= Time.deltaTime / 2;
    }
    
}
