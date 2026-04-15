using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UIElements;


// Manages post-processing effects in the game.
public class PostProcessingManager : Singleton<PostProcessingManager>
{

    private Volume _globalVolume;
    private VolumeProfile _runtimeProfile;
    private ColorAdjustments _colorAdjustments;

    private Tween _colorTween;

    protected override void Awake()
    {
        base.Awake();
        _globalVolume = GetComponent<Volume>();
    }

    private void Start()
    {
        // Clone the profile so runtime changes are runtime only
        _runtimeProfile = Instantiate(_globalVolume.profile);
        _globalVolume.profile = _runtimeProfile;

        if (!_runtimeProfile.TryGet(out _colorAdjustments))
        {
            _colorAdjustments = _runtimeProfile.Add<ColorAdjustments>(true);
        }

    }

    public void ColorBlindToggle(bool enabled)
    {
        _colorTween?.Kill();

        float target = enabled ? -100f : 0f;
        float current = _colorAdjustments.saturation.value;

        // Proportional duration
        float fullDuration = 1f;
        float duration = fullDuration * Mathf.Abs(current - target) / 100f;

        _colorTween = DOVirtual.Float(current, target, duration, val =>
        {
            _colorAdjustments.saturation.value = val;
        }).SetEase(Ease.InOutQuad).SetUpdate(true);
    }

}
