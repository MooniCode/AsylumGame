using UnityEngine;

public class PlayerPickup : MonoBehaviour
{
    private Camera _playerCamera;
    private LayerMask _layerMask;
    private PickupableItem _currentPickupable;

    private void Start()
    {
        _layerMask = LayerMask.GetMask("Item");
        _playerCamera = GetComponentInChildren<Camera>();

        InputManager.Instance.OnInteractInput += HandleInteractInput;
        InputManager.Instance.OnDropInput += HandleDropInput;
    }

    private void Update()
    {
        RaycastHit hit;

        if (Physics.Raycast(_playerCamera.transform.position, _playerCamera.transform.forward, out hit, 5f, _layerMask))
        {
            PickupableItem pickupable = hit.collider.GetComponent<PickupableItem>();

            if (pickupable != null && _currentPickupable != pickupable)
            {
                _currentPickupable?.OnLookEnd();
                _currentPickupable = pickupable;
                _currentPickupable.OnLookStart();
            }
        }
        else if (_currentPickupable != null)
        {
            _currentPickupable.OnLookEnd();
            _currentPickupable = null;
        }
    }

    private void HandleInteractInput()
    {
        if (_currentPickupable != null)
        {
            _currentPickupable.OnPickup(gameObject);
            _currentPickupable = null; // Clear the current pickupable after picking it up
        }
    }

    private void HandleDropInput()
    {
        PlayerInventoryManager inventoryManager = GetComponent<PlayerInventoryManager>();
        if (inventoryManager != null && inventoryManager.HasCurrentItem())
        {
            inventoryManager.DropCurrentItem();
        }
        else
        {
            Debug.Log("No item to drop");
        }
    }

    private void OnDestroy()
    {
        if (InputManager.Instance != null)
        {
            InputManager.Instance.OnInteractInput -= HandleInteractInput;
        }
    }
}
