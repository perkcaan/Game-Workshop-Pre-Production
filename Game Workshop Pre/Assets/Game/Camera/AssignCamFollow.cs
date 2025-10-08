using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssignCamFollow : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera cmc;
    void Start()
    {
        // Upon spawning into a scene, assigns the camera to the player position

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null && cmc == null)
        {
            cmc.Follow = player.transform;
            cmc.LookAt = player.transform;
        }

    }

}
