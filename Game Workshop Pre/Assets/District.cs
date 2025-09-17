using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class District : MonoBehaviour
{
    public List<ClosedRoom> rooms = new List<ClosedRoom>();
    public List<CollectableTrash> allTrash = new List<CollectableTrash>();
    public float collectedTrashCount;

    public DistrictBar districtCleanBar;
    public TMP_Text districtCleanText;


    void Awake()
    {
        rooms = new List<ClosedRoom>(GetComponentsInChildren<ClosedRoom>(true));
    }

    void Start()
    {
        districtCleanBar.SetClean(0);
        districtCleanText.text = "District 0% clean";
        collectedTrashCount = 0;
        // fill trash after rooms have run their Start/Awake
        RefreshTrash();
    }

    public void RefreshTrash()
    {
        allTrash = new List<CollectableTrash>();
        foreach (var room in rooms)
        {
            allTrash.AddRange(room.trashList);
        }
    }


    public void RegisterRoom(ClosedRoom room)
    {
        if (!rooms.Contains(room))
            rooms.Add(room);
    }

    public void UnregisterRoom(ClosedRoom room)
    {
        if (rooms.Contains(room))
            rooms.Remove(room);
    }

    public List<ClosedRoom> GetAllRooms() => rooms;

    public List<CollectableTrash> GetAllTrash()
    {
        List<CollectableTrash> allTrash = new List<CollectableTrash>();
        foreach (var room in rooms)
        {
            allTrash.AddRange(room.trashList);
        }
        return allTrash;
    }

    public void OnPlayerCleanDistrict(CollectableTrash trash)
    {
        collectedTrashCount += 1;
        UpdateDistrictCleanUI();
    }

    private void UpdateDistrictCleanUI()
    {
        float percentClean = collectedTrashCount / allTrash.Count * 100f;

        if (percentClean >= 100f)
        {
            districtCleanText.text = "District Clean!";
        }
        else
        {
            districtCleanText.text = "District " + percentClean.ToString("F0") + "% clean";
        }

        districtCleanBar.SetClean(percentClean);
    }

}
