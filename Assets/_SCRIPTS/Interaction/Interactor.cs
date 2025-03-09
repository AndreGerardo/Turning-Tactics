using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactor : MonoBehaviour
{
    private InteractableObject interactableObject;
    private Character character;

    private HashSet<InteractableObject> interactableObjectCollection = new HashSet<InteractableObject>();

    private void Awake()
    {
        character = GetComponent<Character_Player>();
    }

    private void FixedUpdate()
    {
        try
        {
            FindNearestInteractableObject();
        }
        catch (System.Exception)
        {

        }
        
    }

    public void Interact()
    {
        if (interactableObject == null) return;

        interactableObject.Interact();
    }

    private void FindNearestInteractableObject()
    {

        float minDistance = float.MaxValue;
        InteractableObject closest = null;


        foreach (var interactObj in interactableObjectCollection)
        {
            float distance = Vector3.Distance(transform.position, interactObj.gameObject.transform.position);
            if (distance > minDistance) continue;
            minDistance = distance;
            closest = interactObj;
        }

        if (closest == interactableObject) return;

        interactableObject?.HideInteractKeyUI();
        interactableObject = closest;
        interactableObject?.ShowInteractKeyUI();
    }

    private void OnDisable()
    {
        if (interactableObject != null)
            interactableObject.HideInteractKeyUI();
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<InteractableObject>(out InteractableObject interactable))
        {

            if (interactableObjectCollection.Contains(interactable))
            {
                //Debug.LogWarning($"[InteractableController] TriggerEnter on a preexisting collider {other.gameObject.name}");
                return;
            }
            interactableObjectCollection.Add(interactable);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.TryGetComponent<InteractableObject>(out InteractableObject interactable))
        {
            if (interactable)
            {
                interactableObjectCollection.Remove(interactable);
            }
        }
    }
}
