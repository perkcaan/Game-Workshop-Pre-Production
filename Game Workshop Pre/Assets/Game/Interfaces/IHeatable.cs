using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Interaface version of the Heatable Component, made just in case we find a use/ preffer it over the straight
//component implimentation

public interface IHeatable
{
    public void OnIgnite(HeatMechanic heat);
}


