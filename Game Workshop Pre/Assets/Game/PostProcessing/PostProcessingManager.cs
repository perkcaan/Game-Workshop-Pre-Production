using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;


// Manages post-processing effects in the game.
public class PostProcessingManager : Singleton<PostProcessingManager>
{

    private Volume _globalVolume;
    private VolumeProfile _runtimeProfile;
    private ColorAdjustments _colorAdjustments;

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
        Debug.Log("Toggled: " + enabled);
        if (enabled)
        {
            _colorAdjustments.saturation.value = -100f;
        } else
        {
            _colorAdjustments.saturation.value = 0f;
        }
    }

}
