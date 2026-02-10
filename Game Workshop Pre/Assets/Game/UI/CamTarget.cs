using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamTarget : MonoBehaviour
{
    public Transform player; 
    public float positionInfluence = 0.2f; // Cam leaning distance
    public float maxOffset = 1.5f; // Max distance the camera can stray

    private Camera mainCam;

    void Start()
    {
        mainCam = Camera.main;
    }

    void LateUpdate()
    {
        // Get mouse position in world space
        Vector3 mouseWorld = mainCam.ScreenToWorldPoint(Input.mousePosition);
        mouseWorld.z = 0f;

        // Get the offset vector from player to mouse
        Vector3 playerToMouse = mouseWorld - player.position;
        Vector3 finalOffset = playerToMouse * positionInfluence;

        // Calmp the offset to prevent the cam from moving too far
        finalOffset = Vector3.ClampMagnitude(finalOffset, maxOffset);

        // Set object pos
        transform.position = player.position + finalOffset;
    }
}