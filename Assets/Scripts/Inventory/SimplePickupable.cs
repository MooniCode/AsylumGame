using UnityEngine;

[System.Serializable]
public class SimplePickupable : PickupableItem
{
    [SerializeField] private string itemName = "Simple Item";
    [SerializeField] private Sprite itemIcon;

    public override string ItemName => itemName;

    public override void OnPickup(GameObject player)
    {
        // Create the appropriate item data
        Item pickedUpItem = new SimpleItem();
        pickedUpItem.itemName = itemName;
        pickedUpItem.icon = itemIcon;

        // Try to get the inventory manager from the player
        PlayerInventoryManager inventoryManager = player.GetComponent<PlayerInventoryManager>();

        if (inventoryManager != null && inventoryManager.TryAddItem(pickedUpItem))
        {
            Destroy(gameObject);
            Debug.Log($"Picked up {ItemName}");
        }
        else
        {
            Debug.Log($"Could not pick up {itemName} - inventory might be full or missing");
        }
    }
}
