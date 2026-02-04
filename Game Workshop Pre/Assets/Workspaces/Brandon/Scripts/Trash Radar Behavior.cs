using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrashRadar : MonoBehaviour
{

    private GameObject[] _gameObjectArray;

    void Awake()
    {
        _gameObjectArray = Resources.FindObjectsOfTypeAll<GameObject>();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
