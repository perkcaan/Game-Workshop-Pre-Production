using System.Collections;
using System.Collections.Generic;
using TMPro;
using TMPro.EditorUtilities;
using UnityEditor;
using UnityEngine;

public class SizeLabel : MonoBehaviour
{
    private TMP_Text _text;
    private bool _startVisible;

    private void Awake()
    {
        _text = GetComponent<TMP_Text>();
        if (!_startVisible) Hide();
    }

    public void UpdateSizeLabel(int size)
    {
        _text.text = size.ToString();
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
