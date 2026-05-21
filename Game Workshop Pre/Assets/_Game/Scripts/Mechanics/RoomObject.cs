using UnityEngine;

public class RoomObject : MonoBehaviour
{
    // private static int roomLayerMask = -1;
    // private void OnValidate()
    // {
    //     if (roomLayerMask == -1) roomLayerMask = LayerMask.GetMask("Room");
    //     Collider2D hit = Physics2D.OverlapPoint(transform.position, roomLayerMask);
    //     if (hit == null) return;
    //     Room room = hit.GetComponent<Room>();
    //     if (room == null) return;

    //     Debug.Log($"{gameObject.name} hit Room {hit.gameObject.name}");
    // }
}
