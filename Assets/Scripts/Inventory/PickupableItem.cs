using UnityEngine;

public abstract class PickupableItem : MonoBehaviour
{
    [SerializeField] protected string pickupPrompt = "Press E to pickup";

    public abstract string ItemName { get; }
    public abstract void OnPickup(GameObject player);
    public virtual void OnLookStart() { }
    public virtual void OnLookEnd() { }
}
