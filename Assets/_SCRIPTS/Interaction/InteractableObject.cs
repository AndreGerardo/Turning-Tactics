using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InteractableObject : MonoBehaviour, IInteractable
{
    // Start is called before the first frame update
    [SerializeField] private GameObject interactKeyObj;
    [SerializeField] private UnityEvent OnInteractEvent;

    public void Interact()
    {
        OnInteractEvent?.Invoke();
        HideInteractKeyUI();
    }

    public void ShowInteractKeyUI()
    {
        interactKeyObj.SetActive(true);
    }

    public void HideInteractKeyUI()
    {
        interactKeyObj.SetActive(false);
    }
}
