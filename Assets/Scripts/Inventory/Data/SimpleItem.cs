using UnityEngine;

[System.Serializable]
public class SimpleItem : Item
{
    public override void Use()
    {
        Debug.Log($"Used {itemName}");
    }
}
