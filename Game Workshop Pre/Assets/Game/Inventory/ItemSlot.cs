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
    private int itemStackCount;

    void Awake()
    {
        itemSlotImage = GetComponent<Image>();
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
        sparkle.enabled = true;
        Inventory.Instance.DisplayItem(storedItem);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        sparkle.enabled = false;
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        /*
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
        */
    }
}
