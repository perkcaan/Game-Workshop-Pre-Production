using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlugEnemy : EnemyBase
{
    [Header("Slug Properties")]

    [SerializeField] private float _eatRadius = 3f;

    private bool _stuffed;

    public bool IsStuffed() => _fullTrashSizes >= 30f;

    [SerializeField] float _fullTrashSizes;

    private GameObject _currentTarget;
    private float _currentTargetSize;

    protected override void OnStart()
    {
        
    }

    protected override void OnUpdate()
    {
        Debug.Log(_fullTrashSizes);
    }

    public IEnumerator FindEatableAction(Action<bool> onComplete)
    {
        Debug.Log("FindEatableAction called!\n" + new System.Diagnostics.StackTrace(true));
        Debug.Log("Slug: looking for food...");
        // Fail if already have target or stuffed
        if (_currentTarget != null || IsStuffed())
        {
            onComplete?.Invoke(false);
            Debug.Log("Slug: full. no more searching. tiem to kill");
            yield break;
        }

        float searchTime = 0f;
        float maxSearchTime = 4f;

        while (searchTime < maxSearchTime)
        {
            Collider2D[] trashHits = Physics2D.OverlapCircleAll(transform.position, _eatRadius, 1 << LayerMask.NameToLayer("Trash"));
            Collider2D[] ballHits = Physics2D.OverlapCircleAll(transform.position, _eatRadius, 1 << LayerMask.NameToLayer("TrashBall"));

            // combine them
            List<Collider2D> hits = new List<Collider2D>(trashHits);
            hits.AddRange(ballHits);

            Debug.Log("Slug: hits count = " + hits.Count);

            GameObject closestTarget = null;
            float closestDistance = Mathf.Infinity;
            float closestSize = 0f;

            foreach (var hit in hits)
            {
                Debug.Log("Slug: considering hit " + hit.name);
                float distance = Vector2.Distance(transform.position, hit.transform.position);

                // Check Trash
                Trash trash = hit.GetComponent<Trash>();
                if (trash != null && distance < closestDistance)
                {
                    Debug.Log("Slug: Found Trash to Eat");
                    closestTarget = trash.gameObject;
                    closestDistance = distance;
                    closestSize = trash.Size;
                    continue;
                }

                // Check TrashBall
                TrashBall ball = hit.GetComponent<TrashBall>();
                if (ball != null && distance < closestDistance)
                {
                    Debug.Log("Slug: Found Trash Ball to Eat");
                    closestTarget = ball.gameObject;
                    closestDistance = distance;
                    closestSize = ball.Size;
                }
            }

            if (closestTarget != null)
            {
                Debug.Log("Slug: Food search successful");
                _currentTarget = closestTarget;
                _currentTargetSize = closestSize;

                onComplete?.Invoke(true);
                yield break;
            }

            searchTime += Time.deltaTime;
            Debug.Log("Slug: Found food? " + _currentTarget);
            yield return null;
        }

        // Nothing found within time limit
        Debug.Log("Slug: Nothing found");
        onComplete?.Invoke(false);
    }

    public IEnumerator ConsumeEatableAction (Action<bool> onComplete)
    {
        // If stuffed is active, automatically fail
        if (IsStuffed() || _currentTarget == null)
        {
            Debug.Log(_currentTarget);
            Debug.Log("Slug: Too full. No eating");
            onComplete?.Invoke(false);
            yield break;
        }

        //*** Eating animation

        // Otherwise, wait a second, then add the target to the list
        yield return new WaitForSeconds(1f);

        _fullTrashSizes += _currentTargetSize;
        Destroy(_currentTarget);

        _currentTarget = null;
        _currentTargetSize = 0f;

        onComplete?.Invoke(true);
                Debug.Log("Slug: Ate food");

        yield break;
    }

    public IEnumerator CheckTrashInventoryAction(Action<bool> onComplete)
    {
        Debug.Log("Slug: checking stomach");
        if (_fullTrashSizes >= 30f)
        {
            _stuffed = true;
            onComplete?.Invoke(true); // belly is full
            Debug.Log("Slug: stuffed. no more food");
        }
        else if (_fullTrashSizes < 30f)
        {
            onComplete?.Invoke(false); // belly is not full
            Debug.Log("Slug: still hungry");
        }
        yield break;
    }
}
