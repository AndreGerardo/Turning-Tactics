using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractPrompt_Chest : InteractPrompt
{

    [Header("CHEST CONFIGURATION")]
    [SerializeField] protected Sprite ChestRewardIcon;
    [SerializeField] protected int ChestRewardIndex = 0;

    private bool isChestOpened = false;


    public override void ShowPrompt()
    {
        if (isChestOpened) return;

        base.ShowPrompt();

        OpenChest();

    }

    private void OpenChest()
    {
        if(isChestOpened) return;

        isChestOpened = true;

        ChestEvent.OnGetChestReward?.Invoke(ChestRewardIcon, ChestRewardIndex);

        gameObject.SetActive(false);
    }
}

public static class ChestEvent
{
    public static Action<Sprite, int> OnGetChestReward;
}
