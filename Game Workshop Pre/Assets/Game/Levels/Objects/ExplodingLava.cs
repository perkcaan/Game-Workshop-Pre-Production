using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class ExplodingLava : MonoBehaviour
{
    [SerializeField] private float _timeUntilWarn = 5f;
    [SerializeField] private float _warningDuration = 3f;
    [SerializeField] private float _explosionDuration = 4f;
    [SerializeField] private float _maxRandomTimeOffset = 1f;
    private HeatAreaHitbox _heatArea;
    private SpriteRenderer _spriteRenderer;
    private bool _isExploding = false;
    private bool _isWarning = false;
    private float _currentTime = 0f;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _heatArea = GetComponent<HeatAreaHitbox>();
    }

    private void Start()
    {
        _spriteRenderer.color = Color.white;
        _currentTime += Random.Range(0,_maxRandomTimeOffset);
    }

    private void Update()
    {
        _currentTime += Time.deltaTime;
        
        if (_isExploding && _currentTime >= _timeUntilWarn + _explosionDuration + _warningDuration)
        {
            FinishExplode();
            return;
        }

        if (!_isExploding && _currentTime >= _timeUntilWarn + _warningDuration)
        {
            StartExplode();
            return;
        }

        if (!_isWarning && _currentTime >= _timeUntilWarn) 
        {
            StartWarning();
            return;
        }
    }

    private void StartWarning()
    {
        _spriteRenderer.color = Color.yellow;
        _isWarning = true;
    } 

    private void StartExplode()
    {
        _spriteRenderer.color = Color.red;
        _isExploding = true;
        _heatArea.Enable();
    }

    private void FinishExplode()
    {
        _currentTime = 0f;
        _spriteRenderer.color = Color.white;
        _isExploding = false;
        _isWarning = false;
        _heatArea.Disable();
    }
}
