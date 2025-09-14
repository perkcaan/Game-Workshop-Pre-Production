using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Cleanliness : MonoBehaviour
{
    public GameObject room;
    public GameObject trash;
    private ClosedRoom currentRoom;
    private CollectableTrash currentTrash;
    private List<CollectableTrash> trashList; // get trash list from room script
    public float trashCollected;
    public float trashTotal;
    public float percentClean = 0;


    public CleanBar cleanBar;
    public TMP_Text cleanText;
        


    // Start is called before the first frame update
    void Start()
    {
        cleanBar.gameObject.SetActive(false);
        cleanText.enabled = false;
        PlayerState.Instance.enterRoom.AddListener(OnPlayerEnterRoom);
       PlayerState.Instance.clean.AddListener(OnPlayerClean);

    }

    /*void OnEnable()
    {
        if (PlayerState.Instance != null)
        {
            PlayerState.Instance.enterRoom.AddListener(OnPlayerEnterRoom);
            Debug.Log("poisonnnn");
        }
    } */

    void OnDisable()
    {
        if (PlayerState.Instance != null)
            PlayerState.Instance.enterRoom.RemoveListener(OnPlayerEnterRoom);
    }


    private void OnPlayerEnterRoom(bool entered)
    {

        if (entered)
        {
             cleanText.enabled = true;
            cleanBar.gameObject.SetActive(true);
            room = PlayerState.Instance.room;
            Debug.Log("Player entered " + room.name);

            percentClean = 0;
            cleanBar.SetClean(percentClean);
            cleanText.text = percentClean + "% clean";

            currentRoom = PlayerState.Instance.room.GetComponent<ClosedRoom>();
            trashList = currentRoom.trashList;
            trashTotal = trashList.Count;
            trashCollected = 0;

           // Debug.Log(trashList.Count);
            UpdateCleanText();
        }

        else
        {
            Debug.Log("Player exited " + room.name);
            cleanBar.gameObject.SetActive(false);
            cleanText.enabled = false;
          //  room = PlayerState.Instance.room;
            
        }
    }

    public void OnPlayerClean(bool cleaned)
    {
        if (cleaned)
        {
            trash = PlayerState.Instance.trash;
            currentTrash = PlayerState.Instance.trash.GetComponent<CollectableTrash>();
            trashCollected += currentTrash.weight;
            Debug.Log(currentTrash.weight);
            UpdateCleanText();
        }
    }



    // Update is called once per frame
    public void UpdateCleanText()
    {
        percentClean = (trashCollected / trashTotal) * 100;
        if (percentClean >= 100)
        {
            cleanText.text = "Room Clean!";
        }
        else
        {
            cleanText.text = percentClean.ToString("F0") + "% clean";
        }

        cleanBar.SetClean(percentClean);
    }
}
