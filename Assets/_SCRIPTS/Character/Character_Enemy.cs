using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character_Enemy : Character
{
    [Header("ENEMY CONFIGURATION")]
    [SerializeField] private Collider2D AttackHitboxCollider;
    [SerializeField] private float attackRange = 1f;

    public int poolIndex { get; set; }

    private bool isChasing = false;
    private Character_Player targetPlayer;

    [Header("PATROL CONFIGURATION")]
    [SerializeField] private float maxPatrolDuration = 5f;

    private bool isPatrolling;

    protected override void OnUpdate()
    {

        base.OnUpdate();

        SetRandomPatrol();
        ChasePlayer();
    }

    private void ChasePlayer()
    {
        if(!isChasing || targetPlayer.IsDead) return;

        //float step = baseStats.Speed * Time.deltaTime;
        //targetDirection = Vector3.MoveTowards(targetPlayer.transform.position, transform.position, step
        targetDirection = targetPlayer.transform.position - transform.position;

        if(targetDirection.sqrMagnitude <= attackRange)
        {
            StopCharacterMovement();
            StartCoroutine(StartAttack());
        }

        targetDirection.Normalize();

    }

    public override void SetPosition(Vector3 pos)
    {
        StopCoroutine(StartAttack());
        base.SetPosition(pos);
    }

    public void SetChaseTarget(Character_Player player)
    {
        isChasing = true;

        targetPlayer = player;
    }

    public void StopChasingTarget(Character_Player player)
    {
        isChasing = false;

        StopCharacterMovement();
        targetPlayer = null;
    }

    private void SetRandomPatrol()
    {
        if (isChasing)
        {
            isPatrolling = false;
            StopCharacterMovement();
            return;
        }
        if (isPatrolling) return;
        isPatrolling = true;

        targetDirection = new Vector2(Random.Range(-1, 1), Random.Range(-1, 1));
        targetDirection.Normalize();

        float patrolDuration = Random.Range(3f, maxPatrolDuration);
        DOVirtual.DelayedCall(patrolDuration, () =>
        {
            StopCharacterMovement();
            isPatrolling = false;
        });
    }

    private IEnumerator StartAttack()
    {
        SetCharacterIdleState(true);
        Vector3 lastCharPos = targetPlayer.transform.position;
        yield return new WaitForSeconds(baseStats.AttackDelay);

        transform.DOMove(lastCharPos, 0.25f);

        yield return new WaitForSeconds(baseStats.AttackDelay);

        SetCharacterIdleState(false);
    }


    public override void TakeDamage(Character damageSource)
    {
        //base.TakeDamage(damageSource);
        if (isInvulnerable) return;

        if (targetPlayer != null)
        {
            if(damageSource.TryGetComponent<Character_Player>(out Character_Player player))
                BattleEvent.OnSetupBattle?.Invoke(BattleStartType.PLAYER_NORMAL, player, this);
        }
        else
        {
            if (damageSource.TryGetComponent<Character_Player>(out Character_Player player))
                BattleEvent.OnSetupBattle?.Invoke(BattleStartType.PLAYER_ADVANTAGE, player, this);
        }

    }

    public override void ReduceHealth(int amount)
    {
        base.ReduceHealth(amount);

        if (currentHealth <= 0)
        {
            EnemyEvent.OnEnemyDie?.Invoke();
            gameObject.SetActive(false);
        }
    }

}
