using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Timeline;

public class EnemySweepHandler : MonoBehaviour
{


    private Collider2D _hitbox;
    [SerializeField]private Transform _sweepSingularity;
    [SerializeField] float _curveSteepness = 1.1f;
    [SerializeField] float _curveOffset = 1f;
    [SerializeField] float _attractionCap = 10f;
    [SerializeField] private float _jitterDistance = 0.2f;
    [SerializeField, Range(0, 1)] float _centerAdjustment = 1f;
    public float _sweepForce;
    private Broomhead _broomhead;
    float orientation;
    //public GameObject marker;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _hitbox = GetComponent<Collider2D>();
        _hitbox.enabled = false;
        _sweepSingularity = GetComponentInChildren<SweepSingularity>().transform;
        _broomhead = GetComponentInChildren<Broomhead>();

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void BeginSweep(float rotation, float sweepForce)
    {
        
        _hitbox.enabled = true;
        _broomhead.Active = true;
        _sweepForce = sweepForce;
        UpdateHitbox(rotation);

    }

    public void UpdateHitbox(float rotation)
    {
        orientation = rotation * Mathf.Deg2Rad;
        Vector2 offset = new Vector2(Mathf.Cos(orientation), Mathf.Sin(orientation));

        // Set the sweep box position and rotation relative to the player and their rotation
        transform.position = transform.parent.transform.position;
        transform.rotation = Quaternion.Euler(0, 0, rotation + 90f);
    }

    public void EndSweep()
    {
        _hitbox.enabled = false;
        
        //AudioManager.Instance.Stop(gameObject,"Sweep");

        _broomhead.Active = false;

    }

    private void OnTriggerStay2D(Collider2D collider)
    {
        Vector2 directionOut = new Vector2(Mathf.Cos(orientation), Mathf.Sin(orientation)).normalized;

        ISweepable sweepableObject = collider.gameObject.GetComponent<ISweepable>();
        if (sweepableObject == null) return;


        Vector2 trueSingularity = (Vector2)_sweepSingularity.position + (directionOut * sweepableObject.SizeRadius * _centerAdjustment);
        
        float dist = Vector2.Distance(trueSingularity, collider.transform.position);

        if (dist < _jitterDistance) return;

        float rawAttractionForce = Mathf.Pow(_curveSteepness, dist - _curveOffset);
        float attractionForce = Mathf.Clamp(rawAttractionForce, 0, _attractionCap);
        


        Vector2 direction = (trueSingularity - (Vector2)collider.transform.position).normalized;

        sweepableObject.OnSweep(transform.position, direction.normalized, attractionForce);
    }
}

