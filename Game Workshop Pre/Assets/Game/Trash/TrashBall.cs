using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TrashBall : PushableObject
{
    [SerializeField] float scaleMultiplier;
    public List<CollectableTrash> consumedTrash = new List<CollectableTrash>();
    Animator animator;
    Vector2 lastMove;   //to remember direction

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        Vector2 dir = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        if (dir.sqrMagnitude > 0.01f) //moving
        {
            //send direction to animator
            dir.Normalize();
            
            animator.SetFloat("moveX", dir.x);
            animator.SetFloat("moveY", dir.x);
            animator.SetBool("isMoving", true);

            //remember facing direction
            lastMove = dir;
        }
        else
        {
            //some idle animation in last direction
            animator.SetFloat("moveX", lastMove.x);
            animator.SetFloat("moveY", lastMove.y);
            animator.SetBool("isMoving", false);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.TryGetComponent(out CollectableTrash trash))
        {
            consumedTrash.Add(trash);
            weight += trash.weight;
            SetSize();
            Destroy(trash.gameObject);
            return;
        }
        if (other.gameObject.TryGetComponent(out TrashBall trashBall))
        {
            if (rb.velocity.magnitude > trashBall.rb.velocity.magnitude)
            {
                consumedTrash.AddRange(trashBall.consumedTrash);
                weight += trashBall.weight;
                SetSize();
                Destroy(trashBall.gameObject);
            }
            return;
        }
    }

    public void SetSize()
    {
        float newSize = scaleMultiplier * Mathf.Sqrt(weight);
        transform.localScale = Vector3.one * newSize;
    }
}