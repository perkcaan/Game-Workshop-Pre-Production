using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CleanBar : MonoBehaviour
{

    public Slider slider;
    // Start is called before the first frame update
    public void SetClean(float cleanliness)
    {
        slider.value = cleanliness;
    }
}
