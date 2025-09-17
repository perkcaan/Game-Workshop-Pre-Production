using UnityEngine;
using UnityEngine.Events;

public class PlayerState : MonoBehaviour
{
    public static PlayerState Instance;
   // public UnityEvent<bool> enterRoom;
    public UnityEvent<bool> enterRoom = new UnityEvent<bool>();
    public UnityEvent<bool> clean = new UnityEvent<bool>();
    public bool inBattle;
    public ClosedRoom currentRoom;
    public GameObject trash;

    void Awake()
    {
        if (Instance != null) Destroy(gameObject);
        Instance = this;
        inBattle = false;
    }

    public void EnterRoom(ClosedRoom enteredRoom)
    {
        currentRoom = enteredRoom;
        inBattle = true;
        enterRoom.Invoke(true);
    }

    public void ExitRoom()
    {
        currentRoom = null;
        inBattle = false;
        enterRoom.Invoke(false);
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        /*
        if (collision.TryGetComponent(out ClosedRoom room))
        {
            if (room.trashList.Count > 0)
            {
                currentRoom = room;
                enterRoom.Invoke(true);
            }
        }
        */
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        /*
        if (collision.TryGetComponent(out ClosedRoom room))
        {
            if (room.trashList.Count == 0)
            {
                currentRoom = null;
                enterRoom.Invoke(false);
            }
        }
        */
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        /*
        if (collision.gameObject.CompareTag("Trash"))
        {
            trash = collision.gameObject;
            clean.Invoke(true);
        }
        */
    }

}
