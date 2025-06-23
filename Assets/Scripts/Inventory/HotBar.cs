using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HotBar : MonoBehaviour
{
    [SerializeField] private HotbarSlot[] hotbarSlots;
    public int currentSlot = 0;

    private void Start()
    {
        // Auto-find slots
        if (hotbarSlots == null || hotbarSlots.Length == 0)
        {
            hotbarSlots = GetComponentsInChildren<HotbarSlot>();
        }

        InputManager.Instance.OnPreviousInput += HandlePreviousInput;
        InputManager.Instance.OnNextInput += HandleNextInput;

        UpdateSelection();
    }

    private void HandlePreviousInput()
    {
        // Debug.Log($"Previous input - changing from slot {currentSlot}");
        currentSlot = (currentSlot - 1 + hotbarSlots.Length) % hotbarSlots.Length;
        UpdateSelection();
    }

    private void HandleNextInput()
    {
        // Debug.Log($"Next input - changing from slot {currentSlot}");
        currentSlot = (currentSlot + 1) % hotbarSlots.Length;
        UpdateSelection();
    }

    public bool AddItemToCurrentSlot(Item item)
    {
        // Debug.Log($"Trying to add {item.itemName} to slot {currentSlot}");

        // Try current slot first
        if (hotbarSlots[currentSlot].TryAddItem(item))
        {
            // Debug.Log($"Added {item.itemName} to current slot {currentSlot}");
            return true;
        }

        // If current slot is full, find first empty slot
        for (int i = 0; i < hotbarSlots.Length; i++)
        {
            if (hotbarSlots[i].TryAddItem(item))
            {
                // Debug.Log($"Added {item.itemName} to slot {i}");
                return true;
            }
        }

        Debug.Log("Hotbar is full!");
        return false;
    }

    public Item RemoveCurrentItem()
    {
        return hotbarSlots[currentSlot].RemoveItem();
    }

    public Item GetCurrentItem()
    {
        return hotbarSlots[currentSlot].StoredItem;
    }

    public void UseCurrentItem()
    {
        hotbarSlots[currentSlot].UseItem();
    }
    
    public bool IsCurrentSlotEmpty()
    {
        return !hotbarSlots[currentSlot].HasItem;
    }

    public void UpdateSelection()
    {
        for (int i = 0; i < hotbarSlots.Length; i++)
        {
            hotbarSlots[i].SetSelected(i == currentSlot);
        }
    }

    // Save system methods
    public List<string> GetHotbarItems()
    {
        List<string> itemNames = new List<string>();

        foreach (HotbarSlot slot in hotbarSlots)
        {
            if (slot.HasItem)
            {
                itemNames.Add(slot.StoredItem.itemName);
            }
            else
            {
                itemNames.Add(""); // Empty slot
            }
        }

        return itemNames;
    }

    public void SetHotbarFromSave(List<string> itemNames)
    {
        for (int i = 0; i < hotbarSlots.Length && i < itemNames.Count; i++)
        {
            // Clear the slot first
            hotbarSlots[i].RemoveItem();

            // Add item of not empty
            if (!string.IsNullOrEmpty(itemNames[i]))
            {
                Item item = CreateItemFromName(itemNames[i]);
                if (item != null)
                {
                    hotbarSlots[i].TryAddItem(item);
                }
            }
        }

        UpdateSelection(); // Refresh the visual
    }

    private Item CreateItemFromName(string itemName)
    {
        if (ItemDatabase.Instance != null)
        {
            return ItemDatabase.Instance.GetItemByName(itemName);
        }
        return null;
    }

    private void OnDestroy()
    {
        // Unsubscribe from events to prevent memory leaks
        if (InputManager.Instance != null)
        {
            InputManager.Instance.OnPreviousInput -= HandlePreviousInput;
            InputManager.Instance.OnNextInput -= HandleNextInput;
        }
    }
}
