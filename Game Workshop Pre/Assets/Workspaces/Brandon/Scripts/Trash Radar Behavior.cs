using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrashRadar : MonoBehaviour
{

    private TrashRadar _instance;
    public TrashRadar Instance
    {
        get { return _instance; }
        private set { _instance = value; }
    }

    public BHeap<GameObject> sweepableObjects;

    private Vector2 _location;
    public Vector2 Location
    {
        get {  return _location; }
        set { _location = value; }
    }

    private GameObject _pointer;
    public GameObject Pointer { 
        get { return _pointer; }
        private set { _pointer = value; }
    }

    void Start()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
        Pointer = GameObject.Find("Pointer");
        Location = gameObject.transform.position;

        Instance = this;
        sweepableObjects = new BHeap<GameObject>();
        MonoBehaviour[] sweepableList = FindObjectsOfType<MonoBehaviour>();

        for (int i = 0; i <  sweepableList.Length; i++)
        {
            if (sweepableList[i] as ICleanable is ICleanable)
            {
                Vector2 sweepableLocation = (Vector2)sweepableList[i].transform.position;
                float distance = Vector2.Distance(sweepableLocation, Location);
                sweepableObjects.Enqueue(sweepableList[i].gameObject, distance);
            }
        }
        Debug.Log(sweepableObjects.Count);
        Debug.Log(sweepableList[0]);
        Debug.Log(sweepableList[1]);
        Debug.Log(sweepableList[2]);
    }

    // Update is called once per frame
    void Update()
    {
        Location = gameObject.transform.position;
        GameObject nearestItem = sweepableObjects.Dequeue();
        float distance = Vector3.Distance((Vector3)Location, nearestItem.transform.position);
        Vector3 direction = (nearestItem.transform.position - gameObject.transform.position);
        float pointerAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        gameObject.transform.rotation = Quaternion.Euler(0.0f,0.0f, pointerAngle - 90);
        sweepableObjects.Enqueue(nearestItem, distance);
    }

}
