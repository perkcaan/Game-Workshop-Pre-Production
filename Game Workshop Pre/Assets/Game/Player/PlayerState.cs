using UnityEngine;
using UnityEngine.Events;

public class PlayerState : MonoBehaviour
{
    public static PlayerState Instance;
    public UnityEvent<bool> enterRoom = new UnityEvent<bool>();
    public UnityEvent<float> trashDeleted = new UnityEvent<float>();
    public bool inBattle;
    public ClosedRoom currentRoom;

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

}
