using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraCanvasManager : Singleton<CameraCanvasManager>
{
    [SerializeField] private GameObject _bubbleDialoguePrefab;
    public GameObject BubbleDialoguePrefab
    {
        get { return _bubbleDialoguePrefab; }
    }
}
