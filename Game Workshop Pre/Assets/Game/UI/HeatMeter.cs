using UnityEngine;
using UnityEngine.UI;

public class HeatMeter : MonoBehaviour
{
    [SerializeField] private HeatMechanic _targetHeatMechanic;
    [SerializeField] private Image _fillImage;

    [Header("Colors")]
    [SerializeField] private Gradient _heatGradient;


    private void Start()
    {
        if (_targetHeatMechanic == null || _fillImage == null)
        {
            Debug.Log("Please set up Heat Meter UI");
            enabled = false;
        }
    }

    private void Update()
    {
        float heatProgress = (float) (_targetHeatMechanic.Heat - HeatMechanic.LOWEST_HEAT_VALUE) / (float) (HeatMechanic.HIGHEST_HEAT_VALUE - HeatMechanic.LOWEST_HEAT_VALUE);
        _fillImage.color = _heatGradient.Evaluate(heatProgress);
        _fillImage.fillAmount = heatProgress;
    }
}
