//Had to make this temporary version to be able to make a Prefab for the UI, as for
//Some reason, both the CleanBar and District Clean bar had versions that weren't
//inherting from monobehavior, even thought they very obviously were...
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CleanBarTemp : MonoBehaviour
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
            _cleanText.text = $"{fillAmount * 100f:0}% clean";
        }
        else
        {
            _cleanBar.SetActive(false);
        }
        
    }
}
