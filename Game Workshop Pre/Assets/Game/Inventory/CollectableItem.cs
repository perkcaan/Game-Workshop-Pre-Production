using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

// This is the actual object in the world that sparkles and gets picked up then destroyed
public class CollectableItem : MonoBehaviour
{
    // Item is the item that ends up in the inventory, CollectableItem is the prefab in the world
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
        if (other.TryGetComponent(out PlayerMovementController player))
        {
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
