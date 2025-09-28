using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class District : MonoBehaviour
{
    public List<ClosedRoom> rooms = new List<ClosedRoom>();
    public float totalRooms;
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
        totalRooms = rooms.Count;
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

    public void OnPlayerCleanRoom(ClosedRoom cleanedRoom)
    {
        UnregisterRoom(cleanedRoom);
        UpdateDistrictCleanUI();
    }

    private void UpdateDistrictCleanUI()
    {
        float percentClean = (1 - rooms.Count / totalRooms) * 100f;

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
