using UnityEngine;

public class TrashBallTrail : MonoBehaviour
{
    [SerializeField] float lifeSpan;
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
}
