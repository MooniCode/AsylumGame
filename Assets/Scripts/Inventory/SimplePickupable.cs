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

        // Create a template GameObject that can be used for dropping
        pickedUpItem.worldPrefab = CreateDropTemplate();

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
            // Clean up the template if pickup failed
            if (pickedUpItem.worldPrefab != null)
                Destroy(pickedUpItem.worldPrefab);
        }
    }

    private GameObject CreateDropTemplate()
    {
        // First try to get the actual prefab source
        GameObject prefabSource = null;

#if UNITY_EDITOR
        prefabSource = UnityEditor.PrefabUtility.GetCorrespondingObjectFromSource(gameObject);
#endif

        if (prefabSource != null)
        {
            Debug.Log($"Using prefab source for {itemName}");
            return prefabSource;
        }
        else
        {
            Debug.Log($"Creating drop template for {itemName}");
            // Create a copy of this GameObject to use as a drop template
            GameObject template = Instantiate(gameObject);
            template.name = itemName + "_DropTemplate";
            template.SetActive(false);
            DontDestroyOnLoad(template);
            return template;
        }
    }

    public void SetItemData(string name, Sprite icon)
    {
        itemName = name;
        itemIcon = icon;
    }
}