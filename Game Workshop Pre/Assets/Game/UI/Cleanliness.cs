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

    public District district;
    public List<CollectableTrash> allTrash;
    public float totalTrashWeight = 0;

    public CleanBar cleanBar;
    public TMP_Text cleanText;
        

   
    // Start is called before the first frame update
    void Start()
    {
        cleanBar.gameObject.SetActive(false);
        cleanText.enabled = false;
        PlayerState.Instance.enterRoom.AddListener(OnPlayerEnterRoom);
        PlayerState.Instance.clean.AddListener(OnPlayerClean);

        district.RefreshTrash();
        allTrash = district.GetAllTrash();
        totalTrashWeight = district.totalTrashWeight;
        Debug.Log("Total trash weight in district: " + totalTrashWeight);

    }

    /*void OnEnable()
    {
        if (PlayerState.Instance != null)
        {
            PlayerState.Instance.enterRoom.AddListener(OnPlayerEnterRoom);
            
        }
    } */

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
            room = PlayerState.Instance.room;
            Debug.Log("Player entered " + room.name);

            percentClean = 0;
            cleanBar.SetClean(percentClean);
            cleanText.text = percentClean + "% clean";

            //Get room trash values from ClosedRoom script
            currentRoom = PlayerState.Instance.room.GetComponent<ClosedRoom>();
            trashList = currentRoom.trashList;
            trashTotal = trashList.Count;
            trashCollected = 0;


            UpdateCleanText();
        }

        else
        {
            //null check #whatever
            if (room != null)
            {
                Debug.Log("Player exited " + room.name);

                //Disable HUD in hallways
                cleanBar.gameObject.SetActive(false);
                cleanText.enabled = false;
            }
        }
    }


    //When player touches trash
    public void OnPlayerClean(bool cleaned)
    {
        if (cleaned)
        {

            //Get weight values from CollectableTrash script
            trash = PlayerState.Instance.trash;
            currentTrash = PlayerState.Instance.trash.GetComponent<CollectableTrash>();
            trashCollected += currentTrash.weight;

            // notify district
            FindObjectOfType<District>()
                .OnPlayerCleanDistrict(currentTrash);


            Debug.Log(currentTrash.weight);
            UpdateCleanText();
        }
    }


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
