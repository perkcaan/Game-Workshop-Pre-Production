using UnityEngine;
using System;

public class CollectableTrash : Trash, ISweepable, ISwipeable, IPokeable
{
    [SerializeField] float _sweepDurationToBecomeBall;
    [SerializeField] bool _swipesIntoTrashBall;
    [SerializeField] float _pokeForceMultiplier = 1f;
    [SerializeField] float _swipeForceMultiplier = 1f;
    
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
    public void OnSwipe(Vector2 direction, float force, Collider2D collision)
    {
        _rigidBody.AddForce(direction * force * _swipeForceMultiplier, ForceMode2D.Impulse);
        if (_swipesIntoTrashBall) CreateTrashBall();
    }

    public void OnPoke(Vector2 direction, float force, Collider2D collider)
    {
         _rigidBody.AddForce(direction * force * _pokeForceMultiplier * _rigidBody.mass, ForceMode2D.Impulse);
    }

    void Update()
    {
        if (_sweepTimer >= 0) _sweepTimer -= Time.deltaTime / 2;
    }
    
}
