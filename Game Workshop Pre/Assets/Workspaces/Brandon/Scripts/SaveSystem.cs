using UnityEngine;
using System;
using System.IO;
using System.Collections.Generic;

public class SaveSystem : MonoBehaviour
{
    [SerializeField] SaveData data;

    private static SaveSystem _instance;
    private SaveData loadedData;

    public static SaveSystem instance { get { return _instance; } }


    void Awake()
    {
        if (_instance == null)
            _instance = this;
        else
        {
            Destroy(this.gameObject);
            return;
        }

        if (data == null)
            data = new SaveData();

        data.StartUp();
        DontDestroyOnLoad(_instance);
        
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F5))
        {
            Save();
        }

        if (Input.GetKeyDown(KeyCode.F6))
        {
            Load();
        }
    }

    void Save()
    {
        data.GetSnapShot();
        string json = JsonUtility.ToJson(data);
        File.WriteAllText(Application.persistentDataPath + "/savedat.json", json);
        Debug.Log("Test Save Done");
    }

    void Load()
    {
        string path = Application.persistentDataPath + "/savedat.json";
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            loadedData = JsonUtility.FromJson<SaveData>(json);
            Debug.Log("Data Loaded Successfully");
        }
    }
}

[System.Serializable]
public class SaveData
{
    [SerializeField] public GameObject playerController;
    [SerializeField] public DistrictManager districtManager;
    [SerializeField] public Inventory inventory;
    public HeatMechanic playerHeatMechanic;
    public PlayerMovementController playerMovementController;
    

    public void StartUp()
    {
        if (playerController == null)
        {
            playerController = GameObject.Find("PlayerController");
            playerHeatMechanic = playerController.GetComponent<HeatMechanic>();
            playerMovementController = playerController.GetComponent<PlayerMovementController>();

            if (playerController == null)
                Debug.Log("Error: Issue with finding Player Controller");
        }

        if (districtManager == null)
        {
            districtManager = GameObject.Find("DistrictManager").GetComponent<DistrictManager>();

            if (districtManager == null)
                Debug.Log("Error: Issue with finding District Manager");
        }

        if (inventory == null)
        {
            GameObject inventoryMenuUI = GameObject.Find("OverlayCanvas").GetComponent<PauseMenu>().inventoryMenuUI;
            inventoryMenuUI.SetActive(true);
            inventory = inventoryMenuUI.transform.Find("InventoryPanel").GetComponent<Inventory>();
            inventoryMenuUI.SetActive(false);

            if (inventory == null)
                Debug.Log("Error: Issue with finding Inventory Panel");
        }
    }

    public void GetSnapShot()
    {
        ISaveable.saveableData = new Dictionary<string, List<object>>();
        playerHeatMechanic.AddSavableData();
        playerMovementController.AddSavableData();
        districtManager.AddSavableData();
        inventory.AddSavableData();
    }

}
