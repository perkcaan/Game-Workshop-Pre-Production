using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class TestEventAction : MonoBehaviour
{
    private SpriteRenderer _spriteRenderer;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    [EventAction]
    private void ExampleChangeColor(int i, Color color) {
        Debug.Log(i);
        _spriteRenderer.color = color;
    }
}
