using UnityEngine.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class ItemSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    public Item storedItem;
    public bool isEquipped;
    [SerializeField] Image itemIcon;
    [SerializeField] Image sparkle;
    private Image itemSlotImage;
    private Color hoveringColor;
    private Color baseColor;
    private int itemStackCount;

    void Awake()
    {
        itemSlotImage = GetComponent<Image>();
        baseColor = itemSlotImage.color;
        hoveringColor = Color.white;
    }
    public void StoreItem(Item item)
    {
        storedItem = item;
        itemIcon.enabled = true;
        itemIcon.sprite = storedItem.displayIcon;
    }

    public void ClearItem()
    {
        if (isEquipped) storedItem.UnequipItem();
        storedItem = null;
        itemIcon.enabled = false;
        sparkle.enabled = false;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (storedItem == null) return;
        itemSlotImage.color = hoveringColor;
        Inventory.Instance.DisplayItem(storedItem);
        itemIcon.rectTransform.localScale = Vector2.one * 1.2f;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        itemSlotImage.color = baseColor;
        itemIcon.rectTransform.localScale = Vector2.one;
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        if (storedItem == null) return;
        if (isEquipped && Inventory.Instance.UnequipItem(storedItem))
        {
            isEquipped = false;
            itemSlotImage.color = hoveringColor;
            sparkle.enabled = false;
        }
        else if (Inventory.Instance.EquipItem(storedItem))
        {
            isEquipped = true;
            itemSlotImage.color = hoveringColor;
            sparkle.enabled = true;
            
        }
    }
}
