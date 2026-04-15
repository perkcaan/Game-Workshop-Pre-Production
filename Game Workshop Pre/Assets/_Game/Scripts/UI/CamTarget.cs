using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

public class CamTarget : MonoBehaviour
{
    public Transform Player; 

    [SerializeField] private float _camLeanSensitivity = 0.2f; // Sensitivity to mouse
    [SerializeField] private float _maxCamOffset = 1.5f; // Max distance the cam can lean

    // Getter and setter for camLean
    public float CamLean
    {
        get { return _camLeanSensitivity; }
        set { _camLeanSensitivity = value; }
    }

    // Getter and setter for maxCamOffset
    public float MaxCamOffset
    {
        get { return _maxCamOffset; }
        set { _maxCamOffset = value; }
    }

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
        Vector3 playerToMouse = mouseWorld - Player.position;
        Vector3 finalOffset = playerToMouse * _camLeanSensitivity;

        // Clamp the offset to prevent the cam from moving too far
        finalOffset = Vector3.ClampMagnitude(finalOffset, _maxCamOffset);

        // Set cam target position
        transform.position = Player.position + finalOffset;
    }
}