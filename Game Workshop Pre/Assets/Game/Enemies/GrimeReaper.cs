using AYellowpaper.SerializedCollections;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrimeReaper : EnemyBase
{
    [SerializeField] float _attackDelay;
    [SerializeField] float _attackSpeed;
    //[SerializeField] List<EnemyHeatHitbox> _attackList;
    [SerializeField] FishLaunch FishLaunch;
    private Vector2 _currentTargetPosition;
    [SerializeField] Room _parentRoom;
    


    protected override void OnStart()
    {
        
      
    }

    protected override void OnUpdate()
    {
        
    }

    public new void OnAbsorbedByTrashBall(TrashBall trashBall, float ballVelocity, int ballSize, bool forcedAbsorb)
    {
        Debug.LogWarning("Can't absorb Grime Reaper!");
    }

    public void Attack()
    {
        //StartCoroutine(AttackCoroutine());
        Debug.Log("Grime Reaper Attacking!");
    }

    //public IEnumerator AttackCoroutine()
    //{
    //    yield return new WaitForSeconds(Random.Range((_attackDelay/2),(_attackDelay * 1.5f)));
    //    int index = Random.Range(0, _attackList.Count);
       
            
    //        //_attackList[index].UpdateRotation(transform, rotation);
    //        _attackList[index].gameObject.SetActive(true);
    //        yield return new WaitForSeconds(.25f);
    //        _attackList[index].gameObject.SetActive(false);
            
       

    //    if (_attackList[index].GetComponent<FishLaunch>())
    //    {
            
    //    }

            

    //}
   

    public void SetTargetPosition(Vector2 position)
    {
        _currentTargetPosition = position;
    }

}
