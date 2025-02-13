using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//deprecated
public class PlayerInteract : MonoBehaviour
{
    float interactRange = 1f;

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
