using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

public class WorldItemManager : MonoBehaviour
{
    public static WorldItemManager Instance { get; private set; }

    [Header("World Item Settings")]
    [SerializeField] private LayerMask itemLayer = -1;

    // Keep track of all the world items
    private List<WorldItem> worldItems = new List<WorldItem>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Find all existing world items in the scene
        FindExistingWorldItems();
    }

    private void FindExistingWorldItems()
    {
        // Find all objects with the WorldItem component
        WorldItem[] existingItems = FindObjectsByType<WorldItem>(FindObjectsSortMode.None);
        foreach (WorldItem item in existingItems)
        {
            RegisterWorldItem(item);
        }
    }

    public void RegisterWorldItem(WorldItem item)
    {
        if (!worldItems.Contains(item))
        {
            worldItems.Add(item);
            Debug.Log($"Registered world item {item.ItemId}");
        }
    }

    public void UnregisterWorldItem(WorldItem item)
    {
        if (worldItems.Contains(item))
        {
            worldItems.Remove(item);
            Debug.Log($"Unregister world item: {item.ItemId}");
        }
    }

    public GameObject SpawnWorldItem(string itemId, Vector3 position, Vector3 rotation)
    {
        // Get item data from database
        if (ItemDatabase.Instance == null)
        {
            Debug.LogError("ItemDatabase.Instance is null");
            return null;
        }

        ItemDatabase.ItemData itemData = ItemDatabase.Instance.allItems.Find(item => item.itemName == itemId);
        if (itemData == null || itemData.worldPrefab == null)
        {
            Debug.LogError($"Could not find world prefab for item: {itemId}");
            return null;
        }

        // Spawn the item
        GameObject spawnedItem = Instantiate(itemData.worldPrefab, position, Quaternion.Euler(rotation));

        // Make sure it has a WorldItem component
        WorldItem worldItemComponent = spawnedItem.GetComponent<WorldItem>();
        if (worldItemComponent == null)
        {
            worldItemComponent = spawnedItem.AddComponent<WorldItem>();
        }

        // Set the item ID
        worldItemComponent.SetItemId(itemId);

        // Register it
        RegisterWorldItem(worldItemComponent);

        return spawnedItem;
    }

    public List<ItemSpawn> getAllWorldItemData()
    {
        List<ItemSpawn> itemsData = new List<ItemSpawn>();

        foreach (WorldItem item in worldItems)
        {
            if (item != null && item.gameObject != null)
            {
                ItemSpawn itemSpawn = new ItemSpawn(item.ItemId, item.transform.position, item.transform.eulerAngles);
                itemsData.Add(itemSpawn);
            }
        }

        return itemsData;
    }

    public void ClearAllWorldItems()
    {
        // Create a copy of the list to avoid modification during iteration
        List<WorldItem> itemsToDestroy = new List<WorldItem>(worldItems);

        foreach (WorldItem item in itemsToDestroy)
        {
            if (item != null && item.gameObject != null)
            {
                Destroy(item.gameObject);
            }
        }

        worldItems.Clear();
        Debug.Log("Cleared all world items");
    }

    public void LoadWorldItem(List<ItemSpawn> itemSpawns)
    {
        // Clear existing items first
        ClearAllWorldItems();

        // Spawn all saved items
        foreach (ItemSpawn itemSpawn in itemSpawns)
        {
            SpawnWorldItem(itemSpawn.itemId, itemSpawn.position, itemSpawn.rotation);
        }

        Debug.Log($"Loaded {itemSpawns.Count} world items");
    }
}
