using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class RadiusTrigger : MonoBehaviour
{
    private PlayerControls controls;
    private DumpsterObject parentComponent;
    private bool playerInVicinity = false;

    private void Awake()
    {
        controls = new PlayerControls();
        parentComponent = GetComponentInParent<DumpsterObject>();
    }

    private void OnEnable()
    {
        controls.Enable();
        // Optional: Listen to the input action event directly
        controls.Default.InteractionInput.performed += OnInteractPerformed;
    }

    private void OnDisable()
    {
        // Unsubscribe from the event to prevent memory leaks
        controls.Default.InteractionInput.performed -= OnInteractPerformed;
        controls.Disable();
    }

    private void OnInteractPerformed(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        // Only interact if player is in vicinity and dumpster isn't already opened
        if (playerInVicinity && !parentComponent.GetIsOpened())
        {
            Debug.Log("Dumpster opened via Input Event!");
            parentComponent.OnInteract();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<PlayerMovementController>(out _))
        {
            playerInVicinity = true;
            Debug.Log("Player entered vicinity");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            playerInVicinity = false;
            Debug.Log("Player exited vicinity");
        }
    }
}
