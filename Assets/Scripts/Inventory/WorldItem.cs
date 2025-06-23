using UnityEngine;

public class WorldItem : MonoBehaviour
{
    [SerializeField] private string itemId;

    public string ItemId => itemId;

    private void Start()
    {
        // Register with the world item manager when spawned
        if (WorldItemManager.Instance != null)
        {
            WorldItemManager.Instance.RegisterWorldItem(this);
        }

        // Make sure this item is in the correct layer for pickup detection
        gameObject.layer = LayerMask.NameToLayer("Item");
    }

    private void OnDestroy()
    {
        // Unregister when destroyed
        if (WorldItemManager.Instance != null)
        {
            WorldItemManager.Instance.UnregisterWorldItem(this);
        }
    }

    public void SetItemId(string id)
    {
        itemId = id;
    }

    // This method is called when the item is picked up
    public void OnPickedUp()
    {
        // The WorldItemManager will automatically unregister the item when it is destroyed
        Destroy(gameObject);
    }
}
