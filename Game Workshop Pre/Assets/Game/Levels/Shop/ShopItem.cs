using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopItem : MonoBehaviour
{
    // Item Fields
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

    public float shakeDuration = 0.5f;
    public float shakeMagnitude = 0.1f;


    // Amount Tracking and Trigger Checks
    private bool inTrigger;
    private int initialQuantity;
    private bool isPurchased;


    // Start is called before the first frame update
    void Start()
    {
        descriptionBox.gameObject.SetActive(false);
        gameObject.name = itemName.text;

        if (attachedItem != null)
        {
            attachedItem.displayName = itemName.text;
            description.text = attachedItem.discriptionText;
            sprite.sprite = attachedItem.displayIcon;
            CollectableItem collectableItem = GetComponent<CollectableItem>();
            collectableItem.shopItem = true;
        }
        initialQuantity = quantity;

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



        float newY = spawnPosition.y + Mathf.Sin(Time.time * 1f) * 0.2f;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }

    public void Purchase(int price)
    {
        this.price = price;
        int coins = PlayerPrefs.GetInt("Coins");
        

        if (coins >= price)
        {
            DistrictManager.Instance.RemoveCoins(price);
            quantity--;
            

            if (quantity <= 0)
            {
                gameObject.SetActive(false);
            }

            if (!isPurchased)
            {
                Inventory.Instance.StoreItem(attachedItem);

               
            }

            isPurchased = true;
            attachedItem.displayName = itemName.text + $"({Mathf.Abs(quantity - initialQuantity)} x )";

            

        }
        else
        {
            TriggerShake();
            Debug.Log("YOU'RE TOO POOR");
        }
    }

    public void TriggerShake()
    {
        // Store the original position before shaking
        spawnPosition = transform.localPosition;
        StartCoroutine(Shake());
    }
     
    private IEnumerator Shake()
    {
        float elapsed = 0.0f;
        

        while (elapsed < shakeDuration/3)
        {
            Vector3 randomOffset = Random.insideUnitCircle * shakeMagnitude;
            transform.localPosition = spawnPosition + randomOffset;
            elapsed += Time.deltaTime;
            yield return null; 
        }

        while (elapsed < shakeDuration / 2)
        {
            Vector3 randomOffset = Random.insideUnitCircle * shakeMagnitude;
            transform.localPosition = spawnPosition + (randomOffset * 0.6f);
            elapsed += Time.deltaTime;
            yield return null;
        }


        transform.localPosition = spawnPosition;
    }

public void OnTriggerEnter2D(Collider2D collision)
    {
        
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
