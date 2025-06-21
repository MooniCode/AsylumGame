using System;
using UnityEngine;
using UnityEngine.UI;

public class HotBar : MonoBehaviour
{
    [SerializeField] private HotbarSlot[] hotbarSlots;
    private int currentSlot = 0;

    private void Start()
    {
        // Auto-find slots
        if (hotbarSlots == null || hotbarSlots.Length == 0)
        {
            hotbarSlots = GetComponentsInChildren<HotbarSlot>();
        }

        UpdateSelection();
    }

    private void Update()
    {
        HandleScrollInput();
        HandleNumberKeyInput();
    }

    private void HandleScrollInput()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if (scroll > 0f)
        {
            Debug.Log($"Scrolling up from slot {currentSlot}");
            currentSlot = (currentSlot + 1) % hotbarSlots.Length;
            UpdateSelection();
        }
        else if (scroll < 0f)
        {
            Debug.Log($"Scrolling down from slot {currentSlot}");
            currentSlot = (currentSlot - 1 + hotbarSlots.Length) % hotbarSlots.Length;
            UpdateSelection();
        }
    }

    private void HandleNumberKeyInput()
    {
        for (int i = 0; i < Mathf.Min(hotbarSlots.Length, 9); i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + 1))
            {
                currentSlot = i;
                UpdateSelection();
                break;
            }
        }
    }

    public bool AddItemToCurrentSlot(Item item)
    {
        Debug.Log($"Trying to add {item.itemName} to slot {currentSlot}");

        // Try current slot first
        if (hotbarSlots[currentSlot].TryAddItem(item))
        {
            Debug.Log($"Added {item.itemName} to current slot {currentSlot}");
            return true;
        }

        // If current slot is full, find first empty slot
        for (int i = 0; i < hotbarSlots.Length; i++)
        {
            if (hotbarSlots[i].TryAddItem(item))
            {
                Debug.Log($"Added {item.itemName} to slot {i}");
                return true;
            }
        }

        Debug.Log("Hotbar is full!");
        return false;
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

    private void UpdateSelection()
    {
        Debug.Log($"Updating selection to slot {currentSlot}");

        for (int i = 0; i < hotbarSlots.Length; i++)
        {
            hotbarSlots[i].SetSelected(i == currentSlot);
        }
    }
}
