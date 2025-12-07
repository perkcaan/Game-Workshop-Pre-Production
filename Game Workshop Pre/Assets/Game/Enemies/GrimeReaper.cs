using AYellowpaper.SerializedCollections;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrimeReaper : EnemyBase
{
    [SerializeField] float _attackDelay;
    [SerializeField] float _maxSpeed;
    [SerializeField] float _attackSpeed;
    [SerializeField] List<EnemyHeatHitbox> _attackList;
    PlayerMovementController _playerRef;
    float _playerSpeed;

    protected override void OnStart()
    {
        Debug.Log("Grime Reaper Started");
        _playerRef = FindObjectOfType<PlayerMovementController>();
        //_playerSpeed = _playerRef.GetComponent<PlayerMovementProps>().BaseMaxWalkSpeed;
        //_maxSpeed = _playerSpeed * 0.6f;
    }

    protected override void OnUpdate()
    {
        _moveSpeed = Mathf.Min(_moveSpeed + Time.deltaTime, _maxSpeed);
    }

    public new void OnAbsorbedByTrashBall(TrashBall trashBall, float ballVelocity, int ballSize, bool forcedAbsorb)
    {
        Debug.LogWarning("Can't absorb Grime Reaper!");
    }

    public void Attack()
    {
        StartCoroutine(AttackCoroutine());
        Debug.Log("Grime Reaper Attacking!");
    }

    public IEnumerator AttackCoroutine()
    {
        yield return new WaitForSeconds(Random.Range((_attackDelay/2),(_attackDelay * 1.5f)));
        //_attackList[Random.Range(0, _attackList.Count)].gameObject.SetActive(true);
        

    }

}
