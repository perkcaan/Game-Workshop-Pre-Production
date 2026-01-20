using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using static Unity.Collections.AllocatorManager;

public class HiddenArea : MonoBehaviour
{
    private MaterialPropertyBlock block;
    private TilemapRenderer _renderer;
    private Tween opacityTween;

    private float opacity = 1f;
    [SerializeField] float transitionTime = 2f;

    private void Awake()
    {
        block = new MaterialPropertyBlock();
        _renderer = GetComponent<TilemapRenderer>();
    }

    void Update()
    {
        _renderer.GetPropertyBlock(block);
        block.SetFloat("_Opacity", opacity);
        _renderer.SetPropertyBlock(block);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            //reveal area
            SetOpacity(0f);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            //hide area
            SetOpacity(1f);
        }
    }

    void SetOpacity(float target)
    {
        //kill any tween already running
        opacityTween?.Kill();

        //distance remaining
        float distance = Mathf.Abs(opacity - target);

        //time proportional to distance 
        float duration = transitionTime * distance;

        opacityTween = DOTween.To(
            () => opacity,
            x => opacity = x,
            target,
            duration
        ).SetEase(Ease.Linear);
    }


}
