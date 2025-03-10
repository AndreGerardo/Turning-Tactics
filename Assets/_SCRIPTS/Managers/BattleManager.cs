using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public enum BattleStartType
{
    PLAYER_NORMAL,
    PLAYER_ADVANTAGE,
    ENEMY_ADVANTAGE
}

public class BattleManager : MonoBehaviour
{
    [Header("BATTLE CONFIGURATION")]
    [SerializeField] private GameObject basicAttackEffectPrefab;
    [SerializeField] private GameObject basicDefendEffectPrefab;
    [SerializeField] private GameObject basicRunEffectPrefab;
    [SerializeField] private float effectDuration = 0.1f;
    [SerializeField] private Vector3 effectPosOffset = Vector3.zero;
    private bool isBattleStarted = false;

    [Header("BATTLE AREA REFERENCE")]
    [SerializeField] private GameObject battleCamera;
    [SerializeField] private Transform targetPlayerPos;
    [SerializeField] private Transform targetEnemyPos;
    private Vector3 lastPlayerPos;
    private Vector3 lastEnemyPos;

    [Header("TURN CONFIGURATION")]
    private Character_Player currentPlayer;
    private float playerSpeed;
    private int playerDefense = 0;

    private Character_Enemy currentEnemy;
    private float enemySpeed;

    private void Awake()
    {
        BattleEvent.OnSetupBattle += SetupBattle;

        BattleEvent.OnPlayerAttack += OnPlayerAttack;
        BattleEvent.OnPlayerSpell += OnPlayerSpell;
        BattleEvent.OnPlayerDefend += OnPlayerDefend;
        BattleEvent.OnPlayerRun += OnPlayerRun;
    }

    private void OnDestroy()
    {
        BattleEvent.OnSetupBattle -= SetupBattle;

        BattleEvent.OnPlayerAttack -= OnPlayerAttack;
        BattleEvent.OnPlayerSpell -= OnPlayerSpell;
        BattleEvent.OnPlayerDefend -= OnPlayerDefend;
        BattleEvent.OnPlayerRun -= OnPlayerRun;
    }

    private void Update()
    {
        LockCharacterPosition(currentPlayer, currentEnemy);
    }

    private void SetupBattle(BattleStartType battleStartType, Character_Player player, Character_Enemy enemy)
    {
        if (isBattleStarted) return;
        isBattleStarted = true;

        lastPlayerPos = player.transform.position;
        lastEnemyPos = enemy.transform.position;

        currentPlayer = player;
        currentEnemy = enemy;
        LockCharacterPosition(player, enemy);

        battleCamera.SetActive(true);

        playerSpeed = currentPlayer.GetStats().Speed;
        enemySpeed = currentEnemy.GetStats().Speed;

        switch (battleStartType)
        {
            case BattleStartType.PLAYER_NORMAL:
                BattleEvent.OnDisplayBattleMessage?.Invoke($"The battle starts.");
                break;

            case BattleStartType.PLAYER_ADVANTAGE:
                playerSpeed *= 1.5f;
                BattleEvent.OnDisplayBattleMessage?.Invoke($"You damaged the {currentEnemy.charName} first!");
                currentEnemy.ReduceHealth(currentPlayer.GetStats().Attack);
                break;

            case BattleStartType.ENEMY_ADVANTAGE:
                playerSpeed *= 0.5f;
                BattleEvent.OnDisplayBattleMessage?.Invoke($"{currentEnemy.charName} attacked you first!");
                currentPlayer.ReduceHealth(currentEnemy.GetStats().Attack);
                break;
        }

        EndTurn();
        DOVirtual.DelayedCall(2f, () => StartBattle());

        Debug.Log($"STARTED BATTLE WITH TYPE : {battleStartType}");
        BattleEvent.OnStartBattle?.Invoke();
    }

    private void StartBattle()
    {
        BattleEvent.OnDisplayBattleMessage?.Invoke(string.Empty);
        EndTurn();
        if (playerSpeed >= enemySpeed)
        {
            PlayerTurn();
        }
        else
        {
            StartCoroutine(EnemyTurn());
        }
    }

    private void FinishBattle()
    {
        if (!isBattleStarted) return;

        isBattleStarted = false;
        battleCamera.SetActive(false);

        currentPlayer.SetInvulnerable(3f);
        currentPlayer.SetPosition(lastPlayerPos);

        currentEnemy.SetInvulnerable(3f);
        currentEnemy.SetPosition(lastEnemyPos);

        BattleEvent.OnFinishBattle?.Invoke();
    }

    private void LockCharacterPosition(Character_Player player, Character_Enemy enemy)
    {
        if (!isBattleStarted) return;

        player.SetPosition(targetPlayerPos.position);
        player.SetCharFacing(true);
        enemy.SetPosition(targetEnemyPos.position);
        enemy.SetCharFacing(false);

    }

    private void WinBattle()
    {
        BattleEvent.OnDisplayBattleMessage?.Invoke("You won the battle!");
        BattleEvent.OnBattleWin?.Invoke();
        DOVirtual.DelayedCall(2f, () => FinishBattle());
    }

    private void LoseBattle()
    {
        BattleEvent.OnDisplayBattleMessage?.Invoke("You lose the battle!");
        BattleEvent.OnBattleLose?.Invoke();
        DOVirtual.DelayedCall(2f, () => FinishBattle());
    }

