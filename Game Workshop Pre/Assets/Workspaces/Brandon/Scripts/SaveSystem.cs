using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

public class SaveSystem : MonoBehaviour
{
    [SerializeField] private string saveFileName = "savedat.json";

    private static SaveSystem _instance;
    public static SaveSystem instance => _instance;

    private SaveData _refs;
    private SaveGame _loadedGame;

    private string SavePath => Path.Combine(Application.persistentDataPath, saveFileName);

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        _refs = new SaveData();
        _refs.StartUp();
    }

    private void Update()
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

    public void Save()
    {
        _refs.StartUp();

        SaveContext.Begin(SceneManager.GetActiveScene().name);

        if (_refs.playerMovementController != null)
            _refs.playerMovementController.AddSaveableData();

        if (_refs.districtManager != null)
            _refs.districtManager.AddSaveableData();

        if (_refs.inventory != null)
            _refs.inventory.AddSaveableData();

        if (_refs.score != null)
            _refs.score.AddSaveableData();

        string json = JsonUtility.ToJson(SaveContext.Current, true);
        File.WriteAllText(SavePath, json);

    }

    public void Load()
    {
        if (!File.Exists(SavePath))
        {
            return;
        }

        string json = File.ReadAllText(SavePath);
        _loadedGame = JsonUtility.FromJson<SaveGame>(json);

        if (_loadedGame == null)
        {
            return;
        }

        string activeScene = SceneManager.GetActiveScene().name;

        if (!string.Equals(activeScene, _loadedGame.savedScene, StringComparison.Ordinal))
        {
            SceneManager.sceneLoaded -= OnSceneLoadedApplySave;
            SceneManager.sceneLoaded += OnSceneLoadedApplySave;
            SceneManager.LoadScene(_loadedGame.savedScene, LoadSceneMode.Single);
            return;
        }

        StartCoroutine(ApplyLoadedGameRoutine());
    }

    private void OnSceneLoadedApplySave(Scene scene, LoadSceneMode mode)
    {
        SceneManager.sceneLoaded -= OnSceneLoadedApplySave;
        StartCoroutine(ApplyLoadedGameRoutine());
    }

    private IEnumerator ApplyLoadedGameRoutine()
    {
        yield return null;
        yield return null;

        _refs.StartUp();
        ApplyLoadedGame(_loadedGame);

    }

    private void ApplyLoadedGame(SaveGame game)
    {
        if (game == null)
            return;

        ApplyPlayer(game.playerData);
        ApplyDistrict(game.districtManagerData);
        ApplyInventory(game.inventoryData);
        ApplyScore(game.scoreData);
    }

    private void ApplyPlayer(PlayerSaveData data)
    {
        if (data == null || _refs.playerController == null || _refs.playerMovementController == null)
            return;

        _refs.playerController.transform.position = data.position;

        _refs.playerMovementController.canSweep = data.canSweep;
        _refs.playerMovementController.canPoke = data.canPoke;
        _refs.playerMovementController.canSwipe = data.canSwipe;
        _refs.playerMovementController.canDash = data.canDash;
        _refs.playerMovementController.canHook = data.canHook;

        if (_refs.playerHeatMechanic != null)
        {
            if (!TrySetMember(_refs.playerHeatMechanic, "Heat", data.heat))
            {
                TrySetMember(_refs.playerHeatMechanic, "_heat", data.heat);
            }
        }
    }

    private void ApplyDistrict(DistrictManagerSaveData data)
    {
        if (data == null || _refs.districtManager == null)
            return;

        TrySetMember(_refs.districtManager, "coinsEarned", data.coinsEarned);

        PlayerPrefs.SetInt("Coins", data.coinsEarned);
        PlayerPrefs.Save();

        Room[] sceneRooms = FindObjectsByType<Room>(FindObjectsSortMode.None);
        Dictionary<string, Room> roomLookup = new Dictionary<string, Room>();

        foreach (Room room in sceneRooms)
        {
            string path = GetTransformPath(room.transform);
            if (!roomLookup.ContainsKey(path))
                roomLookup.Add(path, room);
        }

        foreach (RoomSaveData roomData in data.rooms)
        {
            if (roomData == null || string.IsNullOrEmpty(roomData.path))
                continue;

            if (!roomLookup.TryGetValue(roomData.path, out Room room))
                continue;

            TrySetMember(room, "Cleanliness", roomData.cleanliness);
            TrySetMember(room, "_cleanliness", roomData.cleanliness);

            TrySetMember(room, "IsRoomCleaned", roomData.isRoomCleaned);
            TrySetMember(room, "_isRoomCleaned", roomData.isRoomCleaned);

            TrySetMember(room, "IsPlayerInRoom", roomData.isPlayerInRoom);
            TrySetMember(room, "_isPlayerInRoom", roomData.isPlayerInRoom);

            TrySetMember(room, "FreeTrashAmount", roomData.freeTrashAmount);
            TrySetMember(room, "_freeTrashAmount", roomData.freeTrashAmount);
        }
    }

    private void ApplyInventory(InventorySaveData data)
    {
        if (data == null || _refs.inventory == null)
            return;

        ClearInventory(_refs.inventory);

        foreach (InventoryItemSaveData itemData in data.items)
        {
            Item item = FindItemAsset(itemData);
            if (item != null)
            {
                _refs.inventory.StoreItem(item);
            }
        }
    }

    private void ApplyScore(ScoreSaveData data)
    {
        if (data == null || _refs.score == null)
            return;

        _refs.score.currentScore = data.currentScore;
        _refs.score.currentBonus = data.currentBonus;
        _refs.score.timeLeft = data.timeLeft;

        TrySetMember(_refs.score, "thisBonusScore", data.thisBonusScore);

        InvokePrivateMethod(_refs.score, "UpdateUI");
    }

    private void ClearInventory(Inventory inventory)
    {
        BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        FieldInfo equippedItemsField = typeof(Inventory).GetField("equippedItems", flags);
        if (equippedItemsField != null)
        {
            IList equippedItems = equippedItemsField.GetValue(inventory) as IList;
            equippedItems?.Clear();
        }

        FieldInfo itemSlotsField = typeof(Inventory).GetField("itemSlots", flags);
        if (itemSlotsField != null)
        {
            IEnumerable slots = itemSlotsField.GetValue(inventory) as IEnumerable;
            if (slots != null)
            {
                foreach (object slot in slots)
                {
                    if (slot == null) continue;
                    MethodInfo clearMethod = slot.GetType().GetMethod("ClearItem", flags);
                    clearMethod?.Invoke(slot, null);
                }
            }
        }
    }

    private Item FindItemAsset(InventoryItemSaveData itemData)
    {
        Item[] allItems = Resources.FindObjectsOfTypeAll<Item>();

        Item byAssetName = allItems.FirstOrDefault(x => x != null && x.name == itemData.assetName);
        if (byAssetName != null)
            return byAssetName;

        Item byDisplayName = allItems.FirstOrDefault(x => x != null && x.displayName == itemData.displayName);
        if (byDisplayName != null)
            return byDisplayName;

        return null;
    }

    private static bool TrySetMember(object target, string memberName, object value)
    {
        if (target == null || string.IsNullOrEmpty(memberName))
            return false;

        BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
        Type type = target.GetType();

        PropertyInfo property = type.GetProperty(memberName, flags);
        if (property != null && property.CanWrite)
        {
            property.SetValue(target, ConvertIfNeeded(value, property.PropertyType));
            return true;
        }

        FieldInfo field = type.GetField(memberName, flags);
        if (field != null)
        {
            field.SetValue(target, ConvertIfNeeded(value, field.FieldType));
            return true;
        }

        return false;
    }

    private static object ConvertIfNeeded(object value, Type targetType)
    {
        if (value == null)
            return null;

        Type valueType = value.GetType();
        if (targetType.IsAssignableFrom(valueType))
            return value;

        try
        {
            return Convert.ChangeType(value, targetType);
        }
        catch
        {
            return value;
        }
    }

    private static void InvokePrivateMethod(object target, string methodName)
    {
        if (target == null)
            return;

        BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
        MethodInfo method = target.GetType().GetMethod(methodName, flags);
        method?.Invoke(target, null);
    }

    public static string GetTransformPath(Transform target)
    {
        if (target == null)
            return string.Empty;

        string path = target.name;
        Transform current = target.parent;

        while (current != null)
        {
            path = current.name + "/" + path;
            current = current.parent;
        }

        return path;
    }
}

