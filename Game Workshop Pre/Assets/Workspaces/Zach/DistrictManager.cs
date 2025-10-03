using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistrictManager : StaticInstance<DistrictManager>
{
    [SerializeField, Range(HeatMechanic.LOWEST_HEAT_VALUE, HeatMechanic.HIGHEST_HEAT_VALUE)] private int _baseTemperature;
    public int Temperature { get { return _baseTemperature; } } 
}
