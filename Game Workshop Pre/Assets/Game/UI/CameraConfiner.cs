using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class CameraConfiner : MonoBehaviour
{
    private CinemachineConfiner2D camConfiner;
    private List<CamBound> bounds = new List<CamBound>();
    private void Start()
    {
        camConfiner = GetComponent<CinemachineConfiner2D>();
    }

    public void AddBound(CamBound camBound)
    {
        bounds.Add(camBound);
        camConfiner.m_BoundingShape2D = bounds[0].poly;
        camConfiner.InvalidateCache();
    }
    
    public void RemoveBound(CamBound camBound)
    {
        bounds.Remove(camBound);
        if (bounds.Count > 0)
        {
            camConfiner.m_BoundingShape2D = bounds[0].poly;
            camConfiner.InvalidateCache();
        }
    }
}
