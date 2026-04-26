using UnityEngine;
using System.Collections;

public class Geyser : MonoBehaviour
{
    public enum GeyserState { Idle, Warning, Erupting }
    
    public GeyserState currentState = GeyserState.Idle;

    [SerializeField] private SpriteRenderer flashSprite;
    [SerializeField] private CircleCollider2D damageCollider;

    [Header("Timing Settings")]
    [SerializeField] private float timeUntilWarn = 5f;
    [SerializeField] private float warningDuration = 3f;
    [SerializeField] private float explosionDuration = 4f;
    [SerializeField] private float flashInterval = 0.1f;
    [SerializeField] private float maxRandomTimeOffset = 1f;

    private float cooldownTimer;
    private Coroutine flashCoroutine;

    private void Start()
    {
        flashSprite.enabled = false;
        damageCollider.enabled = false;
        
        cooldownTimer = timeUntilWarn + Random.Range(0f, maxRandomTimeOffset);
        
    }

    private void Update()
    {
        if (currentState == GeyserState.Idle)
        {
            cooldownTimer -= Time.deltaTime;
            
            if (cooldownTimer <= 0f)
            {
                StartCoroutine(EruptionSequence());
            }
        }
    }

    private IEnumerator EruptionSequence()
    {
        currentState = GeyserState.Warning;
        flashCoroutine = StartCoroutine(FlashEffect());
        ParticleManager.Instance.Play("LavaAmbient", transform.position);

        yield return new WaitForSeconds(warningDuration);

        currentState = GeyserState.Erupting;
        ParticleManager.Instance.Play("Geyser", transform.position);
        
        if (flashCoroutine != null)  StopCoroutine(flashCoroutine);
        flashSprite.enabled = false; 
        
        damageCollider.enabled = true; 

        yield return new WaitForSeconds(explosionDuration);
        ParticleManager.Instance.Play("LavaAmbient", transform.position);

        damageCollider.enabled = false;
        currentState = GeyserState.Idle;
        cooldownTimer = timeUntilWarn;
    }

    private IEnumerator FlashEffect()
    {
        while (true)
        {
            flashSprite.enabled = !flashSprite.enabled;
            yield return new WaitForSeconds(flashInterval);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out HeatMechanic heat))
        {
            if (currentState == GeyserState.Idle && cooldownTimer > 0.5f)
            {
                cooldownTimer = 0f;
            }
        }
    }
}