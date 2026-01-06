using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(HeatMechanic))]
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
    [SerializeField] TemperatureStage temperatureStage = TemperatureStage.Normal;

    void Start()
    {
        heat = GetComponent<HeatMechanic>();
    }

    void FixedUpdate()
    {
        if (heat.coolingOff)
        {
            CoolingOff();
        }
        else
        {
            HeatingUp();
        }
    }

    private void HeatingUp()
    {
        if (heat.Heat >= smokingThreshold)
        {
            if (heat.Heat >= burningThreshold)
            {   
                if (temperatureStage < TemperatureStage.Burning)
                {
                    ParticleManager.Instance.Play("SmokeBlast", transform.position, null, smokeColor, null, 0.8f);
                    ParticleManager.Instance.Play("SmokeBlast", transform.position, null, smokeColor, null, 1.2f);
                    temperatureStage = TemperatureStage.Burning;
                }
                if (particleTimer <= 0f)
                {
                    ParticleManager.Instance.Play("CinderBeads", transform.position);
                }
            }
            else if (temperatureStage != TemperatureStage.Smoking)
            {
                ParticleManager.Instance.Play("SmokeBlast", transform.position, null, smokeColor, null, 0.8f);
                temperatureStage = TemperatureStage.Smoking;
            }
            if (particleTimer <= 0f)
            {
                ParticleManager.Instance.Play("RisingSmoke", transform.position, null, smokeColor);
                particleTimer = 8 / heat.Heat;
            }
            particleTimer -= Time.fixedDeltaTime;
        }
        else
        {
            temperatureStage = TemperatureStage.Normal;
        }

    }

    private void CoolingOff()
    {
        if (heat.Heat >= sweatingThreshold)
        {
            if (heat.Heat >= steamingThreshold)
            {
                if (temperatureStage > TemperatureStage.Steaming)
                {
                    ParticleManager.Instance.Play("SmokeBlast", transform.position, null, steamColor, null, 0.4f);
                    ParticleManager.Instance.Play("SmokeBlast", transform.position, null, steamColor, null, 0.6f);
                    temperatureStage = TemperatureStage.Steaming;
                }
                if (particleTimer <= 0f)
                {
                    ParticleManager.Instance.Play("RisingSmoke", transform.position, null, steamColor);
                }
            }
            else if (temperatureStage != TemperatureStage.Sweating)
            {
                ParticleManager.Instance.Play("SmokeBlast", transform.position, null, steamColor, null, 0.4f);
                temperatureStage = TemperatureStage.Sweating;
            }
            if (particleTimer <= 0f)
            {
                ParticleManager.Instance.Play("SweatBeads", transform.position);
                particleTimer = 8 / heat.Heat;
            }
            particleTimer -= Time.fixedDeltaTime;
        }
        else
        {
            temperatureStage = TemperatureStage.Normal;
        }
    }
}