using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopItem : MonoBehaviour
{
    [SerializeField] public int price;
    [SerializeField] public int quantity;
    [SerializeField] TMP_Text priceText;
    [SerializeField] TMP_Text quantityText;
    [SerializeField] TMP_Text itemName;
    [SerializeField] TMP_Text description;
    [SerializeField] SpriteRenderer sprite;
    [SerializeField] GameObject descriptionBox;
    [SerializeField] Item attachedItem;
    public Vector3 spawnPosition;
    public Collider2D triggerCollider;
    private bool inTrigger;
    // Start is called before the first frame update
    void Start()
    {
        descriptionBox.gameObject.SetActive(false);
        gameObject.name = itemName.text;
        CollectableItem collectableItem = GetComponent<CollectableItem>();
        collectableItem.isCollected = true;
    }

    // Update is called once per frame
    void Update()
    {
        priceText.text = price + " Coins";
        quantityText.text = quantity + " x" ;

        if (Input.GetKeyDown(KeyCode.E) && inTrigger)
        {
            Purchase(price);
            

        }

        //transform.DOMoveY(0, 0.2f).SetLoops(1, LoopType.Restart).SetEase(Ease.InOutSine);
        float newY = spawnPosition.y + Mathf.Sin(Time.time * 1f) * 0.2f;

        // Update the object's position
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }

    public void Purchase(int price)
    {
        this.price = price;
        int coins = PlayerPrefs.GetInt("Coins");

        if (coins > price)
        {
            DistrictManager.Instance.RemoveCoins(price);
            quantity--;

            if (quantity <= 0)
            {
                gameObject.SetActive(false);
                Inventory.Instance.StoreItem(attachedItem);
            }

        }
        else
        {
            Debug.Log("YOU'RE TOO POOR");
        }
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        triggerCollider = collision;
        if (collision.TryGetComponent(out PlayerMovementController player))
        {
            descriptionBox.gameObject.SetActive(true);
            inTrigger = true;

        }
    }

    

    public void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            descriptionBox.gameObject.SetActive(false);
            inTrigger = false;
        }
    }
}
