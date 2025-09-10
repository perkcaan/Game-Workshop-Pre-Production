using UnityEngine;
using UnityEngine.Events;

public class PlayerState : MonoBehaviour
{
    public static PlayerState Instance;
   // public UnityEvent<bool> enterRoom;
    public UnityEvent<bool> enterRoom = new UnityEvent<bool>();
    public bool inBattle;
    public GameObject room; 

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

}
