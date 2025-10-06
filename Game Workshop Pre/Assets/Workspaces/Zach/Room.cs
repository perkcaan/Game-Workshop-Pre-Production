using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class Room : MonoBehaviour
{
    [SerializeField, Range(HeatMechanic.LOWEST_HEAT_VALUE, HeatMechanic.HIGHEST_HEAT_VALUE)] private int _baseTemperature;
    public int Temperature { get { return _baseTemperature; } } // Return _baseTemperature + anything that modifies room temperature


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent(out HeatMechanic heatable))
        {
            heatable.EnterRoom(this);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent(out HeatMechanic heatable))
        {
            heatable.ExitRoom(this);
        }
    }

}
