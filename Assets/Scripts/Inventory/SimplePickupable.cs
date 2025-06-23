using UnityEngine;

[System.Serializable]
public class SimplePickupable : PickupableItem
{
    [SerializeField] private string itemName = "Simple Item";
    [SerializeField] private Sprite itemIcon;

    public override string ItemName => itemName;
    public Sprite ItemIcon => itemIcon;

    public override void OnPickup(GameObject player)
    {
        // Get tge complete item data from the ItemDatabase
        Item pickedUpItem = null;

        if (ItemDatabase.Instance != null)
        {
            pickedUpItem = ItemDatabase.Instance.GetItemByName(itemName);
        }

        // Try to get the inventory manager from the player
        PlayerInventoryManager inventoryManager = player.GetComponent<PlayerInventoryManager>();
        if (inventoryManager != null && inventoryManager.TryAddItem(pickedUpItem))
        {
            // Get the WorldItem component and call OnPickedUp to properly unregister
            WorldItem worldItem = GetComponent<WorldItem>();
            if (worldItem != null)
            {
                worldItem.OnPickedUp(); // This will destroy the object and unregister it
            }
            else
            {
                Destroy(gameObject); // Fallback if no WorldItem component
            }
        }
        else
        {
            Debug.Log($"Could not pick up {itemName} - inventory might be full or missing");
        }
    }

    public void SetItemData(string name, Sprite icon)
    {
        itemName = name;
        itemIcon = icon;
    }
}