using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shop : MonoBehaviour
{
    
    [SerializeField] GameObject shopUI;
    [SerializeField] public List<ShopItem> shopList;
    
    // Start is called before the first frame update
    void Start()
    {
        foreach (ShopItem item in shopList)
        {
            Instantiate(item,item.spawnPosition,Quaternion.identity);
            
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OpenShop()
    {
        shopUI.SetActive(true);
        Time.timeScale = 0f;
    }
    

    
}
