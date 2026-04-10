using UnityEngine;

public class ObjectGate : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private BoxCollider2D boxCollider2D;
    private void Start()
    {
        transform.position += new Vector3(0, 0, -1);
        spriteRenderer = GetComponent<SpriteRenderer>();
        boxCollider2D = GetComponent<BoxCollider2D>();
        SetGates(true);
    }

    public void SetGates(bool gateStatus)
    {
        if (gateStatus)
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
