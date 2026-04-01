using Ink.Parsed;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.EventSystems;

public class CollectorEnemy : EnemyBase
{
    [SerializeField] private List<CollectableTrash> _collectedTrash;
    public float collectionRadius;
    [SerializeField] EnemySweepHandler sweepHandler;
    
    float rotation;


    protected override void OnStart()
    {
        
    }

    protected override void OnUpdate()
    {
        sweepHandler.UpdateHitbox(FacingRotation);
    }
   

    public void Collect(CollectableTrash trash)
    {
        //trash.gameObject.SetActive(false);
        
        sweepHandler.BeginSweep(rotation,2f);
        _collectedTrash.Add(trash);
        
        _behaviour.Blackboard.Remove("targetPosition");
        _behaviour.Blackboard.Remove("target");
        

    }

    public void EmptyTrash()
    {
        foreach (CollectableTrash trash in _collectedTrash)
        {
            trash.gameObject.SetActive(true);
            trash.transform.parent = null;
            
        }
        _collectedTrash.Clear();
    }

    public IEnumerator Collection(Action<bool> onComplete)
    {
         
        yield return null;
        RaycastHit2D[] trash = Physics2D.CircleCastAll(transform.position, collectionRadius,Vector2.zero);
        foreach (RaycastHit2D obj in trash)
        {
            CollectableTrash detectedTrash = obj.collider.GetComponent<CollectableTrash>();

            if (detectedTrash != null)
            {
                Debug.Log("Collecting");
                //detectedTrash.transform.parent = transform;
               
                Collect(detectedTrash);
                
            }
        }
        onComplete?.Invoke(true);
    }

    protected override void ForceCancelAction()
    {
        sweepHandler.EndSweep();
        EmptyTrash();
    }

    //public void OnTriggerEnter2D(Collider2D collision)
    //{
    //    if (collision.TryGetComponent(out CollectableTrash trash))
    //    {

    //        Collect(trash);

    //    }
    //}

}
