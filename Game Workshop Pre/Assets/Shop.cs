using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shop : MonoBehaviour
{
    public int testPrice;
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
        int coins = PlayerPrefs.GetInt("Coins");

        if (coins > price)
        {
            DistrictManager.Instance.RemoveCoins(price);

        }
        else
        {
            Debug.Log("YOU'RE TOO POOR");
        }
    }

    public void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.GetComponent<PlayerMovementController>() != null)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                Purchase(testPrice);
            }
        }
    }
}
