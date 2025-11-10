using Cinemachine;
using UnityEngine;

public class CamBound : MonoBehaviour
{
    private CameraConfiner camConfiner;
    public PolygonCollider2D poly;

    private void Awake()
    {
        poly = GetComponent<PolygonCollider2D>();
        if (poly == null)
        {
            Debug.LogError("No PolygonCollider2D found on object");
        }
    }

    private void Start()
    {
        camConfiner = FindAnyObjectByType<CameraConfiner>();

        if (camConfiner == null)
        {
            Debug.LogError("No CinemachineConfiner2D found in scene");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<PlayerMovementController>())
        {
            Debug.Log("Player Entered Camera Bound: " + gameObject.name);
            camConfiner.AddBound(this);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent<PlayerMovementController>())
        {
            Debug.Log("Player Entered Camera Bound: " + gameObject.name);
            camConfiner.RemoveBound(this);
        }
    }
}
