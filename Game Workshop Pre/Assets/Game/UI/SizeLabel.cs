using System.Collections;
using System.Collections.Generic;
using TMPro;
using TMPro.EditorUtilities;
using UnityEditor;
using UnityEngine;

public class SizeLabel : MonoBehaviour
{
    [SerializeField] private TMP_Text _text;
    private bool _startVisible;

    private void Awake()
    {
        if (!_startVisible) Hide();
    }

    public void UpdateSizeLabel(int size)
    {
        string text = size.ToString();
        _text.text = text;
    }

    public void SetColor(Color color)
    {
        _text.color = color;
    }

    public void Hide()
    {
        _text.enabled = false;
    }

    public void Show()
    {
        _text.enabled = true;
    }
}
