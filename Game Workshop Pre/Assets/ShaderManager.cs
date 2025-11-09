using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class ShaderManager : MonoBehaviour
{
    // Properties
    [SerializeField] private float _dissolveTime = 0.5f; 


    private Renderer _renderer;
    private MaterialPropertyBlock _block;


    private Tween _dissolveTween;

    private void Awake()
    {
        _renderer = GetComponent<SpriteRenderer>();
        _block = new MaterialPropertyBlock();
    }

    private void Start()
    {
        StartDissolve();
    }
    
    private void StartDissolve()
    {
        _renderer.GetPropertyBlock(_block);
        float burnWidth = _renderer.material.GetFloat("_BurnWidth");
        float maxDissolve = 1.05f + burnWidth;
        DOVirtual.Float(0, maxDissolve, _dissolveTime, val =>
        {
            _block.SetFloat("_Dissolve", val);
            _renderer.SetPropertyBlock(_block);
        });
        
    }
}
