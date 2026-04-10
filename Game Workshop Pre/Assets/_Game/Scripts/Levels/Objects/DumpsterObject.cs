using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using DG.Tweening;
using UnityEngine;

public class DumpsterObject : MonoBehaviour, IRadius
{

    // Refs
    [SerializeField] private CollectableItem storedItem;
    [SerializeField] private GameObject player;
    [SerializeField] private Inventory playerInventory;

    // Rummaging
    [SerializeField] private int rummageMax = 3;
    private int rummageCount = 0;
    private bool isOpened = false;
    private PlayerControls controls;

    void Awake()
    {
        controls = new PlayerControls();
        isOpened = false;

        player = FindAnyObjectByType<PlayerMovementController>().gameObject;

    }

    void OnEnable()
    {
        controls.Enable();
    }

    void OnDisable()
    {
        controls.Disable();
    }

    public bool GetIsOpened()
    {
        return isOpened;
    }

    public void SetIsOpened(bool b)
    {
        isOpened = b;
    }

    public void OnInteract()
    {
        Debug.Log("OnInteract Activated");

        // Open dumpster
        isOpened = true;

        // Disable player components 
        if (player.TryGetComponent<PlayerMovementController>(out PlayerMovementController playerMC))
        playerMC.enabled = false;

        if (player.TryGetComponent<Collider2D>(out Collider2D playerCollider))
        playerCollider.enabled = false;

        // Move player into dumpster
        Sequence jumpTween = DOTween.Sequence();
        jumpTween .Append(
            player.transform.DOLocalMoveY(2.5f, .6f)
                .SetEase(Ease.InOutSine)
        );
        jumpTween.Append(
            player.transform.DOLocalMoveY(1f, 0.3f)
                .SetEase(Ease.InOutSine)
        );

        // Begin Rummaging
        StartCoroutine(RummageCoroutine(playerMC, playerCollider));
    }

    private IEnumerator RummageCoroutine(
    PlayerMovementController playerMC, 
    Collider2D playerCollider)
    {
        Debug.Log("Rummage Start");
        
        yield return new WaitForSeconds(1.1f);

        // Disable player sprite
        SpriteRenderer playerSR = player.GetComponentInChildren<SpriteRenderer>();
        if (playerSR != null) playerSR.enabled = false;

        while (rummageCount < rummageMax)
        {
            
            if (controls.Default.EscapeTrashBallInput.triggered)
            {
                rummageCount++;
                transform.DOShakePosition(0.2f, 0.2f);
                transform.DOShakePosition(0.2f, 0.2f);
                Debug.Log($"Rummage {rummageCount}/{rummageMax}");
            }
            yield return null;

        }

        Debug.Log("Rummage End");

        // Restore player sprite
        if (playerSR != null) playerSR.enabled = true;

        // Return player to initial position
        Sequence returnJumpTween = DOTween.Sequence();
        returnJumpTween .Append(
            player.transform.DOLocalMoveY(-1f, .6f)
                .SetEase(Ease.InOutSine)
        );

        yield return new WaitForSeconds(2.0f);

        // Spawn the stored item image above the player
        if (storedItem != null)
        {
            Debug.Log("Spawning Item");

            CollectableItem tempItemInstance =
            Instantiate(storedItem, player.transform.position + Vector3.up * 1f, Quaternion.identity);

            yield return new WaitForSeconds(1.5f);

            // Add item to inventory
            playerInventory.StoreItem(tempItemInstance.item);
            Destroy(tempItemInstance.gameObject);

    
        }
        
        // Return player functionality
        if (playerMC != null) playerMC.enabled = true;
        if (playerCollider != null) playerCollider.enabled = true;

    }

}
