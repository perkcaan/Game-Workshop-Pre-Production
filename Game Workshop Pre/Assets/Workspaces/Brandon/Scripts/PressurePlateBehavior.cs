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
    [Tooltip("The Speed at which the tashball rolls into the center of the man hole")]
    [SerializeField] float fallInConstant = 25f;
    [Tooltip("The Hangtime before the Trash Ball \"falls through\" the man hole")]
    [SerializeField] float fallTime = 1.25f;

    private bool activated = false;
    private SpriteRenderer sr;
    private float cooldownTimer;
    private bool coolingDown = false;
    private bool isRollingToCenter = false;
    private Rigidbody2D trashballRB;
    private float maxRollDistance;


    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        sr.color = Color.green;
        if (onTriggerEvent.GetPersistentEventCount() == 0)
            onTriggerEvent.AddListener(ReminderTriggerEvent);
        activated = false;
    }

    private void FixedUpdate()
    {
        if (isRollingToCenter)
        {
            Vector2 trashBallPos = (Vector2)trashballRB.transform.position;
            Vector2 manholeCenterPos = (Vector2)this.transform.position;
            Vector2 heading = (manholeCenterPos - trashBallPos);
            float distance = Vector2.Distance(manholeCenterPos, trashBallPos);
            trashballRB.velocity = (((heading * fallInConstant * distance) / maxRollDistance));

            if (Mathf.Abs(distance) < 0.05f) {
                trashballRB.transform.position = this.transform.position;
                trashballRB.constraints = RigidbodyConstraints2D.FreezeAll;
                isRollingToCenter = false;
            }
        }

    }

    private void OnTriggerEnter2D(Collider2D other)
    {

        if (!other.name.Contains("TrashBall"))
            return;

        //Plate should exclude Player and Enemy Layers for purposes of triggering.
        int trashSize = (int)other.GetComponent<TrashBall>().Size;
        if (trashSize > minTrashSizeBound && trashSize < maxTrashSizeBound && !activated)
        {
            trashballRB = other.GetComponent<Rigidbody2D>();
            maxRollDistance = Vector2.Distance((Vector2)(trashballRB.transform.position), (Vector2)this.transform.position);
            isRollingToCenter = true;
            activated = true;
            StartCoroutine(RollToCenter());
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

    private IEnumerator RollToCenter()
    {
        while (isRollingToCenter)
            yield return new WaitForEndOfFrame();

            TrashBall tb = trashballRB.gameObject.GetComponent<TrashBall>();
            if (tb != null)
                yield return StartCoroutine(ManHoleFallThrough(tb));

        onTriggerEvent?.Invoke();
        if (resettable)
            StartCoroutine(PlateCoolDown(cooldownTimer));
    }

    private IEnumerator ManHoleFallThrough(TrashBall tb)
    {
        while (fallTime >= 0f)
        {
            fallTime -= Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        //Scale Shrinkage to simulate falling
        fallTime = 1f;
        while (fallTime > 0f)
        {
            tb.gameObject.transform.localScale *= (fallTime);
            fallTime -= Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        
        tb.ClearContents();
        Destroy(tb.gameObject);
    }
}