    private void PlayEffectAtPosition(GameObject vfxPrefab, Vector3 targetPos)
    {
        GameObject obj = Instantiate(vfxPrefab);
        obj.transform.position = targetPos;
        obj.transform.rotation = Quaternion.identity;

        Destroy(obj, effectDuration);

    }


    #region TURNS

    private void PlayerTurn()
    {
        BattleEvent.OnPlayerTurn?.Invoke();
    }

    private IEnumerator PlayerAttack()
    {
        BattleEvent.OnDisplayBattleMessage?.Invoke("You attacked!");
        currentEnemy.ReduceHealth(currentPlayer.GetStats().Attack);
        EndTurn();

        PlayEffectAtPosition(basicAttackEffectPrefab, targetEnemyPos.position + effectPosOffset);


        yield return new WaitForSeconds(2f);

        BattleEvent.OnDisplayBattleMessage?.Invoke(string.Empty);
        StartCoroutine(EnemyTurn());

    }

    private IEnumerator PlayerSpell()
    {
        Spell spell = currentPlayer.GetStats().Spell;
        BattleEvent.OnDisplayBattleMessage?.Invoke($"You used {spell.SpellName}!");
        currentEnemy.ReduceHealth(spell.Damage);
        EndTurn();

        PlayEffectAtPosition(spell.EffectPrefab, targetEnemyPos.position + effectPosOffset);

        yield return new WaitForSeconds(2f);

        BattleEvent.OnDisplayBattleMessage?.Invoke(string.Empty);
        StartCoroutine(EnemyTurn());
    }

    private IEnumerator PlayerDefend()
    {
        BattleEvent.OnDisplayBattleMessage?.Invoke("You made yourself sturdy!");
        playerDefense = currentPlayer.GetStats().Defend;
        EndTurn();

        PlayEffectAtPosition(basicDefendEffectPrefab, targetPlayerPos.position + effectPosOffset);

        yield return new WaitForSeconds(2f);

        BattleEvent.OnDisplayBattleMessage?.Invoke(string.Empty);
        StartCoroutine(EnemyTurn());

    }

    private IEnumerator PlayerRun()
    {
        BattleEvent.OnDisplayBattleMessage?.Invoke("You ran for your life.");
        currentPlayer.SetCharFacing(false);

        PlayEffectAtPosition(basicRunEffectPrefab, targetPlayerPos.position + effectPosOffset);

        yield return new WaitForSeconds(2f);

        BattleEvent.OnDisplayBattleMessage?.Invoke(string.Empty);
        FinishBattle();

    }

    private IEnumerator EnemyTurn()
    {
        BattleEvent.OnEnemyTurn?.Invoke();

        float rdm = UnityEngine.Random.Range(0f, 1f);

        if(rdm >= 0.5f)
        {
            BattleEvent.OnDisplayBattleMessage?.Invoke($"{currentEnemy.charName} attacked!");
            currentPlayer.ReduceHealth(currentEnemy.GetStats().Attack - playerDefense);

            PlayEffectAtPosition(basicAttackEffectPrefab, targetPlayerPos.position + effectPosOffset);

        }
        else
        {
            Spell spell = currentEnemy.GetStats().Spell;
            BattleEvent.OnDisplayBattleMessage?.Invoke($"{currentEnemy.charName} used {spell.SpellName}!");
            currentPlayer.ReduceHealth(spell.Damage - playerDefense);

            PlayEffectAtPosition(spell.EffectPrefab, targetPlayerPos.position + effectPosOffset);

        }
        
        playerDefense = 0;
        EndTurn();

        yield return new WaitForSeconds(2f);

        BattleEvent.OnDisplayBattleMessage?.Invoke(string.Empty);

        if (currentEnemy.GetCurrentHealth() > 0)
        { 
            PlayerTurn();
        }
            

    }

    private void EndTurn()
    {
        BattleEvent.OnEndTurn?.Invoke(currentPlayer, currentEnemy);

        if (currentPlayer.GetCurrentHealth() <= 0)
        {
            StopCoroutine(EnemyTurn());
            LoseBattle();
        }
        else if(currentEnemy.GetCurrentHealth() <= 0)
        {
            StopCoroutine(PlayerAttack());
            StopCoroutine(PlayerSpell());
            StopCoroutine(EnemyTurn());
            WinBattle();
        }

    }

    #endregion

    #region EVENT HANDLER

    private void OnPlayerAttack()
    {
        StartCoroutine(PlayerAttack());
    }

    private void OnPlayerSpell()
    {
        StartCoroutine(PlayerSpell());
    }

    private void OnPlayerDefend()
    {
        StartCoroutine(PlayerDefend());
    }

    private void OnPlayerRun()
    {
        StartCoroutine(PlayerRun());
    }

    #endregion

}

public static class BattleEvent
{
    public static Action<BattleStartType, Character_Player, Character_Enemy> OnSetupBattle;
    public static Action<Character_Player, Character_Enemy> OnEndTurn;

    public static Action OnStartBattle;
    public static Action OnFinishBattle;
    public static Action OnBattleWin;
    public static Action OnBattleLose;

    public static Action<string> OnDisplayBattleMessage;
    public static Action OnPlayerTurn;
    public static Action OnEnemyTurn;

    public static Action OnPlayerAttack;
    public static Action OnPlayerSpell;
    public static Action OnPlayerDefend;
    public static Action OnPlayerRun;

}

