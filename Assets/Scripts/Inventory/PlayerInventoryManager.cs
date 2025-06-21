using System;
using UnityEngine;

public class PlayerInventoryManager : MonoBehaviour
{
    [SerializeField] private HotBar hotbar;

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
    }

    private void Update()
    {
        // Handle item usage input
        if (Input.GetKeyDown(KeyCode.Mouse0) || Input.GetKeyDown(KeyCode.Space))
        {
            UseCurrentItem();
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
