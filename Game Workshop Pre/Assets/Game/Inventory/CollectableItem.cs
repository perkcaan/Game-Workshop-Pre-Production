using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class CollectableItem : MonoBehaviour
{
    public Item item;

    [Header("Sparkle Effects")]
    [SerializeField] float sparkleRotationSpeed;
    [SerializeField] Transform largeSparkle;
    [SerializeField] Transform smallSparkle;
    private bool isCollected;

    void Awake()
    {
        GetComponent<SpriteRenderer>().sprite = item.displayIcon;
    }

    void Update()
    {
        smallSparkle.Rotate(0, 0, 3 * sparkleRotationSpeed * Time.deltaTime);
        largeSparkle.Rotate(0, 0, sparkleRotationSpeed * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (isCollected) return;
        if (other.TryGetComponent(out PlayerMovementController player)) {
            isCollected = true;
            Sequence sequence = DOTween.Sequence();
            sequence.Append(transform.DOScale(new Vector3(1.2f, 1.2f, 1), 0.4f).SetEase(Ease.OutQuad));
            sequence.Append(transform.DOScale(new Vector3(0f, 0f, 1), 0.3f).SetEase(Ease.OutQuad));
            sequence.OnComplete(OnCollect);
        }
    }

    void OnCollect()
    {
        Inventory.Instance.StoreItem(item);
        Destroy(gameObject);
    }
}
