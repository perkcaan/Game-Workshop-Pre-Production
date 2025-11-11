using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class ShaderManager : MonoBehaviour
{
    // Properties
    [Tooltip("The maximum rate per second at which this object flashes.")]
    [SerializeField] private float _maxFlashFrequency = 10f;
    [Tooltip("How fast flashing ramps up after reaching warning threshold. (Exponential)")]
    [SerializeField] private float _heatFlashRamp = 2f;
    [Tooltip("The length of time for dissolve animation.")]
    [SerializeField] private float _dissolveTime = 0.5f;
    [Tooltip("The width of the 'burn' effect on the dissolve animation.")]
    [SerializeField] private float _dissolveBurnWidth = 0.4f;
    [Tooltip("Texture to use this Shader with mesh. Leave empty if using a SpriteRenderer.")]
    [SerializeField] private Texture _meshTexture;
    
    private float _flashPhase = 0f; // This is the time spent in the heat warning threshold.
    // Components
    private Renderer _renderer;
    private MaterialPropertyBlock _block;

    private void Awake()
    {
        _renderer = GetComponent<Renderer>();
        _block = new MaterialPropertyBlock();
    }

    private void Start()
    {
        _renderer.GetPropertyBlock(_block);

        if (_meshTexture != null) _block.SetTexture("_MainTex", _meshTexture);
        _renderer.SetPropertyBlock(_block);
    }
    
    public void Reset()
    {
        _flashPhase = 0f;

        _renderer.GetPropertyBlock(_block);
        _block.SetFloat("_FlashPhase", _flashPhase);
        _block.SetFloat("_Dissolve", 0f);
        _renderer.SetPropertyBlock(_block);
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


        _renderer.GetPropertyBlock(_block);
        _block.SetFloat("_Heat", heat01);
        _block.SetFloat("_FlashPhase", _flashPhase);
        _renderer.SetPropertyBlock(_block);
    }
    
    // Have this be called on 
    public void StartDissolve(Action onDone = null)
    {
        float maxDissolve = 1.05f + _dissolveBurnWidth;

        _renderer.GetPropertyBlock(_block);
        _block.SetFloat("_BurnWidth", _dissolveBurnWidth);
        _block.SetFloat("_IsDissolving", true ? 1f : 0f);
        _renderer.SetPropertyBlock(_block);
        Tween tween = DOVirtual.Float(0, maxDissolve, _dissolveTime, val =>
        {
            _block.SetFloat("_Dissolve", val);
            _renderer.SetPropertyBlock(_block);
        }).OnComplete(() =>
        {
            _block.SetFloat("_IsDissolving", false ? 1f : 0f);
            _renderer.SetPropertyBlock(_block);
            onDone?.Invoke(); // Call back when tween is finished
        });

        
    }
}
