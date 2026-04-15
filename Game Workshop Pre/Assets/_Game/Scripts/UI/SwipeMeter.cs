using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SwipeMeter : MonoBehaviour
{
    [SerializeField] private GameObject swipeMeter;
    [SerializeField] private Image fillImage;
    [SerializeField] private Slider slider;
  

    public void SetFill(float value)
    {
        /*if (fillImage == null)
        {
            Debug.LogWarning("SwipeMeter: No fill image assigned!");
            return;
        }

        fillImage.fillAmount = Mathf.Clamp01(value); */ //not using fill image yet

        if (slider == null)
        {
            Debug.LogWarning("No slider assigned");
            return;
        }

        slider.value = Mathf.Clamp01(value);
    }

    public void Show(bool visible)
    {
        gameObject.SetActive(visible);
    }

}
