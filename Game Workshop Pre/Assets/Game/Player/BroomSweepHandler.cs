using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using UnityEngine;
using System;
using AYellowpaper.SerializedCollections;


// Handles the player's sweep ability
[RequireComponent(typeof(Collider2D))]
public class BroomSweepHandler : MonoBehaviour
{

    // Components
    private PlayerMovementController _parent;
    private Collider2D _hitbox;
    private PlayerContext _ctx;
    private FMOD.Studio.EventInstance _sweepSoundInstance;
    private BroomPokebox _pokebox;
    private Broomhead _broomhead;
    private Transform _sweepSingularity;
    public GameObject marker;

    // Properties
    [SerializeField] float _curveSteepness = 1.1f;
    [SerializeField] float _curveOffset = 1f;
    [SerializeField] float _attractionCap = 10f;
    [SerializeField] private float _jitterDistance = 0.2f;
    [SerializeField, Range(0,1)] float _centerAdjustment = 1f;
    [SerializeField] private bool _logAttraction = true;
    [Header("Spring Mode")]
    [SerializeField] private bool _doSpringMode = false;
    [SerializeField] private float _springStrength = 120f;
    [SerializeField] private float _dampingForce = 12f;
    [SerializeField] private float _springAttractionCap = 40f;

    [Header("Poke")]
    [SerializeField] private LayerMask _pokeLayers;
    [SerializeField] GameObject _pokeOrigin;

    // Fields
    private float _rotation = 0f;
    private float _sweepForce = 1f;

    // Unity methods
    private void Awake()
    {
        _hitbox = GetComponent<Collider2D>();
        _hitbox.enabled = false;
        _sweepSoundInstance = RuntimeManager.CreateInstance("event:/Player/Sweep/Sweep");


        _pokebox = GetComponentInChildren<BroomPokebox>();
        _pokebox.Initialize(this);

        _broomhead = GetComponentInChildren<Broomhead>();
        _broomhead.Initialize(this);

        _sweepSingularity = GetComponentInChildren<SweepSingularity>().transform;
    }

    public void Initialize(PlayerMovementController player, PlayerContext ctx)
    {
        _parent = player;
        _ctx = ctx;
    }

    // Sweep
    public void BeginSweep(float rotation, float sweepForce)
    {
        _hitbox.enabled = true;
        _broomhead.Active = true;
        if(_hitbox.enabled)
            _sweepSoundInstance.start();
        else
        {
            _sweepSoundInstance.release();
        }
        _sweepForce = sweepForce;
        UpdateHitbox(rotation);
    }

    public void UpdateHitbox(float rotation)
    {
        _rotation = rotation * Mathf.Deg2Rad;
        Vector2 offset = new Vector2(Mathf.Cos(_rotation), Mathf.Sin(_rotation));
        
        // Set the sweep box position and rotation relative to the player and their rotation
        transform.position = _parent.transform.position + new Vector3(0, -0.25f, 0f) + ((Vector3)offset * 0.5f);
        transform.rotation = Quaternion.Euler(0, 0, rotation + 90f);
    }

    public void EndSweep()
    {
        _hitbox.enabled = false;
        _sweepSoundInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        //AudioManager.Instance.Stop(gameObject,"Sweep");
        
         _broomhead.Active = false;
       
    }

    // Collision trigger
    private void OnTriggerStay2D(Collider2D collider)
    {
        if (_doSpringMode)
        {
            SpringMode(collider);
            return;
        }
        Vector2 directionOut = new Vector2(Mathf.Cos(_rotation), Mathf.Sin(_rotation)).normalized;

        ISweepable sweepableObject = collider.gameObject.GetComponent<ISweepable>();
        if (sweepableObject == null) return;


        Vector2 trueSingularity = (Vector2) _sweepSingularity.position + (directionOut * sweepableObject.SizeRadius * _centerAdjustment); 
        marker.transform.position = trueSingularity;
        float dist = Vector2.Distance(trueSingularity, collider.transform.position);

        if (dist < _jitterDistance) return;

        float rawAttractionForce = Mathf.Pow(_curveSteepness, dist - _curveOffset);
        float attractionForce = Mathf.Clamp(rawAttractionForce, 0 ,_attractionCap);
        if (_logAttraction) Debug.Log($"Attraction: {attractionForce}");


        Vector2 direction = (trueSingularity - (Vector2) collider.transform.position).normalized;

        sweepableObject.OnSweep(transform.position, direction.normalized, attractionForce);
    }

