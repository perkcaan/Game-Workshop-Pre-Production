using System.Collections;
using DG.Tweening;
using UnityEngine;

public class DanceManager : Singleton<DanceManager>
{
    [SerializeField] PlayerMovementController player;
    [SerializeField] Transform sparkle;
    [SerializeField] MiteDancer[] miteDancers;

    void Start()
    {
        sparkle.localScale = Vector2.zero;
        BreakItDown();
    }

    void Update()
    {
        sparkle.Rotate(0, 0, 90f * Time.deltaTime);
    }

    public void BreakItDown()
    {
        sparkle.DOScale(Vector3.one, 2).SetEase(Ease.OutBack);
        player.gameObject.transform.DOMove(sparkle.position, 0.5f);
        player.Dance();
        foreach (MiteDancer dancer in miteDancers)
        {
            dancer.gameObject.SetActive(true);
            dancer.Dance();
        }

        StartCoroutine(RoomClearParticles());
    }

    private IEnumerator RoomClearParticles()
    {
        int rings = 12;
        int bubblesInRing = 4;
        float radiusStep = 1.2f;
        float delayBetweenRings = 0.05f;

        for (int i = 0; i < rings; i++)
        {
            float currentRadius = (i + 1) * radiusStep;

            for (int j = 0; j < bubblesInRing; j++)
            {
                float angle = j * (Mathf.PI * 2 / bubblesInRing);
                float x = Mathf.Cos(angle) * currentRadius;
                float y = Mathf.Sin(angle) * currentRadius;

                Vector3 bubblePosition = sparkle.transform.position + new Vector3(x, y, 0);
                ParticleManager.Instance.Play("RoomClear", bubblePosition);
            }
            bubblesInRing += 2 * (i + 1);
            yield return new WaitForSeconds(delayBetweenRings);
        }
    }
}
