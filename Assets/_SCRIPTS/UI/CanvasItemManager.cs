using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasItemManager : MonoBehaviour
{
    [Header("ITEM REFERENCE")]
    [SerializeField] private Image[] itemImages;


    private void Awake()
    {
        ChestEvent.OnGetChestReward += SetInventoryItem;
    }

    private void OnDestroy()
    {
        ChestEvent.OnGetChestReward -= SetInventoryItem;
    }

    private void SetInventoryItem(Sprite icon, int index)
    {
        itemImages[index].sprite = icon;
        itemImages[index].gameObject.SetActive(true);
    }

}

