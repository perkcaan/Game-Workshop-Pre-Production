using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorManager : MonoBehaviour
{

    private Camera mainCam;

    [SerializeField] private Sprite baseSprite; // Base cursor sprite
    [SerializeField] private Sprite overTrashSprite; // Cursor sprite when hovering over trash

    private SpriteRenderer childSpriteRenderer; // Reference to cursor sprite


    void Awake()
    {
        mainCam = Camera.main;
        Cursor.visible = false; // Hide normal mouse cursor

        childSpriteRenderer = GetComponentInChildren<SpriteRenderer>();
        childSpriteRenderer.sprite = baseSprite;
    }

    void Update()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = 10f;
        transform.position = mainCam.ScreenToWorldPoint(mousePos);

        
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        // Swap to a new cursor when over trash
        if (collision.gameObject.GetComponent<Trash>())
        {
            childSpriteRenderer.sprite = overTrashSprite;
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        // Swap back to base cursor 
        if (collision.gameObject.GetComponent<Trash>())
        {
            childSpriteRenderer.sprite = baseSprite;
        }
    }
}
