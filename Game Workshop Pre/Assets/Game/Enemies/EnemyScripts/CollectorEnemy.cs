using Ink.Parsed;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class CollectorEnemy : EnemyBase
{
    [SerializeField] private List<CollectableTrash> _collectedTrash;
    protected override void OnStart()
    {
        
    }

    protected override void OnUpdate()
    {
        
    }
   

    public void Collect(CollectableTrash trash)
    {
        trash.gameObject.SetActive(false);
        _collectedTrash.Add(trash);
    }

    public IEnumerator Collection(Action<bool> onComplete)
    {
         
        yield return null;
        //OnTriggerEnter2D(GetComponent<Collider2D>());
        onComplete?.Invoke(true);
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out CollectableTrash trash))
        {
            
            Collect(trash);
            
        }
    }
}
