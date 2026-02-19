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
    [SerializeField] private TextMeshProUGUI _coinText;
    [SerializeField] private float _minScaleSize = 1.2f;
    [SerializeField] private float _maxScaleSize = 3f;
    [SerializeField] private int _sizeOfMaxScale = 40;
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
        newLabel.Setup(position, color, size);
    }

    public static void CreateCoinLabel(Color color, int coins)
    {
        
        PopupLabel newLabel = null;
        PopupLabelPooler plp = PopupLabelPooler.Instance;
        if (plp == null)
        {
            Debug.LogWarning("Add a PopupLabelPooler prefab to the scene.");
            return;
        }
        newLabel = plp.GetCoinLabel();

        newLabel.CoinSetup(color, coins);


    }

    public void Setup(Vector2 position, Color color, int size)
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
        transform.position += offset;
        PlayPlusAnimation(labelScale);
    }

    public void CoinSetup(Color color, int coins)
    {
        coins = PlayerPrefs.GetInt("Coins");
        string labelText = $"Coins: {coins}";
        float labelScale = 1;
        string text = labelText;

        Canvas parent = FindFirstObjectByType<Canvas>();
        if (parent != null)
        {
            transform.SetParent(parent.transform, false);
        }


        RectTransform rectTransform = GetComponent<RectTransform>();
        if (rectTransform == null)
            rectTransform = gameObject.AddComponent<RectTransform>();

        


        rectTransform.anchoredPosition = new Vector2(-950f, 354.3f);

        gameObject.name = "Coin Label: " + labelText;
        _coinText.text = text;
        _coinText.color = color;
        _coinText.alpha = 1f;

        PlayPlusAnimation(labelScale);
    }



    private void PlayPlusAnimation(float labelScale)
    {
        if (_sequence != null && _sequence.IsActive())
        {
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
        TMP_Text targetText = _coinText != null && _coinText.text != "" ? _coinText : _text;
        if (targetText != null)
        {
            _sequence.Append(
                targetText.DOFade(0f, _fadeAwayDuration)
                .SetDelay(_fadeAwayDelay));
        }

        
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