public static class SaveContext
{
    public static SaveGame Current { get; private set; }

    public static void Begin(string sceneName)
    {
        Current = new SaveGame();
        Current.savedScene = sceneName;
    }
}

[Serializable]
public class SaveData
{
    [NonSerialized] public GameObject playerController;
    [NonSerialized] public DistrictManager districtManager;
    [NonSerialized] public Inventory inventory;
    [NonSerialized] public ScoreBehavior score;
    [NonSerialized] public HeatMechanic playerHeatMechanic;
    [NonSerialized] public PlayerMovementController playerMovementController;

    public void StartUp()
    {
        playerController = GameObject.Find("PlayerController");
        if (playerController != null)
        {
            playerHeatMechanic = playerController.GetComponent<HeatMechanic>();
            playerMovementController = playerController.GetComponent<PlayerMovementController>();
        }

        GameObject districtManagerObject = GameObject.Find("DistrictManager");
        if (districtManagerObject != null)
        {
            districtManager = districtManagerObject.GetComponent<DistrictManager>();
        }

        GameObject overlayCanvas = GameObject.Find("OverlayCanvas");
        if (overlayCanvas != null)
        {
            PauseMenu pauseMenu = overlayCanvas.GetComponent<PauseMenu>();
            if (pauseMenu != null && pauseMenu.inventoryMenuUI != null)
            {
                bool wasActive = pauseMenu.inventoryMenuUI.activeSelf;
                pauseMenu.inventoryMenuUI.SetActive(true);

                Transform panel = pauseMenu.inventoryMenuUI.transform.Find("InventoryPanel");
                if (panel != null)
                {
                    inventory = panel.GetComponent<Inventory>();
                }

                pauseMenu.inventoryMenuUI.SetActive(wasActive);
            }
        }

        GameObject scoreBar = GameObject.Find("ScoreBar");
        if (scoreBar != null)
        {
            score = scoreBar.GetComponent<ScoreBehavior>();
        }
    }
}

