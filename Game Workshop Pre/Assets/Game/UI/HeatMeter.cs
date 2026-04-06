using UnityEngine;
using UnityEngine.UI;

public class HeatMeter : MonoBehaviour
{
    [SerializeField] private HeatMechanic _targetHeatMechanic;
    [SerializeField] private Image _fillImage;
    [SerializeField] private Image _outline;

    [Header("Colors")]
    [SerializeField] private Gradient _heatGradient;
    [SerializeField] private Color _flashColor = Color.white;
    [SerializeField] private Color _outlineBaseColor = new Color(1, 1, 1, 0); 
    [SerializeField] private float _flashSpeed = 5f;

    [Header("Outline Effects")]
    [SerializeField] private float _hitHoldDuration = 0.5f; 
    [SerializeField] private float _highHeatPulseSpeed = 10f;

    [Header("Shake Settings")]
    [SerializeField] private float _shakeIntensity = 5f;
    [SerializeField] private float _heatDamageShakeIntensity = 1.5f;
    [SerializeField] private float _shakeSharpThreshold = 10f;
    [SerializeField] private float _shakeHighThreshold = 0.85f;

    private RectTransform _rectTransform;
    private Vector2 _originalPosition;
    private float _lastHeat;
    private float _fillFlashIntensity;
    
    private float _outlineHitTimer;
    private float _outlineFadeIntensity;

    private void Start()
    {
        if (_targetHeatMechanic == null || _fillImage == null || _outline == null)
        {
            Debug.LogError("Please set up Heat Meter UI components.");
            enabled = false;
            return;
        }
        
        _rectTransform = GetComponent<RectTransform>();
        _originalPosition = _rectTransform.anchoredPosition;
        _lastHeat = _targetHeatMechanic.Heat;

        _outline.color = _outlineBaseColor;
    }

    private void Update()
    {
        float currentHeat = _targetHeatMechanic.Heat;
        float maxRange = HeatMechanic.HIGHEST_HEAT_VALUE - HeatMechanic.LOWEST_HEAT_VALUE;
        float heatProgress = Mathf.Clamp01((currentHeat - HeatMechanic.LOWEST_HEAT_VALUE) / maxRange);
        float heatChange = currentHeat - _lastHeat;

        if (heatChange > _shakeSharpThreshold)
        {
            _outlineHitTimer = _hitHoldDuration;
            _outlineFadeIntensity = 1f;
            _fillFlashIntensity = 1f; 
        }
        else if (heatChange > 0)
        {
            _fillFlashIntensity = 1f;
        }

        if (_outlineHitTimer > 0)
        {
            _outline.color = _flashColor;
            _outlineHitTimer -= Time.deltaTime;
        }
        else if (heatProgress >= _shakeHighThreshold)
        {
            float pulse = Mathf.PingPong(Time.time * _highHeatPulseSpeed, 1f);
            _outline.color = Color.Lerp(_outlineBaseColor, _flashColor, pulse);
        }
        else
        {
            _outlineFadeIntensity = Mathf.MoveTowards(_outlineFadeIntensity, 0f, Time.deltaTime * _flashSpeed);
            _outline.color = Color.Lerp(_outlineBaseColor, _flashColor, _outlineFadeIntensity);
        }

        _fillFlashIntensity = Mathf.MoveTowards(_fillFlashIntensity, 0f, Time.deltaTime * _flashSpeed);
        Color baseFillColor = _heatGradient.Evaluate(heatProgress);
        _fillImage.color = Color.Lerp(baseFillColor, _flashColor, _fillFlashIntensity);
        _fillImage.fillAmount = heatProgress * 1.05f;

        HandleShake(heatChange, heatProgress);

        _lastHeat = currentHeat;
    }

    private void HandleShake(float heatChange, float heatProgress)
    {
        Vector2 shakeOffset = Vector2.zero;

        if (heatChange > _shakeSharpThreshold)
        {
            shakeOffset = Random.insideUnitCircle * (_heatDamageShakeIntensity * heatChange);
        }
        else if (heatProgress >= _shakeHighThreshold)
        {
            shakeOffset = Random.insideUnitCircle * (_shakeIntensity * heatProgress);
        }

        if (shakeOffset != Vector2.zero)
        {
            _rectTransform.anchoredPosition = _originalPosition + shakeOffset;
        }
        else
        {
            _rectTransform.anchoredPosition = Vector2.Lerp(_rectTransform.anchoredPosition, _originalPosition, Time.deltaTime * 10f);
        }
    }
}