using UnityEngine;

public class HeatAreaHitbox : MonoBehaviour
{
    [SerializeField] private float _maxHeatCaused;
    [SerializeField] private float _heatPerSecond;
    private Collider2D _collider;

    private void Awake()
    {
        _collider = GetComponent<Collider2D>();
        _collider.enabled = false;
    }

    public void Enable()
    {
        _collider.enabled = true;
    }

    public void Disable()
    {
        _collider.enabled = false;
    }

    private void OnTriggerStay2D(Collider2D collider)
    {
        if (collider.TryGetComponent(out HeatMechanic heat))
        {
            if (heat.Heat < _maxHeatCaused)
            {
                    heat.ModifyHeat(_heatPerSecond * Time.fixedDeltaTime);
            } else
            {
                    heat.ModifyHeat(0); // dont cooldown, just stay at max heat
            }
            
        }
    }

}
