using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lava : MonoBehaviour
{
    [SerializeField] private float _heatPerSecond = 1f;

    private void OnTriggerStay2D(Collider2D collider)
    {
        if (collider.gameObject.TryGetComponent(out HeatMechanic heat))
        {
            heat.ModifyHeat(_heatPerSecond * Time.fixedDeltaTime);
        }
    }
}
