using NUnit.Framework;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;

[CreateAssetMenu(fileName = "itemDatabase", menuName = "Inventory/ItemDatabase")]
public class ItemDatabase : ScriptableObject
{
    [System.Serializable]
    public class ItemData
    {
        public string itemName;
        public Sprite itemIcon;
        public GameObject worldPrefab;
    }

    public static ItemDatabase Instance;
    public List<ItemData> allItems;

    private void OnEnable()
    {
        Instance = this;
    }

    public Item GetItemByName(string itemName)
    {
        ItemData data = allItems.Find(item => item.itemName == itemName);
        if (data != null)
        {
            SimpleItem item = new SimpleItem();
            item.itemName = itemName;
            item.icon = data.itemIcon;
            item.worldPrefab = data.worldPrefab;
            return item;
        }
        return null;
    }
}
