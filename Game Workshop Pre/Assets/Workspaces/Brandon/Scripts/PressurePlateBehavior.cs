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
    [Tooltip("The Speed at which the trashball rolls into the center of the man hole")]
    [SerializeField] float fallInConstant = 25f;
    [SerializeField] private Sprite _spriteOpen;
    [SerializeField] private Sprite _spriteClosed;

    private bool activated = false;
    private SpriteRenderer sr;
    private float cooldownTimer;
    private bool coolingDown = false;
    private bool isRollingToCenter = false;
    private TrashBall _currentTrashball;
    private float maxRollDistance;


    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        if (onTriggerEvent.GetPersistentEventCount() == 0)
            onTriggerEvent.AddListener(ReminderTriggerEvent);
        activated = false;
        sr.sprite = _spriteOpen;
    }

    private void FixedUpdate()
    {
        if (isRollingToCenter)
        {
            Rigidbody2D trashballRB = _currentTrashball.rigidBody;
            Vector2 trashBallPos = (Vector2)trashballRB.transform.position;
            Vector2 manholeCenterPos = (Vector2)this.transform.position;
            Vector2 heading = (manholeCenterPos - trashBallPos);
            float distance = Vector2.Distance(manholeCenterPos, trashBallPos);
            trashballRB.velocity = (((heading * fallInConstant * distance) / maxRollDistance));

            if (Mathf.Abs(distance) < 0.05f) {
                trashballRB.MovePosition(manholeCenterPos);
                trashballRB.constraints = RigidbodyConstraints2D.FreezeAll;
                isRollingToCenter = false;
                _currentTrashball.Delete();
                _currentTrashball = null;
            }
        }

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        //Plate should exclude Player and Enemy Layers for purposes of triggering.
        if (!other.TryGetComponent(out TrashBall trashBall)) return;
        
        _currentTrashball = trashBall;
        int trashSize = (int) trashBall.Size;
        if (trashSize >= minTrashSizeBound && trashSize <= maxTrashSizeBound && !activated)
        {
            Rigidbody2D trashballRB = other.attachedRigidbody;
            maxRollDistance = Vector2.Distance((Vector2)(trashballRB.transform.position), (Vector2)this.transform.position);
            isRollingToCenter = true;
            activated = true;
            sr.sprite = _spriteClosed;
            trashBall.PrepareDelete();
            StartCoroutine(RollToCenter());
        }
    }

    private void ReminderTriggerEvent()
    {
        Debug.Log($"The pressure plate at {transform.position} does not have an assigned trigger method");
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
        sr.sprite = _spriteOpen;
        yield break;    //I know this shouldn't be needed, but just in case. y'never know right.
    }

    private IEnumerator RollToCenter()
    {
        while (isRollingToCenter)
            yield return new WaitForEndOfFrame();

        onTriggerEvent?.Invoke();
        if (resettable)
            StartCoroutine(PlateCoolDown(cooldownTimer));
    }

}
