using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopItem : MonoBehaviour
{
    [SerializeField] public int price;
    [SerializeField] public int quantity;
    [SerializeField] TMP_Text itemName;
    [SerializeField] TMP_Text description;
    [SerializeField] SpriteRenderer sprite;
    public Vector3 spawnPosition;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Purchase(int price)
    {
        this.price = price;
        int coins = PlayerPrefs.GetInt("Coins");

        if (coins > price)
        {
            DistrictManager.Instance.RemoveCoins(price);
            gameObject.SetActive(false);

        }
        else
        {
            Debug.Log("YOU'RE TOO POOR");
        }
    }
}
