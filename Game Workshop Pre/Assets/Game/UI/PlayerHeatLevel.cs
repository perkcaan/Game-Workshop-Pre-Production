using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHeatLevel : MonoBehaviour
{

    [SerializeField] GameObject playerController;
    [SerializeField] Image heatProgressBar;


    private Heatable heatable;
    private float heatProgress;
    


    // Start is called before the first frame update
    void Start()
    {
        heatable = playerController.transform.Find("HeatRadius").GetComponent<Heatable>();
        heatProgress = (float)heatable.heatLevel / (float)heatable.heatThreshold;
        heatProgressBar.fillAmount = heatProgress;
    }

    // Update is called once per frame
    void Update()
    {
        heatProgress = (float)heatable.heatLevel / (float)heatable.heatThreshold;
        heatProgressBar.fillAmount = heatProgress;
    }
}
