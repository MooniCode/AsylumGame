using UnityEngine;
using UnityEngine.UI;

public class HotbarSlot : MonoBehaviour
{
    [SerializeField] private Image slotBackground;
    [SerializeField] private Image itemIcon;
    [SerializeField] private Sprite selectedSprite;
    [SerializeField] private Sprite unselectedSprite;

    private Item storedItem;
    private bool isSelected;

    public bool HasItem => storedItem != null;
    public Item StoredItem => storedItem;

    private void Awake()
    {
        if (slotBackground == null)
        {
            slotBackground = GetComponent<Image>();
        }

        if (itemIcon == null && transform.childCount > 0)
        {
            itemIcon = transform.GetChild(0).GetComponent<Image>();
        }
    }

    public bool TryAddItem(Item item)
    {
        if (storedItem != null)
            return false; // Slot is occupied

        storedItem = item;
        UpdateVisual();
        return true;
    }

    public Item RemoveItem()
    {
        Item removedItem = storedItem;
        storedItem = null;
        UpdateVisual();
        return removedItem;
    }

    public void SetSelected(bool selected)
    {
        isSelected = selected;
        UpdateVisual();
    }

    public void UseItem()
    {
        if (storedItem != null)
        {
            storedItem.Use();
        }
    }

    private void UpdateVisual()
    {
        // Update selection highlight
        if (slotBackground != null)
        {
            slotBackground.sprite = isSelected ? selectedSprite : unselectedSprite;
        }

        // Update item icon
        if (itemIcon != null)
        {
            if (storedItem != null)
            {
                itemIcon.sprite = storedItem.icon;
                itemIcon.enabled = true;
            }
            else
            {
                itemIcon.sprite = null;
                itemIcon.enabled = false;
            }
        }
    }
}
