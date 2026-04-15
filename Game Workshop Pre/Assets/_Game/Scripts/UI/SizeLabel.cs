using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SizeLabel : MonoBehaviour
{
    [SerializeField] private TMP_Text _text;
    private bool _startVisible;
    private bool _shown = true;
    private bool _enabled = true;

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
        _shown = false;
        UpdateVisibility();
    }

    public void Show()
    {
        _shown = true;
        UpdateVisibility();
    }

    private void UpdateVisibility()
    {
        _text.enabled = _shown && _enabled;
    }

    //Settings
    private void OnEnable()
    {
        if (GameManager.Instance == null) return;
        GameManager.Instance.SettingsChanged += OnSettingsChanged;
        OnSettingsChanged();
    }

    private void OnDisable()
    {
        if (GameManager.Instance == null) return;
        GameManager.Instance.SettingsChanged -= OnSettingsChanged;
    }
    
    private void OnSettingsChanged()
    {
        if (GameManager.Instance == null) return;
        _enabled = GameManager.Instance.UseTrashballLabels;
        UpdateVisibility();
    }



}
