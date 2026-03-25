using Ink.Parsed;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEditor.Rendering;
using UnityEngine;

public class CollectorEnemy : EnemyBase
{
    [SerializeField] private List<CollectableTrash> _collectedTrash;
    public float collectionRadius;
    EnemySweepHandler sweepHandler;

    
    protected override void OnStart()
    {
        sweepHandler = new EnemySweepHandler();
    }

    protected override void OnUpdate()
    {
        
    }
   

    public void Collect(CollectableTrash trash)
    {
        //trash.gameObject.SetActive(false);
        //_collectedTrash.Add(trash);
        //sweepHandler.BeginSweep();
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
                detectedTrash.transform.parent = transform;
                Collect(detectedTrash);
            }
        }
        onComplete?.Invoke(true);
    }

    protected override void ForceCancelAction()
    {
        throw new NotImplementedException();
    }

    //public void OnTriggerEnter2D(Collider2D collision)
    //{
    //    if (collision.TryGetComponent(out CollectableTrash trash))
    //    {

    //        Collect(trash);

    //    }
    //}

}
