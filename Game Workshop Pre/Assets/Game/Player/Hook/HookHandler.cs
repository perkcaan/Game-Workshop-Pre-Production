using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Handles the player's swipe ability
public class HookHandler : MonoBehaviour
{

    // Components
    private PlayerMovementController _parent;
    private LineRenderer _lineRenderer;

    [Header("Hook Setup")]
    [SerializeField] private Transform _hookHead;
    [SerializeField] private float _maxDistance = 6f;
    // Fields
    private bool _isActive = false;
    private bool _isRetracting = false;
    private Vector2 _direction;
    private float _currentSpeed;
    private float _pullForce;
    public bool IsActive => _isActive;

    private void Awake()
    {
        _parent = transform.parent.GetComponent<PlayerMovementController>();
        _lineRenderer = GetComponent<LineRenderer>();

        if (_parent == null)
        {
            Debug.LogWarning("Player Hook Handler cannot find Player.");
            gameObject.SetActive(false);
            return;
        }

        _lineRenderer.positionCount = 2;
        _lineRenderer.enabled = false;

        if (_hookHead != null)
        {
            HookHead headScript = _hookHead.GetComponent<HookHead>();
            if (headScript != null) headScript.Initialize(this);

            _hookHead.gameObject.SetActive(false);
            _hookHead.parent = null;
        }
    }

    private void Update()
    {
        if (!_isActive || _hookHead == null) return;

        MoveHook();
        DrawRope();
    }
    public void ThrowHook(float angle, float pullForce, float duration)
    {
        if (_isActive) return;

        _isActive = true;
        _isRetracting = false;
        _pullForce = pullForce;

        _currentSpeed = _maxDistance / (duration / 2f);

        float radians = angle * Mathf.Deg2Rad;
        _direction = new Vector2(Mathf.Cos(radians), Mathf.Sin(radians));

        _hookHead.position = _parent.transform.position;
        _hookHead.rotation = Quaternion.Euler(0, 0, angle - 90f);
        _hookHead.gameObject.SetActive(true);

        _lineRenderer.enabled = true;
    }

    private void MoveHook()
    {
        float step = _currentSpeed * Time.deltaTime;

        if (!_isRetracting)
        {
            _hookHead.Translate(Vector3.up * step);
            float dist = Vector2.Distance(_parent.transform.position, _hookHead.position);
            
            if (dist >= _maxDistance)
            {
                StartRetract();
            }
        }
        else
        {
            _hookHead.position = Vector2.MoveTowards(_hookHead.position, _parent.transform.position, step);

            if (Vector2.Distance(_hookHead.position, _parent.transform.position) < 0.2f)
            {
                EndHook();
            }
        }
    }

    private void DrawRope()
    {
        if (_parent == null || _hookHead == null) return;
        _lineRenderer.SetPosition(0, _parent.transform.position);
        _lineRenderer.SetPosition(1, _hookHead.position);
    }

    public void StartRetract()
    {
        _isRetracting = true;
    }

    private void EndHook()
    {
        _isActive = false;
        _lineRenderer.enabled = false;
        _hookHead.gameObject.SetActive(false);
    }

    public void OnHookHit(Collider2D other)
    {
        if (!_isActive || _isRetracting) return;

        float knockbackMultiplier = 0f;

        if (other.TryGetComponent(out PlayerMovementController player)) return;
        if (other.TryGetComponent(out Sticky sticky)) {
            Vector2 directionToPlayer = (_parent.transform.position - _hookHead.position).normalized;
            _parent.OnSwipe(directionToPlayer, -_pullForce * 2, other, ref knockbackMultiplier);
            StartRetract();
            return;
        }
        if (other.TryGetComponent(out Wall wall)) {
            StartRetract();
            return;
        }

        if (other.TryGetComponent(out ISwipeable swipeable)) {
            Vector2 directionToPlayer = (_parent.transform.position - _hookHead.position).normalized;
            swipeable.OnSwipe(directionToPlayer, _pullForce, other, ref knockbackMultiplier);
        
            // ParticleManager.Instance.Play("HookHit", _hookHead.position);
            StartRetract();
        }
    }

    private void OnDestroy()
    {
        if (_hookHead != null) Destroy(_hookHead.gameObject);
    }
}
