using System.Collections.Generic;
using UnityEngine;

public class EventGate : MonoBehaviour
{
    private SpriteRenderer _spriteRenderer;
    private Collider2D _collider;
    [SerializeField] private bool _isGateUp = true;
    [SerializeField] private bool _isHorizontal;
    private void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _collider = GetComponent<Collider2D>();
        _spriteRenderer.enabled = false;
        _collider.enabled = false;
        if (_isGateUp) Close();
    }

    [EventAction]
    public void Open()
    {
        _spriteRenderer.enabled = false;
        _collider.enabled = false;
        _isGateUp = false;
        if (_isHorizontal)
        {
            ParticleManager.Instance.Play("GateOpenH", transform.position);
        }
        else
        {
            ParticleManager.Instance.Play("GateOpenV", transform.position);
        }
    }

    [EventAction]
    public void Close()
    {
        _spriteRenderer.enabled = true;
        _collider.enabled = true;
        _isGateUp = true;
        if (_isHorizontal)
        {
            ParticleManager.Instance.Play("GateOpenH", transform.position);
        }
        else
        {
            ParticleManager.Instance.Play("GateOpenV", transform.position);
        }
    }
}
