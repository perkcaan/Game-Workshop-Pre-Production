using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Cleanliness : MonoBehaviour
{
    public GameObject room;
    private ClosedRoom currentRoom;
    private List<CollectableTrash> trashList; // get trash list from room script
    public int trashCollected;
    public int trashTotal;

    public TMP_Text cleanText;
        


    // Start is called before the first frame update
    void Start()
    {

       PlayerState.Instance.enterRoom.AddListener(OnPlayerEnterRoom);
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
            room = PlayerState.Instance.room;
            Debug.Log("Player entered " + room.name);

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
          //  room = PlayerState.Instance.room;
        }
    }

    // Update is called once per frame
    public void UpdateCleanText()
    {
        cleanText.text = (trashCollected / trashTotal) * 100 + "% clean";
    }
}
