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
    [SerializeField] EnemySweepHandler sweepHandler;
    Animator Animator;
    float rotation;


    protected override void OnStart()
    {
        Animator = GetComponentInChildren<Animator>();
    }

    protected override void OnUpdate()
    {
        sweepHandler.UpdateHitbox(FacingRotation);
    }
   

    public void Collect(CollectableTrash trash)
    {
        //trash.gameObject.SetActive(false);
        
        sweepHandler.BeginSweep(rotation,2f);
        trash.NullType();

    }

    public void EmptyTrash()
    {
        foreach (CollectableTrash trash in _collectedTrash)
        {
            trash.gameObject.SetActive(true);
            trash.transform.parent = null;
            trash.OnTrashBallRelease(null, Vector2.zero);
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
