using UnityEngine;

public class Gate : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private BoxCollider2D boxCollider2D;
    private void Start()
    {
        //DistrictManager.Instance.OnGateFlip += SetGates;
        transform.position += new Vector3(0, 0, -3);
        spriteRenderer = GetComponent<SpriteRenderer>();
        boxCollider2D = GetComponent<BoxCollider2D>();
        //SetGates(DistrictManager.Instance.AreGatesUp);
    }

    private void OnDisable()
    {
        if (DistrictManager.Instance == null) return;
        DistrictManager.Instance.OnGateFlip -= SetGates;
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
