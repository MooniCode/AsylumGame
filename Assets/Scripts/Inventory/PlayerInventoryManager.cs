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
            }
        }
    }

    private void SpawnDroppedItem(Item item)
    {
        if (item.worldPrefab == null)
        {
            Debug.LogWarning($"Item {item.itemName} does not have a world prefab to drop.");
            return;
        }

        LayerMask groundLayer = LayerMask.GetMask("Ground", "Building");

        if (Physics.Raycast(playerCamera.transform.position, Vector3.down, out RaycastHit hit, Mathf.Infinity, groundLayer))
        {
            Vector3 groundNormal = hit.normal;

            // Get bounds from a temporary object
            GameObject tempObject = Instantiate(item.worldPrefab, Vector3.zero, Quaternion.identity);
            tempObject.SetActive(true);
            Bounds bounds = GetObjectBounds(tempObject);
            DestroyImmediate(tempObject);

            // Create the actual dropped object
            GameObject droppedObject = Instantiate(item.worldPrefab, hit.point, Quaternion.identity);

            // Align to ground and apply player rotation
            AlignItemToGroundAndPlayer(droppedObject, groundNormal);

            // Position above ground surface
            float bottomOffset = bounds.extents.y;
            Vector3 finalPosition = hit.point + (groundNormal * bottomOffset);
            droppedObject.transform.position = finalPosition;

            droppedObject.SetActive(true);

            // Configure the dropped item
            SimplePickupable pickupable = droppedObject.GetComponent<SimplePickupable>();
            if (pickupable != null)
            {
                pickupable.SetItemData(item.itemName, item.icon);
            }

            // Register with WorldItemManager for saving
            WorldItem worldItem = droppedObject.GetComponent<WorldItem>();
            if (worldItem == null)
            {
                worldItem = droppedObject.AddComponent<WorldItem>();
            }

            worldItem.SetItemId(item.itemName);
            if (WorldItemManager.Instance != null)
            {
                WorldItemManager.Instance.RegisterWorldItem(worldItem);
            }
        }
        else
        {
            Debug.LogWarning("No ground found beneath player");
        }
    }

    private Bounds GetObjectBounds(GameObject obj)
    {
        // Try collider first
        Collider col = obj.GetComponent<Collider>();
        if (col != null && col.enabled)
        {
            return col.bounds;
        }

        // Try child renderers
        Renderer[] childRenderers = obj.GetComponentsInChildren<Renderer>();
        if (childRenderers.Length > 0)
        {
            Bounds combinedBounds = childRenderers[0].bounds;
            for (int i = 1; i < childRenderers.Length; i++)
            {
                combinedBounds.Encapsulate(childRenderers[i].bounds);
            }
            return combinedBounds;
        }

        // Fallback
        return new Bounds(obj.transform.position, Vector3.one * 0.5f);
    }

    private void AlignItemToGroundAndPlayer(GameObject droppedObject, Vector3 groundNormal)
    {
        // Find the CameraRoot child object
        Transform cameraRoot = transform.Find("CameraRoot");

        Vector3 playerForwardDirection = Vector3.forward;
        if (cameraRoot != null)
        {
            // Get the actual forward direction the player is facing
            playerForwardDirection = cameraRoot.forward;
        }
        else
        {
            Debug.LogWarning("CameraRoot not found! Using camera's parent forward as fallback.");
            playerForwardDirection = playerCamera.transform.parent.forward;
        }

        // Project the player's forward direction onto the ground plane
        Vector3 projectedForward = Vector3.ProjectOnPlane(playerForwardDirection, groundNormal).normalized;

        // Create rotation using LookRotation (more robust than Euler angles)
        if (projectedForward != Vector3.zero)
        {
            droppedObject.transform.rotation = Quaternion.LookRotation(projectedForward, groundNormal);
        }
        else
        {
            // Fallback if projection results in zero vector
            Quaternion groundAlignment = Quaternion.FromToRotation(Vector3.up, groundNormal);
            droppedObject.transform.rotation = groundAlignment;
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