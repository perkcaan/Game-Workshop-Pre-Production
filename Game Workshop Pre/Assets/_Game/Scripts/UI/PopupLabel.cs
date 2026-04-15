using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection.Emit;
using DG.Tweening;
using TMPro;
using UnityEngine;
using Color = UnityEngine.Color;

public class PopupLabel : MonoBehaviour
{

    [SerializeField] private TMP_Text _text;
    [SerializeField] private SpriteRenderer _imageLabel;
    [SerializeField] private float _minScaleSize = 1.2f;
    [SerializeField] private float _maxScaleSize = 3f;
    [SerializeField] private int _sizeOfMaxScale = 40;
    [SerializeField] private int _coinsOfMaxScale = 40;
    [SerializeField] private float _scaleUpTime = 0.2f;
    [SerializeField] private float _scaleDownSize = 0.8f;
    [SerializeField] private float _scaleDownTime = 0.3f;
    [SerializeField] private float _randomOffsetDistance = 0.1f;
    [SerializeField] private float _floatUpDistance = 0.5f;
    [SerializeField] private float _floatUpDuration = 0.6f;
    [SerializeField] private float _fadeAwayDelay = 0.3f;
    [SerializeField] private float _fadeAwayDuration = 0.3f;
    private Sequence _sequence;

    
    public static void CreatePlusLabel(Vector2 position, Color color, int size)
    {
        PopupLabel newLabel = null;
        PopupLabelPooler plp = PopupLabelPooler.Instance;
        if (plp == null)
        {
            Debug.LogWarning("Add a PopupLabelPooler prefab to the scene.");
            return;
        }
        newLabel = plp.GetLabel();
        newLabel.PlusSetup(position, color, size);
    }

    public static void CreateCoinLabel(Vector2 position, Color color, int coins)
    {   
        PopupLabel newLabel = null;
        PopupLabelPooler plp = PopupLabelPooler.Instance;
        if (plp == null)
        {
            Debug.LogWarning("Add a PopupLabelPooler prefab to the scene.");
            return;
        }
        newLabel = plp.GetLabel();
        newLabel.CoinSetup(position, color, coins);
    }

    public void PlusSetup(Vector2 position, Color color, int size)
    {
        string labelText = "+" + size;
        float labelScale = _minScaleSize + (size - 1f) / (_sizeOfMaxScale - 1f) * (_maxScaleSize - _minScaleSize);
        labelScale = Mathf.Clamp(labelScale, _minScaleSize, _maxScaleSize);

        gameObject.name = "Popup Label: " + labelText;
        transform.position = position;
        string text = labelText;
        _text.text = text;
        _text.color = color;
        transform.localScale = Vector3.one;
        _text.alpha = 1f;
        Vector3 offset = Random.insideUnitCircle * _randomOffsetDistance;
        _imageLabel.enabled = false;
        transform.position += offset;
        PlayPlusAnimation(labelScale);
    }

    public void CoinSetup(Vector2 position, Color color, int coins)
    {

        string labelText = $"{coins}";
        float labelScale = _minScaleSize + (coins - 1f) / (_coinsOfMaxScale - 1f) * (_maxScaleSize - _minScaleSize);
        labelScale = Mathf.Clamp(labelScale, _minScaleSize, _maxScaleSize);

        gameObject.name = "Coin Label: " + labelText;
        transform.position = position;
        string text = labelText;
        _text.text = text;
        _text.color = color;
        transform.localScale = Vector3.one;
        _text.alpha = 1f;
        Vector3 offset = Random.insideUnitCircle * _randomOffsetDistance;
        _imageLabel.enabled = true;
        transform.position += offset;
        UpdateImagePosition();
        PlayPlusAnimation(labelScale);
    }

    private void UpdateImagePosition()
    {
       _text.ForceMeshUpdate();

        TMP_TextInfo info = _text.textInfo;
        if (info.lineCount == 0)
            return;

        TMP_LineInfo line = info.lineInfo[info.lineCount - 1];

        float rightEdge = line.lineExtents.max.x;

        float midY = (line.lineExtents.min.y + line.lineExtents.max.y) * 0.5f;

        float spacing = 1f / 16;

        // Get sprite half width in local units
        float halfWidth = _imageLabel.sprite.bounds.extents.x;

        Vector3 localPos = new Vector3(
            rightEdge + halfWidth - spacing,
            midY,
            0f
        );

        _imageLabel.transform.localPosition = localPos;
    }

    private void PlayPlusAnimation(float labelScale)
    {   
        
        if (_sequence != null && _sequence.IsActive())
        {
            Debug.Log("Have to kill you. Sorry.");
            _sequence.Kill();
        }

        _sequence = DOTween.Sequence()
            .SetLink(gameObject, LinkBehaviour.KillOnDisable);

        // Scale up
        _sequence.Append(
            transform.DOScale(labelScale, _scaleUpTime)
                .SetEase(Ease.OutBack)
        );

        // Scale back down
        _sequence.Append(
            transform.DOScale(_scaleDownSize, _scaleDownTime)
            .SetEase(Ease.OutQuad)
        );

        // Fade after delay
        _sequence.Append(
            _text.DOFade(0f, _fadeAwayDuration)
            .SetDelay(_fadeAwayDelay)
            .OnComplete(() => TweenComplete())
        );

        // Float up
        _sequence.Join(
            transform.DOMoveY(transform.position.y + _floatUpDistance, Mathf.Min(_floatUpDuration, _fadeAwayDelay + _fadeAwayDelay))
            .SetEase(Ease.OutQuad)
        );
    }
    
    private void TweenComplete()
    {
        PopupLabelPooler plp = PopupLabelPooler.Instance;
        if (plp != null)
        {
            _sequence?.Kill();
            _sequence = null;
            plp.ReturnLabel(this);
            
        }
    }
}
