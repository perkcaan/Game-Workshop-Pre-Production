using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening; // Make sure DOTween is included

public class CleanBar : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private RectTransform _cleanBarRect; // Changed to RectTransform for UI sliding
    [SerializeField] private Image _outline;
    [SerializeField] private Image _fillImage;
    [SerializeField] private TMP_Text _cleanText;

    [Header("Animation Settings")]
    [SerializeField] private float _slideDuration = 0.5f;
    [SerializeField] private float _fillDuration = 0.3f;
    [SerializeField] private float _outlineFadeDuration = 1f;
    [SerializeField] private float _visibleYPos = -50f; // Target Y anchored position when on screen
    [SerializeField] private float _hiddenYPos = 150f;  // Target Y anchored position when off screen

    private float _lastTargetFill = -1f;
    private bool _isShowing = false;
    
    // Storing tween references so we can overwrite them if values change mid-animation
    private Tweener _fillTweener;
    private Tweener _outlineTweener;

    private void Start()
    {
        if (_cleanBarRect == null || _fillImage == null || _cleanText == null)
        {
            Debug.LogError("Please set up Clean Bar UI");
            enabled = false;
            return;
        }

        _cleanBarRect.anchoredPosition = new Vector2(_cleanBarRect.anchoredPosition.x, _hiddenYPos);
        _cleanBarRect.gameObject.SetActive(false);
        
        if (_outline != null)
        {
            Color c = _outline.color;
            c.a = 0f;
            _outline.color = c;
        }
    }

    private void Update()
    {
        float targetFill = DistrictManager.Instance?.FocusedRoom?.Cleanliness ?? 1f;

        if (targetFill < 1f && !_isShowing)
        {
            DisplayCleanBar();
        }
        else if (targetFill >= 1f && _isShowing)
        {
            HideCleanBar();
        }

        if (Mathf.Abs(targetFill - _lastTargetFill) > 0.001f)
        {
            UpdateCleanliness(targetFill);
        }
    }

    private void UpdateCleanliness(float targetFill)
    {
        float change = targetFill - _lastTargetFill;
        _lastTargetFill = targetFill;

        _fillTweener?.Kill();
        _fillTweener = _fillImage.DOFillAmount(targetFill, _fillDuration)
            .SetEase(Ease.OutQuad)
            .OnUpdate(() => 
            {
                int percent = Mathf.FloorToInt(_fillImage.fillAmount * 100f);
                _cleanText.text = $"Objective: {percent}% clean";
            });

        if (change > 0 && _outline != null)
        {
            FlashOutline(change);
        }
    }

    private void FlashOutline(float intensity)
    {
        _outlineTweener?.Kill();

        float targetAlpha = Mathf.Clamp(intensity * 5f, 0.4f, 1f); 
        
        Color outlineColor = _outline.color;
        outlineColor.a = targetAlpha;
        _outline.color = outlineColor;

        _outlineTweener = _outline.DOFade(0f, _outlineFadeDuration).SetEase(Ease.InOutQuad);
    }

    private void DisplayCleanBar()
    {
        if (_isShowing) return;
        _isShowing = true;

        _cleanBarRect.gameObject.SetActive(true);
        _fillImage.fillAmount = 0;
        _cleanBarRect.DOAnchorPosY(_visibleYPos, _slideDuration)
            .SetEase(Ease.OutBack);
    }

    private void HideCleanBar()
    {
        if (!_isShowing) return;
        _isShowing = false;

        _cleanBarRect.DOAnchorPosY(_hiddenYPos, _slideDuration)
            .SetEase(Ease.InBack)
            .OnComplete(() => _cleanBarRect.gameObject.SetActive(false));
    }
}