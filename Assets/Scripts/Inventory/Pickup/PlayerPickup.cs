using UnityEngine;

public class PlayerPickup : MonoBehaviour
{
    private Camera _playerCamera;
    private LayerMask _layerMask;
    private GameObject _currentTarget;
    private PickupableItem _currentPickupable;

    private void Start()
    {
        _layerMask = LayerMask.GetMask("Item");
        _playerCamera = GetComponentInChildren<Camera>();
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

            if (Input.GetKeyDown(KeyCode.E) && _currentPickupable != null)
            {
                _currentPickupable.OnPickup(gameObject);
                _currentPickupable = null;
            }
        }
        else if (_currentPickupable != null)
        {
            _currentPickupable.OnLookEnd();
            _currentPickupable = null;
        }
    }
}
