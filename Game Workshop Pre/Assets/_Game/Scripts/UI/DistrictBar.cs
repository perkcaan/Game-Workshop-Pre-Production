using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class DistrictBar : MonoBehaviour
{
    [SerializeField] private GameObject _districtBar;
    [SerializeField] private Image _fillImage;
    [SerializeField] private TMP_Text _districtText;


    private void Start()
    {
        if (_districtBar == null || _fillImage == null || _districtText == null)
        {
            Debug.Log("Please set up District Bar UI");
            enabled = false;
        }
    }

    private void OnEnable()
    {
        float fillAmount = DistrictManager.Instance?.GetCleanCompletion() ?? 1f;
        _fillImage.fillAmount = fillAmount;
        _districtText.text = $"District: {fillAmount * 100f:0}%";
    }
}
