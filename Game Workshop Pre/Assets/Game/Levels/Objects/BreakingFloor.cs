using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class BreakingFloor : MonoBehaviour
{
    [SerializeField] float breakDuration = 1f;
    [SerializeField] float stayBrokenDuration = 1f;
    [SerializeField] float repairDuration = 1f;
    [SerializeField] SpriteRenderer spriteRenderer;
    private bool isBroken = false;
    private bool isRepairing = false;
    private Coroutine currentSequence;
    private List<GroundedMechanic> groundedObjects = new List<GroundedMechanic>();

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out GroundedMechanic gm))
        {
            if (!isBroken)
            {
                if (!groundedObjects.Contains(gm))
                {
                    groundedObjects.Add(gm);
                    gm.IsGrounded++;
                } 
            }
        }

        if (other.TryGetComponent(out HeatMechanic heat))
        {
            if (isRepairing)
            {
                StopCoroutine(currentSequence);
                currentSequence = StartCoroutine(BreakAndRepairSequence());
                return;
            }
            
            if (!isBroken && !isRepairing)
            {
                currentSequence = StartCoroutine(BreakAndRepairSequence());
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.TryGetComponent(out GroundedMechanic gm))
        {
            if (groundedObjects.Contains(gm))
            {
                groundedObjects.Remove(gm);
                gm.IsGrounded--;
            }
        }
    }

    IEnumerator BreakAndRepairSequence()
    {
        isBroken = false;
        isRepairing = false;
        spriteRenderer.DOFade(0f, breakDuration);
        yield return new WaitForSeconds(breakDuration);

        isBroken = true;
        foreach (GroundedMechanic gm in groundedObjects)
        {
            gm.IsGrounded--;
        }
        groundedObjects.Clear();
        yield return new WaitForSeconds(stayBrokenDuration);

        isBroken = false;
        isRepairing = true;
        spriteRenderer.DOFade(1f, repairDuration);
        yield return new WaitForSeconds(repairDuration);
        
        isRepairing = false;
    }
}
