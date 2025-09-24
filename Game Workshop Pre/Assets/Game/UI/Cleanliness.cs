using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Cleanliness : MonoBehaviour
{
    public ClosedRoom currentPlayerRoom;
    private float trashTotal;
    private float cleanedTrash;
    public float percentClean = 0;
    public District district;

    public CleanBar cleanBar;
    public TMP_Text cleanText;
        
    void Start()
    {
        cleanBar.gameObject.SetActive(false);
        cleanText.enabled = false;
        PlayerState.Instance.enterRoom.AddListener(OnPlayerEnterRoom);
        PlayerState.Instance.trashDeleted.AddListener(OnPlayerClean);
    }

    void OnDisable()
    {
        if (PlayerState.Instance != null)
        {
            PlayerState.Instance.enterRoom.RemoveListener(OnPlayerEnterRoom);
        }
    }

    private void OnPlayerEnterRoom(bool entered)
    {
        if (entered)
        {
            //Show HUD
            cleanText.enabled = true;
            cleanBar.gameObject.SetActive(true);

            //Get room and set values
            currentPlayerRoom = PlayerState.Instance.currentRoom;
            percentClean = 0;
            cleanedTrash = 0;
            cleanBar.SetClean(percentClean);
            cleanText.text = percentClean + "% clean";

            //Get room trash values from ClosedRoom script
            trashTotal = currentPlayerRoom.trashList.Count;

            UpdateCleanText();
        }
        else // if player leaves room
        {
            cleanBar.gameObject.SetActive(false);
            cleanText.enabled = false;
        }
    }

    //When trash is destroyed
    public void OnPlayerClean(float amountCleaned)
    {
        cleanedTrash += amountCleaned;
        UpdateCleanText();
    }

    public void UpdateCleanText()
    {
        percentClean = (cleanedTrash / trashTotal) * 100;
        if (percentClean >= 100)
        {
            cleanText.text = "Room Clean!";
            PlayerState.Instance.ExitRoom();
            district.OnPlayerCleanRoom(currentPlayerRoom);
            //currentPlayerRoom.gameObject.SetActive(false);
        }
        else
        {
            cleanText.text = percentClean.ToString("F0") + "% clean";
        }

        cleanBar.SetClean(percentClean);
    }
}
