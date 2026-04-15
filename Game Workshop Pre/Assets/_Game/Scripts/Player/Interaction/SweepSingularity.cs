using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SweepSingularity : MonoBehaviour
{
    [SerializeField] private bool _showSprite = false;
    private SpriteRenderer _spriteRenderer;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _spriteRenderer.enabled = _showSprite;
    }
}
