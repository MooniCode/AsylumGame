using System;
using UnityEngine;
using UnityEngine.VFX;

public class PlayerInventoryManager : MonoBehaviour
{
    [SerializeField] private HotBar hotbar;
    private Camera playerCamera;

    private void Start()
    {
        // Auto-find hotbar
        if (hotbar == null)
        {
            hotbar = FindAnyObjectByType<HotBar>();

            if (hotbar == null)
            {
                Debug.Log("Hotbar is not found in the scene");
            }
        }

        playerCamera = GetComponentInChildren<Camera>();
    }

    private void Update()
    {
        // Handle item usage input
        if (Input.GetKeyDown(KeyCode.Mouse0) || Input.GetKeyDown(KeyCode.Space))
        {
            UseCurrentItem();
        }
    }

    public void DropCurrentItem()
    {
        if (hotbar != null)
        {
            Item droppedItem = hotbar.RemoveCurrentItem();
            if (droppedItem != null)
            {
                SpawnDroppedItem(droppedItem);
                Debug.Log($"Dropped {droppedItem.itemName}");
            }
        }
    }

    private void SpawnDroppedItem(Item item)
    {
        // Check if the item has a world prefab
        if (item.worldPrefab == null)
        {
            Debug.LogWarning($"Item {item.itemName} does not have a world prefab to drop.");
            return;
        }

        // Raycast down from player camera to find ground
        Vector3 dropPosition;
        if (Physics.Raycast(playerCamera.transform.position, Vector3.down, out RaycastHit hit, Mathf.Infinity))
        {
            dropPosition = hit.point;
        }
        else
        {
            // If no ground found, just drop at players feet
            dropPosition = transform.position;
        }

        // Instantiate the item in the world
        GameObject droppedObject = Instantiate(item.worldPrefab, dropPosition, Quaternion.identity);

        // Makse sure the dropped object is active
        droppedObject.SetActive(true);

        // Configure the dropped item with the original data
        SimplePickupable pickupable = droppedObject.GetComponent<SimplePickupable>();
        if (pickupable != null)
        {
            pickupable.SetItemData(item.itemName, item.icon);
        }
    }

    public bool TryAddItem(Item item)
    {
        if (hotbar != null)
        {
            return hotbar.AddItemToCurrentSlot(item);
        }

        Debug.Log("No hotbar available to add item to");
        return false;
    }

    private void UseCurrentItem()
    {
        if (hotbar != null)
        {
            hotbar.UseCurrentItem();
        }
    }

    public Item GetCurrentItem()
    {
        return hotbar != null ? hotbar.GetCurrentItem() : null;
    }

    public bool HasCurrentItem()
    {
        return hotbar != null && !hotbar.IsCurrentSlotEmpty();
    }
}
