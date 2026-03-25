using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.Animations;
using DG.Tweening;
using Unity.AI.Navigation;

public class RadiusTrigger : MonoBehaviour
{
    PlayerControls controls;
    private DumpsterObject parentComponent;
    private bool playerInVicinity = false;

    void Awake()
    {
        controls = new PlayerControls();
        parentComponent = transform.parent.GetComponent<DumpsterObject>();
    }

    void OnEnable()
    {
        controls.Enable();
    }

    void OnDisable()
    {
        controls.Disable();
    }

    void Update()
    {
        // If the player is in the vicinity, pressing 'E' will open the Dumpster
        if (playerInVicinity && controls.Default.InteractionInput.WasPerformedThisFrame())
        {
            if (!parentComponent.isOpened)
            {
                parentComponent.isOpened = true;
                parentComponent.DumpsterOpened();
                parentComponent.transform.DOShakePosition(0.2f, 0.2f);
            }
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            playerInVicinity = true;
            parentComponent.InPlayerVicinity();
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            playerInVicinity = false;
        }
    }
}