    private void SpringMode(Collider2D collider)
    {
        Vector2 directionOut = new Vector2(Mathf.Cos(_rotation), Mathf.Sin(_rotation)).normalized;

        ISweepable sweepableObject = collider.gameObject.GetComponent<ISweepable>();
        if (sweepableObject == null) return;

        Vector2 trueSingularity = (Vector2)_sweepSingularity.position + (directionOut * sweepableObject.SizeRadius * _centerAdjustment);
        marker.transform.position = trueSingularity;

        Vector2 toTarget = trueSingularity - (Vector2)collider.transform.position;

        float dist = toTarget.magnitude;
        Vector2 direction = toTarget.normalized;

        Rigidbody2D rb = collider.attachedRigidbody;

        float towardSpeed = 0f;
        if (rb != null)
        {
            towardSpeed = Vector2.Dot(rb.linearVelocity, direction);
        }

        float springForce = dist * _springStrength;
        float dampingForce = towardSpeed * _dampingForce;

        float attractionForce = Mathf.Clamp(springForce - dampingForce, 0f, _springAttractionCap);

        if (_logAttraction) Debug.Log($"Attraction: {attractionForce}");

        sweepableObject.OnSweep(transform.position, direction, attractionForce);
    }

    // Sweep Poke
    public void DoPoke(float rotation, float pokeForce)
    {
        _pokebox.Active = true;
        UpdateHitbox(rotation);

        Vector2 pokeDirection = new Vector2(Mathf.Cos(_rotation), Mathf.Sin(_rotation));

        CapsuleCollider2D capsule = _pokebox.Collider;

        // Get collider bounds
        Vector2 worldCenter = capsule.transform.TransformPoint(capsule.offset);
        Vector2 size = new Vector2(
            capsule.size.x * Mathf.Abs(capsule.transform.lossyScale.x),
            capsule.size.y * Mathf.Abs(capsule.transform.lossyScale.y)
        );
        CapsuleDirection2D direction = capsule.direction;
        float angle = capsule.transform.eulerAngles.z;

        // Do hit and process data
        Collider2D[] hits = Physics2D.OverlapCapsuleAll(worldCenter, size, direction, angle, _pokeLayers);        
        List<(Collider2D collider, float distance)> hitColliders = HitProcessor.ProcessHits<IPokeable>(hits, _pokeOrigin.transform.position);

        foreach ((Collider2D collider, float distance) entry in hitColliders)
        {
            Collider2D collider = entry.collider;
            if (!collider.TryGetComponent(out IPokeable pokeable)) continue;

            float knockbackMultiplier = 0f;
            pokeable.OnPoke(pokeDirection, pokeForce, collider, ref knockbackMultiplier);
            // Apply resulting knockback (if there is any)
            if (knockbackMultiplier > 0f)
            {
                float knockbackForce =  knockbackMultiplier * _ctx.Rigidbody.mass * pokeForce / collider.attachedRigidbody.mass;
                _ctx.Rigidbody.AddForce(-pokeDirection * knockbackForce * _parent.PokeKnockbackMultiplier, ForceMode2D.Impulse);
            }
            break; //Poke only hits the first thing
        }
    }

    public void EndPoke()
    {
        _pokebox.Active = false;
    }

    // 
    public void OnPokeTrigger(Collider2D collider)
    {
        // This trigger can potentially be used for things such as attack parries if they are added later.
    }


}
