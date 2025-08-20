using UnityEngine;
using UnityEngine.Events;

public class PlayerState : MonoBehaviour
{
    public static PlayerState Instance;
    public UnityEvent<bool> enterRoom;
    public bool inBattle;
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
}
