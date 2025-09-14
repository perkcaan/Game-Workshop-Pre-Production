using UnityEngine;
using UnityEngine.Events;

public class PlayerState : MonoBehaviour
{
    public static PlayerState Instance;
   // public UnityEvent<bool> enterRoom;
    public UnityEvent<bool> enterRoom = new UnityEvent<bool>();
    public UnityEvent<bool> clean = new UnityEvent<bool>();
    public bool inBattle;
    public GameObject room;
    public GameObject trash;

    void Awake()
    {
        if (Instance != null) Destroy(gameObject);
        Instance = this;
        inBattle = false;
    }

    public void EnterRoom()
    {
        inBattle = true;
        enterRoom.Invoke(true);
    }

    public void ExitRoom()
    {
        inBattle = false;
        enterRoom.Invoke(false);
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Room"))
        {
            room = collision.gameObject;
            Debug.Log(room.name);
            enterRoom.Invoke(true);
        }
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Room"))
        {
            Debug.Log("Player left: " + collision.gameObject.name);
            enterRoom.Invoke(false); // notify that player exited
            room = null;
        }
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Trash"))
        {
            trash = collision.gameObject;
            clean.Invoke(true);
        }
    }

}
