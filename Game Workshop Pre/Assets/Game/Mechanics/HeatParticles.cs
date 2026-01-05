using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeatParticles : MonoBehaviour
{       
    private enum TemperatureStage
    {
        Steaming,
        Sweating,
        Normal,
        Smoking,
        Burning
    }
    [SerializeField] float sweatingThreshold = 30f;
    [SerializeField] float steamingThreshold = 60f;
    [SerializeField] float smokingThreshold = 30f;
    [SerializeField] float burningThreshold = 60f;
    [SerializeField] Color smokeColor;
    [SerializeField] Color steamColor;
    HeatMechanic heat;
    float particleTimer = 0f;

    int temperatureStage = 0;

    void Start()
    {
        heat = GetComponent<HeatMechanic>();
    }

    void FixedUpdate()
    {
        if (heat.coolingOff && heat.Heat >= sweatingThreshold)
        {
            CoolingOff();
        }
        else if (heat.Heat >= smokingThreshold)
        {
            HeatingUp();
        }
        else if(heat.Heat < sweatingThreshold)
        {
            temperatureStage = (int)TemperatureStage.Normal;
        }
    }

    private void HeatingUp()
    {
        if (heat.Heat >= burningThreshold)
        {
            if (temperatureStage < (int)TemperatureStage.Burning)
            {
                ParticleManager.Instance.Play("SmokeBlast", transform.position, null, smokeColor, null, 0.8f);
                ParticleManager.Instance.Play("SmokeBlast", transform.position, null, smokeColor, null, 1.2f);
                temperatureStage = (int)TemperatureStage.Burning;
            }
            if (particleTimer <= 0f)
            {
                ParticleManager.Instance.Play("CinderBeads", transform.position);
            }
        }
        if (temperatureStage < (int)TemperatureStage.Smoking)
        {
            ParticleManager.Instance.Play("SmokeBlast", transform.position, null, smokeColor, null, 0.8f);
            temperatureStage = (int)TemperatureStage.Smoking;
        }
        
        if (particleTimer <= 0f)
        {
            ParticleManager.Instance.Play("RisingSmoke", transform.position, null, smokeColor);
            particleTimer = 8 / heat.Heat;
        }
        particleTimer -= Time.fixedDeltaTime;
    }

    private void CoolingOff()
    {
        if (heat.Heat >= steamingThreshold)
        {
            if (temperatureStage > (int)TemperatureStage.Steaming)
            {
                ParticleManager.Instance.Play("SmokeBlast", transform.position, null, steamColor, null, 0.8f);
                ParticleManager.Instance.Play("SmokeBlast", transform.position, null, steamColor, null, 0.4f);
                temperatureStage = (int)TemperatureStage.Steaming;
            }
            if (particleTimer <= 0f)
            {
                ParticleManager.Instance.Play("RisingSmoke", transform.position, null, steamColor);
            }
        }

        if (temperatureStage > (int)TemperatureStage.Sweating)
        {
            ParticleManager.Instance.Play("SmokeBlast", transform.position, null, steamColor, null, 0.4f);
            temperatureStage = (int)TemperatureStage.Sweating;
        }
        
        if (particleTimer <= 0f)
        {
            ParticleManager.Instance.Play("SweatBeads", transform.position);
            particleTimer = 8 / heat.Heat;
        }
        particleTimer -= Time.fixedDeltaTime;
    }
}