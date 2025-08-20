using UnityEngine;

public class Gate : MonoBehaviour
{
    SpriteRenderer spriteRenderer;
    BoxCollider2D boxCollider2D;
    void Start()
    {
        PlayerState.Instance.enterRoom.AddListener(EnterRoom);
        transform.position += new Vector3(0, 0, -3);
        spriteRenderer = GetComponent<SpriteRenderer>();
        boxCollider2D = GetComponent<BoxCollider2D>();
        EnterRoom(false);
    }

    public void EnterRoom(bool entering)
    {
        if (entering)
        {
            spriteRenderer.enabled = true;
            boxCollider2D.enabled = true;
        }
        else
        {
            spriteRenderer.enabled = false;
            boxCollider2D.enabled = false;
        }
    }
}
