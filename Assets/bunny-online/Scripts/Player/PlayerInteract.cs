using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using UnityEngine;
using UnityEngine.InputSystem;

//deprecated
public class PlayerInteract : NetworkBehaviour
{
    float interactRange = 1f;

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (!IsOwner) return;
        TryInteract();
    }

    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            TryInteract();
        }
    }

    public void TryInteract()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, interactRange);
        foreach (var collider in colliders)
        {
            if (collider.TryGetComponent(out IInteractable interactable))
            {
                Debug.Log(collider.gameObject);
                interactable.Interact();
            }
        }
    }
}
