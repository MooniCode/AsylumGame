using UnityEngine;

[System.Serializable]
public abstract class Item
{
    public string itemName;
    public Sprite icon;
    public GameObject worldPrefab;

    public abstract void Use();
    public virtual string getStatusText() => "";
}
