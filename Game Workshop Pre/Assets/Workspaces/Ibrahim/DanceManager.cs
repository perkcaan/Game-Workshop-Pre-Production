using System.Collections;
using DG.Tweening;
using UnityEngine;

public class DanceManager : Singleton<DanceManager>
{
    [SerializeField] PlayerMovementController player;
    [SerializeField] Transform sparkle;
    [SerializeField] RectTransform banner;
    [SerializeField] MiteDancer[] miteDancers;

    void Start()
    {
        sparkle.localScale = Vector2.zero;
        banner.gameObject.SetActive(false);
        BreakItDown();
    }

    void Update()
    {
        sparkle.Rotate(0, 0, 90f * Time.deltaTime);
    }

    [EventAction]
    public void BreakItDown()
    {
        sparkle.gameObject.SetActive(true);
        banner.gameObject.SetActive(true);
        banner.DOAnchorPosY(300f, 1f).SetEase(Ease.OutBack);

        sparkle.DOScale(Vector3.one, 2).SetEase(Ease.OutBack);
        player.gameObject.transform.DOMove(sparkle.position, 0.5f);
        player.Dance();
        foreach (MiteDancer dancer in miteDancers)
        {
            dancer.gameObject.SetActive(true);
            dancer.Dance();
        }
    }
}
