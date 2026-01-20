// using System.Collections;
// using System.Collections.Generic;
// using System.Runtime.CompilerServices;
// using UnityEngine;

//Note: Need to be reimplemented with the new enemy system

// public class SwarmEnemy : EnemyBase
// {
//     [SerializeField] private float _attackDuration = 1f;
//     [SerializeField] private float _attackCooldown = 4f;
//     [SerializeField] private EnemyHeatHitbox _attackHitbox;

//     public void PerformAttack()
//     {
//         _blackboard.TryGet<float>("rotation", out float rotation);
//         _attackHitbox.UpdateRotation(transform, rotation);
//         _attackHitbox.Enable();
//         _animator.SetBool("Attacking", true);


//         StartCoroutine(AttackDuration());
//     }

//     private IEnumerator AttackDuration()
//     {
//         yield return new WaitForSeconds(_attackDuration);
//         _attackHitbox.Disable();
//         _animator.SetBool("Attacking", false);
//         yield return new WaitForSeconds(_attackCooldown);
//         _blackboard.Set<bool>("isInAction", false);

//     }


//     protected override void OnStart()
//     {

//     }

//     protected override void OnUpdate()
//     {

//     }
// }
