using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CanvasBattleMenuManager : MonoBehaviour
{
    [Header("BATTLE MENU REFERENCE")]
    [SerializeField] private CanvasGroup battleMenuCG;
    [SerializeField] private CanvasHealthBarUI playerHealthBarUI;
    [SerializeField] private CanvasHealthBarUI enemyHealthBarUI;
    [SerializeField] private TMP_Text battleMessageText;

    [Header("PLAYER TURN MENU REFERENCE")]
    [SerializeField] private GameObject playerTurnMenuPanelCG;
    [SerializeField] private Button buttonPlayerAttack;
    [SerializeField] private Button buttonPlayerSpell;
    [SerializeField] private Button buttonPlayerDefend;
    [SerializeField] private Button buttonPlayerRun;


    private void Awake()
    {
        BattleEvent.OnStartBattle += ShowBattleMenuUI;
        BattleEvent.OnFinishBattle += HideBattleMenuUI;
        BattleEvent.OnSetupBattle += SetBattleMenuUI;
        BattleEvent.OnEndTurn += RefreshHealth;
        BattleEvent.OnDisplayBattleMessage += SetBattleMessage;
        BattleEvent.OnPlayerTurn += ShowPlayerTurnUI;
        BattleEvent.OnEnemyTurn += HidePlayerTurnUI;

        buttonPlayerAttack.onClick.AddListener(() => BattleEvent.OnPlayerAttack?.Invoke());
        buttonPlayerSpell.onClick.AddListener(() => BattleEvent.OnPlayerSpell?.Invoke());
        buttonPlayerDefend.onClick.AddListener(() => BattleEvent.OnPlayerDefend?.Invoke());
        buttonPlayerRun.onClick.AddListener(() => BattleEvent.OnPlayerRun?.Invoke());

    }

    private void OnDestroy()
    {
        BattleEvent.OnStartBattle -= ShowBattleMenuUI;
        BattleEvent.OnFinishBattle -= HideBattleMenuUI;
        BattleEvent.OnSetupBattle -= SetBattleMenuUI;
        BattleEvent.OnEndTurn -= RefreshHealth;
        BattleEvent.OnDisplayBattleMessage -= SetBattleMessage;
        BattleEvent.OnPlayerTurn -= ShowPlayerTurnUI;
        BattleEvent.OnEnemyTurn -= HidePlayerTurnUI;

    }

    private void ShowBattleMenuUI()
    {
        battleMenuCG.alpha = 1f;
        battleMenuCG.interactable = true;
        battleMenuCG.blocksRaycasts = true;
    }

    private void HideBattleMenuUI()
    {
        battleMenuCG.alpha = 0f;
        battleMenuCG.interactable = false;
        battleMenuCG.blocksRaycasts = false;

        HidePlayerTurnUI();

    }

    private void SetBattleMenuUI(BattleStartType battleStartType, Character_Player player, Character_Enemy enemy)
    {
        playerHealthBarUI.SetCurrentHealthBar(player.GetCurrentHealth(), player.GetStats().MaxHealth);
        enemyHealthBarUI.SetCurrentHealthBar(enemy.GetCurrentHealth(), enemy.GetStats().MaxHealth);

        HidePlayerTurnUI();
    }

    private void RefreshHealth(Character_Player player, Character_Enemy enemy)
    {
        playerHealthBarUI.SetCurrentHealthBar(player.GetCurrentHealth(), player.GetStats().MaxHealth);
        enemyHealthBarUI.SetCurrentHealthBar(enemy.GetCurrentHealth(), enemy.GetStats().MaxHealth);
    }

    private void SetBattleMessage(string message)
    {
        battleMessageText.SetText(message);
    }


    private void ShowPlayerTurnUI()
    {
        playerTurnMenuPanelCG.SetActive(true);
    }

    private void HidePlayerTurnUI()
    {
        playerTurnMenuPanelCG.SetActive(false);
    }

}
