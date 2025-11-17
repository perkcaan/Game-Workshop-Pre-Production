using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeavyRangedEnemy : EnemyBase
{

    [SerializeField] private float _rangedAttackDuration = 5f;
    [SerializeField] private float _rangedAttackCooldown = 6f;

    [SerializeField] private float _blastHeatApplied = 10f;
    [SerializeField] private float _constantHeatApplied = 1f;

    public void PerformAttack()
    {
        Debug.Log("PerformAttack called on: " + name);
        _moveSpeed = 0f;
        Rigidbody.velocity = Vector2.zero;
        // Get the target ITargetable
        if (_blackboard.TryGetNotNull<ITargetable>("target", out ITargetable target))
        {
            // Convert to MonoBehaviour to access transform
            MonoBehaviour targetMono = target as MonoBehaviour;
            if (targetMono == null) return;

            // Get target position
            if (_blackboard.TryGet<Vector2?>("targetPosition", out Vector2? targetPosNullable) && targetPosNullable.HasValue)
            {
                Vector2 targetPos = targetPosNullable.Value;

                HeatMechanic playerHeat = targetMono.GetComponent<HeatMechanic>();
                StartCoroutine(RangedAttackRoutine(targetMono.gameObject, targetPos, playerHeat));
            }
        }
        else
        {
            Debug.LogWarning("No target found on blackboard!");
        }
    }



    private IEnumerator RangedAttackRoutine(GameObject player, Vector2 playerPos, HeatMechanic playerHeat)
    {
        if (playerHeat == null) yield break;

        // Apply initial burst
        float _baseMoveSpeed = _moveSpeed;
        playerHeat.ModifyHeat(_blastHeatApplied, true);

        // Apply constant heat over duration
        float elapsed = 0f;
        while (elapsed < _rangedAttackDuration)
        {
            yield return new WaitForSeconds(1f);
            elapsed += 1f;
            playerHeat.ModifyHeat(_constantHeatApplied, true);
        }

        _moveSpeed = _baseMoveSpeed;
        yield return new WaitForSeconds(_rangedAttackCooldown);
        _blackboard.Set<bool>("isInAction", false);
    }



    protected override void OnStart()
    {

    }

    protected override void OnUpdate()
    {

    }
}
