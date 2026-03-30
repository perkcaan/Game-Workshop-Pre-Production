using UnityEngine;

public class TrashBallTrail : MonoBehaviour
{
    [SerializeField] float lifeSpan;
    [SerializeField] float _heatPerSecond;
    float timer;
    void Start()
    {
        timer = 0;
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer > lifeSpan)
        {
            Destroy(gameObject);
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
