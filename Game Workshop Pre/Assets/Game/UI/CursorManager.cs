using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorManager : MonoBehaviour
{
    private Camera _mainCam;

    [SerializeField] private Sprite _baseSprite; // Reference to base cursor sprute
    [SerializeField] private Sprite _overTrashSprite; // Reference to sprite when cursor is over trash

    private SpriteRenderer _childSpriteRenderer;

    // Getter and setter for BaseSprite
    public Sprite BaseSprite
    {
        get { return _baseSprite; }
        set
        {
            _baseSprite = value;

            // If currently using base sprite, update sprite
            if (_childSpriteRenderer != null && _childSpriteRenderer.sprite != _overTrashSprite)
            {
                _childSpriteRenderer.sprite = _baseSprite;
            }

        }
    }

    // Getter and setter for OverTrashSprite
    public Sprite OverTrashSprite
    {
        get { return _overTrashSprite; }
        set
        {
            _overTrashSprite = value;

            // If currently hovering trash, update sprite
            if (_childSpriteRenderer != null && _childSpriteRenderer.sprite != _baseSprite)
            {
                _childSpriteRenderer.sprite = _overTrashSprite;
            }

        }
    }


    void Awake()
    {
        _mainCam = Camera.main;
        Cursor.visible = false; // Hide normal mouse cursor

        _childSpriteRenderer = GetComponentInChildren<SpriteRenderer>();
        _childSpriteRenderer.sprite = _baseSprite;
    }

    void Update()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = 10f;
        transform.position = _mainCam.ScreenToWorldPoint(mousePos);

        
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        // Swap to a new cursor when over trash
        if (collision.gameObject.GetComponent<Trash>())
        {
            _childSpriteRenderer.sprite = _overTrashSprite;
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        // Swap back to base cursor 
        if (collision.gameObject.GetComponent<Trash>())
        {
            _childSpriteRenderer.sprite = _baseSprite;
        }
    }
}
