using UnityEngine;
using UnityEngine.UI;

public class HeatMeter : MonoBehaviour
{
    [SerializeField] private HeatMechanic _targetHeatMechanic;
    [SerializeField] private Image _fillImage;
    [SerializeField] private Image _outline;
    [SerializeField] private Image _skullEyes;
    [SerializeField] private RectTransform _skullJaw;
    [SerializeField] private float _heatFillMultiplier = 1.075f;

    [Header("Colors")]
    [SerializeField] private Gradient _heatGradient;
    [SerializeField] private Color _flashColor = Color.white;
    [SerializeField] private Color _outlineBaseColor =  Color.white; 
    [SerializeField] private Color _baseEyeColor = Color.white;
    [SerializeField] private Color _eyeDamageColor =  Color.white; 
    [SerializeField] private float _flashSpeed = 5f;
    [SerializeField] private float _eyeFlashSpeed = 2f;

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
    private Vector2 _originalJawPosition;
    private float _lastHeat;
    private float _fillFlashIntensity;
    private float _eyeFlashIntensity;

    private float _outlineHitTimer;
    private float _outlineFadeIntensity;

    private void Start()
    {
        if (_targetHeatMechanic == null || _fillImage == null || _outline == null)
        {
            //Debug.LogError("Please set up Heat Meter UI components.");
            enabled = false;
            return;
        }
        
        _rectTransform = GetComponent<RectTransform>();
        _originalPosition = _rectTransform.anchoredPosition;
        _originalJawPosition = _skullJaw.anchoredPosition;
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
            _eyeFlashIntensity = 1f;
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
            _eyeFlashIntensity = Mathf.PingPong(Time.time * _highHeatPulseSpeed, 0.5f) + 0.5f;
        }
        else
        {
            _outlineFadeIntensity = Mathf.MoveTowards(_outlineFadeIntensity, 0f, Time.deltaTime * _flashSpeed);
            _outline.color = Color.Lerp(_outlineBaseColor, _flashColor, _outlineFadeIntensity);
        }

        _fillFlashIntensity = Mathf.MoveTowards(_fillFlashIntensity, 0f, Time.deltaTime * _flashSpeed);
        Color baseFillColor = _heatGradient.Evaluate(heatProgress);
        _fillImage.color = Color.Lerp(baseFillColor, _flashColor, _fillFlashIntensity);
        _fillImage.fillAmount = heatProgress * _heatFillMultiplier;

        _eyeFlashIntensity = Mathf.MoveTowards(_eyeFlashIntensity, 0f, Time.deltaTime * _eyeFlashSpeed);
        _skullEyes.color = Color.Lerp(_baseEyeColor, _eyeDamageColor, _eyeFlashIntensity);

        HandleShake(heatChange, heatProgress);

        _lastHeat = currentHeat;
    }

    private void HandleShake(float heatChange, float heatProgress)
    {
        Vector2 shakeOffset = Vector2.zero;
        Vector2 jawOffset = Vector2.zero;

        if (heatChange > _shakeSharpThreshold) {
            shakeOffset = Random.insideUnitCircle * (_heatDamageShakeIntensity * heatChange);
            jawOffset = new Vector2(0, -_heatDamageShakeIntensity * heatChange);
        }
        else if (heatProgress >= _shakeHighThreshold) {
            float currentOffset = (_rectTransform.anchoredPosition - _originalPosition).magnitude;
            if (currentOffset < _shakeIntensity * heatProgress / 3f)
            {
                shakeOffset = Random.insideUnitCircle * (_shakeIntensity * heatProgress);
                jawOffset = new Vector2(0, -_shakeIntensity * heatProgress);  
            }
        }

        if (shakeOffset != Vector2.zero) {
            _rectTransform.anchoredPosition = _originalPosition + shakeOffset;
            _skullJaw.anchoredPosition = _originalJawPosition + jawOffset;
        }
        else {
            _rectTransform.anchoredPosition = Vector2.Lerp(_rectTransform.anchoredPosition, _originalPosition, Time.deltaTime * 10f);
            _skullJaw.anchoredPosition = Vector2.Lerp(_skullJaw.anchoredPosition, _originalJawPosition, Time.deltaTime * 5f);
        }
    }
}