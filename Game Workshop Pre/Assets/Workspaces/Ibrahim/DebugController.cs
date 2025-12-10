using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class DebugController : MonoBehaviour
{
    [SerializeField] PauseMenu pauseMenu;
    [SerializeField] PlayerMovementController playerController;
    [SerializeField] CinemachineVirtualCamera vcam;
    [SerializeField] GameObject debugMenuUI;
    private CinemachineConfiner2D confiner;
    private CinemachineFramingTransposer transposer;
    private bool cheatsEnabled = false;
    void Start()
    {
        confiner = vcam.GetComponent<CinemachineConfiner2D>();
        transposer = vcam.GetCinemachineComponent<CinemachineFramingTransposer>();
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            cheatsEnabled = true;
            debugMenuUI.SetActive(!debugMenuUI.activeSelf);
        }
        if (!cheatsEnabled) return;
        if (Input.GetKeyDown(KeyCode.Z))
        {
            pauseMenu.gameObject.SetActive(!pauseMenu.isActiveAndEnabled);
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            playerController.canDash = true;
            playerController.canSweep = true;
            playerController.canSwipe = true;
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            confiner.enabled = !confiner.enabled;
        }

        Vector3 offsetChange = Vector3.zero;

        if (Input.GetKey(KeyCode.J)) offsetChange.x -= 3f;
        if (Input.GetKey(KeyCode.L)) offsetChange.x += 3f;

        float verticalInput = 0f;
        if (Input.GetKey(KeyCode.I)) verticalInput = 3f;
        if (Input.GetKey(KeyCode.K)) verticalInput = -3f;

        offsetChange.y = verticalInput;
        transposer.m_TrackedObjectOffset += offsetChange * Time.deltaTime;

        float zoomChange = 0f;
        if (Input.GetKey(KeyCode.V)) zoomChange = -1f;
        if (Input.GetKey(KeyCode.B)) zoomChange = 1f;

        float newSize = vcam.m_Lens.OrthographicSize + (zoomChange * Time.deltaTime);
        vcam.m_Lens.OrthographicSize = Mathf.Clamp(newSize, 2, 16);
    }
}
