using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PressurePlateBehavior : MonoBehaviour
{

    [SerializeField] int minTrashSizeBound;
    [SerializeField] int maxTrashSizeBound;
    [SerializeField] UnityEvent onTriggerEvent;
    [SerializeField] bool resettable;
    [SerializeField] float cooldownDuration; 

    private bool activated;
    private SpriteRenderer sr;
    private float cooldownTimer;
    private bool coolingDown = false;
    
    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        sr.color = Color.green;
        if (onTriggerEvent.GetPersistentEventCount() == 0)
            onTriggerEvent.AddListener(ReminderTriggerEvent);
        activated = false;
    }



    private void OnTriggerEnter2D(Collider2D other)
    {

        if (activated)
            return;

        //Plate should exclude Player and Enemy Layers for purposes of triggering.
        int trashSize = (int)other.GetComponent<TrashBall>().Size;
        if (trashSize > minTrashSizeBound && trashSize < maxTrashSizeBound && !activated)
        {
            other.GetComponent<Rigidbody2D>().
            activated = true;
            onTriggerEvent?.Invoke();
            if (resettable)
                StartCoroutine(PlateCoolDown(cooldownTimer));
        }
    }

    private void ReminderTriggerEvent()
    {
        Debug.Log($"The pressure plate at {transform.position} does not have an assigned trigger method");
        sr.color = Color.yellow;
    }

    public void TestTriggerEvent()
    {
        sr.color = Color.red;
    }

    private IEnumerator PlateCoolDown(float cooldown)
    {
        if (!coolingDown)
        {
            cooldownTimer = cooldownDuration;
            coolingDown = true;
        }

        while (cooldownTimer > 0f)
        {
            cooldownTimer -= Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        coolingDown = false;
        activated = false;
        sr.color = Color.green;
        yield break;    //I know this shouldn't be needed, but just in case. y'never know right.
    }

}
