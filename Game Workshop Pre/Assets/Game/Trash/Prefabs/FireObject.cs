using UnityEngine;

public class FireObject : MonoBehaviour
{
    [SerializeField] float _heatPerSecond;

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.TryGetComponent(out TrashBall trashBall))
        {
            trashBall.isBurning = true;
        }
    }

    void OnTriggerStay2D(Collider2D collider)
    {
        if (collider.TryGetComponent(out HeatMechanic heat))
        {
            heat.ModifyHeat(_heatPerSecond * Time.fixedDeltaTime);
        }
    }
}
