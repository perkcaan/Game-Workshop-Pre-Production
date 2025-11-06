using Cinemachine;
using UnityEngine;

public class CamBound : MonoBehaviour
{
    private CinemachineConfiner2D camConfiner;
    private PolygonCollider2D poly;

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
        camConfiner = FindAnyObjectByType<CinemachineConfiner2D>();

        if (camConfiner == null)
        {
            Debug.LogError("No CinemachineConfiner2D found in scene");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("Player Entered Camera Bound: " + gameObject.name);

            camConfiner.m_BoundingShape2D = poly;
            camConfiner.InvalidateCache();
        }
    }
}