[Serializable]
public class SaveGame
{
    public string savedScene;
    public PlayerSaveData playerData;
    public DistrictManagerSaveData districtManagerData;
    public InventorySaveData inventoryData;
    public ScoreSaveData scoreData;
}

[Serializable]
public class PlayerSaveData
{
    public Vector3 position;
    public float heat;
    public bool canSweep;
    public bool canPoke;
    public bool canSwipe;
    public bool canDash;
    public bool canHook;

    public PlayerSaveData(GameObject player, PlayerMovementController pmc)
    {
        position = player.transform.position;
        heat = pmc.PlayerHeat != null ? pmc.PlayerHeat.Heat : 0f;
        canSweep = pmc.canSweep;
        canPoke = pmc.canPoke;
        canSwipe = pmc.canSwipe;
        canDash = pmc.canDash;
        canHook = pmc.canHook;
    }
}

[Serializable]
public class ScoreSaveData
{
    public int currentScore;
    public int currentBonus;
    public float timeLeft;
    public int thisBonusScore;

    public ScoreSaveData(ScoreBehavior score, int comboScore)
    {
        currentScore = score.currentScore;
        currentBonus = score.currentBonus;
        timeLeft = score.timeLeft;
        thisBonusScore = comboScore;
    }
}

[Serializable]
public class DistrictManagerSaveData
{
    public int coinsEarned;
    public List<RoomSaveData> rooms = new List<RoomSaveData>();
}

[Serializable]
public class RoomSaveData
{
    public string path;
    public float cleanliness;
    public bool isRoomCleaned;
    public bool isPlayerInRoom;
    public int freeTrashAmount;

    public RoomSaveData(Room room)
    {
        path = SaveSystem.GetTransformPath(room.transform);
        cleanliness = room.Cleanliness;
        isRoomCleaned = room.IsRoomCleaned;
        isPlayerInRoom = room.IsPlayerInRoom;
        freeTrashAmount = room.FreeTrashAmount;
    }
}

[Serializable]
public class InventorySaveData
{
    public List<InventoryItemSaveData> items = new List<InventoryItemSaveData>();
}

[Serializable]
public class InventoryItemSaveData
{
    public string assetName;
    public string displayName;
    public string descriptionText;
    public string iconName;

    public InventoryItemSaveData(Item item)
    {
        assetName = item.name;
        displayName = item.displayName;
        descriptionText = item.discriptionText;
        iconName = item.displayIcon != null ? item.displayIcon.name : string.Empty;
    }
}

[Serializable]
public class EntitySaveData
{
    public Vector3 position;
    public float moveSpeed;
    public float speedModifer;
    public float facingRotation;
    public float stunTime;

}

[Serializable]
public class TrashSaveData
{
    public Vector3 position;
    public int size;
    public int trashMatSize;

}

[Serializable]
public class TrashBallSaveData
{
    public Vector3 position;
    public int size;
    public float healthPercent;
}