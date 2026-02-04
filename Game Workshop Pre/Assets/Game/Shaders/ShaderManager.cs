using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class ShaderManager : MonoBehaviour
{
    // Properties
    [SerializeField] private List<Renderer> _renderers = new List<Renderer>();
        
    [Tooltip("The maximum rate per second at which this object flashes.")]
    [SerializeField] private float _maxFlashFrequency = 10f;
    [Tooltip("How fast flashing ramps up after reaching warning threshold. (Exponential)")]
    [SerializeField] private float _heatFlashRamp = 2f;
    [Tooltip("The length of time for dissolve animation.")]
    [SerializeField] private float _dissolveTime = 0.5f;
    [Tooltip("The width of the 'burn' effect on the dissolve animation.")]
    [SerializeField] private float _dissolveBurnWidth = 0.4f;
    [Tooltip("The time it takes to sink in lava.")]
    [SerializeField] private float _height = 1.0f;
    [Tooltip("Texture to use this Shader with mesh. Leave empty if using a SpriteRenderer.")]
    [SerializeField] private Texture _meshTexture;

    private float _flashPhase = 0f; // This is the time spent in the heat warning threshold.
    private MaterialPropertyBlock _block;
    private Coroutine _dissolveCoroutine;
    private Coroutine _lavaCoroutine;

    private void Awake()
    {
        if (_renderers.Count <= 0) _renderers.Add(GetComponent<Renderer>());
        DOTween.Init(true, true, LogBehaviour.ErrorsOnly); 
    }
    private void Start()
    {
        _block = new MaterialPropertyBlock();
    }

    private void SetFloatProperties(string propRef, float propVal)
    {
        foreach (Renderer renderer in _renderers)
        {
            renderer.GetPropertyBlock(_block);
            _block.SetFloat(propRef, propVal);
            renderer.SetPropertyBlock(_block);
        }
    }
    
    public void Reset()
    {
        _flashPhase = 0f;

        SetFloatProperties("_FlashPhase", _flashPhase);
        SetFloatProperties("_Dissolve", 0f);
    }


    // Have this be called from HeatMechanic
    public void UpdateHeatShader(float heat, int warningThreshold, int ignitionThreshold)
    {

        // Get heat from _warningThreshold to _ignitionThreshold as 0-1 float 
        float heat01 = Mathf.Clamp01((heat - warningThreshold) / (ignitionThreshold - warningThreshold));

        // If above warning threshold, flash according to frequency and heat
        if (heat >= warningThreshold)
        {
            float xExp = Mathf.Pow(heat01, _heatFlashRamp);
            float flashSpeed = Mathf.Lerp(0, _maxFlashFrequency, xExp);
            _flashPhase += Time.deltaTime * flashSpeed;
        }
        else
        {
            _flashPhase = 0f;
        }


        SetFloatProperties("_Heat", heat01);
        SetFloatProperties("_FlashPhase", _flashPhase);
    }

    // Have this be called on Heat Mechanic
    public void StartDissolve(Action onDone = null)
    {
        if (_dissolveCoroutine != null)
        {
            StopCoroutine(_dissolveCoroutine);
        }

        _dissolveCoroutine = StartCoroutine(DissolveCoroutine(onDone));
    }



    private IEnumerator DissolveCoroutine(Action onDone)
    {
        float maxDissolve = 1.05f + _dissolveBurnWidth;
        float time = 0f;

        SetFloatProperties("_BurnWidth", _dissolveBurnWidth);
        SetFloatProperties("_IsDissolving", 1f);

        while (time < _dissolveTime)
        {
            float progress = time / _dissolveTime;
            float dissolveVal = Mathf.Lerp(0f, maxDissolve, progress);
            SetFloatProperties("_Dissolve", dissolveVal);
            time += Time.deltaTime;
            yield return null;
        }

        SetFloatProperties("_Dissolve", maxDissolve);
        SetFloatProperties("_IsDissolving", 0f);

        _dissolveCoroutine = null;
        onDone?.Invoke();
    }

    //Called on lava script
    public void SinkInLava(Action onDone = null)
    {
        if (_lavaCoroutine != null)
        {
            StopCoroutine(_lavaCoroutine);
        }

        _lavaCoroutine = StartCoroutine(LavaCoroutine(onDone));
    }

    private IEnumerator LavaCoroutine(Action onDone)
    {
        float time = 0f;

        while (time < 3f)
        {
            float height = time / 3f;
            SetFloatProperties("_height", height);
            time += Time.deltaTime;
            yield return null;
        }

        SetFloatProperties("_height", 1);

        _lavaCoroutine = null;
        onDone?.Invoke();
    }
}
