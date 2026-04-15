using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CleanBar : MonoBehaviour
{
    [SerializeField] private GameObject _cleanBar;
    [SerializeField] private Image _fillImage;
    [SerializeField] private TMP_Text _cleanText;

    private void Start()
    {
        if (_cleanBar == null || _fillImage == null || _cleanText == null)
        {
            Debug.Log("Please set up Clean Bar UI");
            enabled = false;
        }
    }

    private void Update()
    {
        float fillAmount = DistrictManager.Instance?.FocusedRoom?.Cleanliness ?? 1f;
        if (fillAmount < 1f)
        {
            _cleanBar.SetActive(true);
            _fillImage.fillAmount = fillAmount;
            int percent = Mathf.FloorToInt(fillAmount * 100f);
            _cleanText.text = $"{percent}% clean";
        }
        else
        {
            _cleanBar.SetActive(false);
        }
        
    }
}
