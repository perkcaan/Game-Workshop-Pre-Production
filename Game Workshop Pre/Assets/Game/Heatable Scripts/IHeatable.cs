using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Interaface version of the Heatable Component, made just in case we find a use/ preffer it over the straight
//component implimentation

public interface IHeatable
{

    //Current heat level
    int heatValue { get; set; }

    //Heat level at which adverse effects happen
    int heatThreshold { get; set; }

    //Natrual fall rate of heat level
    int heatEntropy { get; set; }


    void RaiseHeat(int heatValue);

    void LowerHeat(int heatValue);

    //Coroutine for dissapaiting heat from actor
    IEnumerator HeatEntropy();
}
