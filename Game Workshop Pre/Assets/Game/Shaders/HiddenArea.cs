using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using static Unity.Collections.AllocatorManager;

public class HiddenArea : MonoBehaviour
{
    [SerializeField] Material mat;
    private MaterialPropertyBlock block;
    private TilemapRenderer renderer;
    [SerializeField] float opacity = 1f;


    private void Awake()
    {
        block = new MaterialPropertyBlock();
        renderer = GetComponent<TilemapRenderer>();
    }

    void Update()
    {
        Debug.Log(opacity);
        renderer.GetPropertyBlock(block);
        block.SetFloat("_Opacity", opacity);
        renderer.SetPropertyBlock(block);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        return;
        if (collision.CompareTag("Player"))
        {
            Debug.Log("Hewwo");
            reveal();
        }
    }

    void reveal()
    {
       DOTween.To(() => opacity, x => opacity = x, 0, 2);
       mat.SetFloat("Opacity", opacity);
    }
}
