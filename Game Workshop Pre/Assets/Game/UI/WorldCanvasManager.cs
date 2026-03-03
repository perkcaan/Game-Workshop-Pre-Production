using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldCanvasManager : Singleton<WorldCanvasManager>
{
    [SerializeField] private GameObject _bubbleDialoguePrefab;
    public GameObject BubbleDialoguePrefab
    {
        get { return _bubbleDialoguePrefab; }
    }
}
