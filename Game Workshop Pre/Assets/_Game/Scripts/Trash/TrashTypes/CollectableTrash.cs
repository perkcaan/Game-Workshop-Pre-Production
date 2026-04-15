using UnityEngine;
using System;

public class CollectableTrash : Trash, ISweepable, ISwipeable, IPokeable, ITargetable
{
    [SerializeField] float _sweepDurationToBecomeBall;
    [SerializeField] bool _swipesIntoTrashBall;
    [SerializeField] float _pokeForceMultiplier = 1f;
    [SerializeField] float _swipeForceMultiplier = 1f;
    public GameObject HitParent { get { return gameObject; } }

    public TargetType _targetType = TargetType.Trash;


    public TargetType TargetType
    {
        get => _targetType;
        set => _targetType = value;
    }

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
    public void OnSwipe(Vector2 direction, float force, Collider2D collision, ref float knockbackMultiplier)
    {
        _rigidBody.AddForce(direction * force * _swipeForceMultiplier, ForceMode2D.Impulse);
        if (_swipesIntoTrashBall) CreateTrashBall();
    }

    public void OnPoke(Vector2 direction, float force, Collider2D collider, ref float knockbackMultiplier)
    {
         _rigidBody.AddForce(direction * force * _pokeForceMultiplier * _rigidBody.mass, ForceMode2D.Impulse);
    }

    void Update()
    {
        if (_sweepTimer >= 0) _sweepTimer -= Time.deltaTime / 2;
    }

    public TargetType GetTargetType()
    {
        
        return TargetType.Trash;
    }

    public void NullType()
    {
        _targetType = TargetType.Null;
        
    }

    
}
