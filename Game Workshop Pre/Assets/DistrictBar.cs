using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DistrictBar : MonoBehaviour
{

    public Slider slider;
    // Start is called before the first frame update
    public void SetClean(float districtCleanliness)
    {
        slider.value = districtCleanliness;
    }
}
