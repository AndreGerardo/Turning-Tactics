using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour, IAttackable
{
    [field: Header("GENERAL CONFIGURATION")]
    [field: SerializeField] public string charName { get; private set; }
    [SerializeField] protected Stats baseStats;
    [SerializeField] protected Animator anim;
    [SerializeField] protected AnimationEventHandler animEventHandler;
    [SerializeField] protected Rigidbody2D rb;
    [SerializeField] protected AttackHitBox attackHitBox;
    [Space]
    [SerializeField] protected bool isFacingRight = true;
    [SerializeField] protected bool canMove = true;

    protected Vector2 targetDirection = Vector2.zero;
    public bool IsDead = false;
    protected bool isInvulnerable = false;
    protected bool isControllable = true;

    [Header("HEALTH")]
    protected int currentHealth;


    private void Awake()
    {
        CharacterEvent.OnSetPlayerIdleState += SetCharacterIdleState;
        
        BattleEvent.OnStartBattle += DisableCharacterMovement;
        BattleEvent.OnFinishBattle += EnableCharacterMovement;

        attackHitBox.Init(this);

        currentHealth = baseStats.MaxHealth;
    }

    private void OnDestroy()
    {
        CharacterEvent.OnSetPlayerIdleState -= SetCharacterIdleState;

        BattleEvent.OnStartBattle -= DisableCharacterMovement;
        BattleEvent.OnFinishBattle -= EnableCharacterMovement;
    }

    private void Start()
    {
        InitAnimEvents();
    }

    private void Update()
    {
        if(IsDead) return;

        OnUpdate();
    }

    private void FixedUpdate()
    {
        OnFixedUpdate();
    }

    protected virtual void OnUpdate()
    {
        UpdateAnimationParams();
    }

    protected virtual void OnFixedUpdate()
    {
        if (canMove && !IsDead) MoveCharacter();
    }


    private void MoveCharacter()
    {
        rb.velocity = baseStats.MoveSpeed * targetDirection * Time.fixedDeltaTime;

        FlipPlayer(targetDirection.x);
    }

    protected void FlipPlayer(float horizontalMove)
    {
        if (isFacingRight && horizontalMove < 0f || !isFacingRight && horizontalMove > 0f)
        {
            isFacingRight = !isFacingRight;
            Vector3 localScale = transform.localScale;
            localScale.x *= -1f;
            transform.localScale = localScale;
        }
    }

    protected void InitAnimEvents()
    {
        animEventHandler.AddEvent("DisableMovement", () => SetCharacterIdleState(true));
        animEventHandler.AddEvent("EnableMovement", () => SetCharacterIdleState(false));
    }

    protected void UpdateAnimationParams()
    {
        if (anim.parameterCount <= 0) return;
            anim.SetBool("walk", rb.velocity.magnitude > 0.1f);
            anim.SetBool("idle", rb.velocity.magnitude <= 0.1f);
    }

    protected void SetCharacterIdleState(bool isIdle)
    {
        canMove = !isIdle;

        if(isIdle) StopCharacterMovement();
    }

    private void EnableCharacterMovement()
    {
        SetCharacterIdleState(false);
        isControllable = true;
    }

    private void DisableCharacterMovement()
    {
        SetCharacterIdleState(true);
        isControllable = false;
    }

    public virtual void SetPosition(Vector3 pos)
    {
        DOTween.Kill(transform);
        StopCharacterMovement();
        transform.position = pos;
        //rb.MovePosition(pos);
    }

    public void SetInvulnerable(float duration)
    {
        isInvulnerable = true;
        DOVirtual.DelayedCall(duration, () => isInvulnerable = false);
    }

    public void SetCharFacing(bool isRight)
    {
        FlipPlayer(isRight ? 1f : -1f);
    }

    protected void StopCharacterMovement()
    {
        rb.velocity = Vector2.zero;
    }

    protected void Attack()
    {
        if (!canMove) return;

        anim.SetTrigger("attack");
    }

    public virtual void TakeDamage(Character damageSource)
    {
        if (isInvulnerable) return;
        //TODO: Start TurnBasedCombat
        Debug.Log($"{gameObject.name} has taken damage from {damageSource.gameObject.name}.");
    }

    public virtual void ReduceHealth(int amount)
    {
        currentHealth -= amount;

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            IsDead = true;
        }

    }

    public void ReviveCharacter()
    {
        IsDead = false;
        currentHealth = baseStats.MaxHealth;
    }

    public int GetCurrentHealth()
    {
        return currentHealth;
    }

    public Stats GetStats()
    {
        return baseStats;
    }

}

public static class CharacterEvent
{
    public static Action<bool> OnSetPlayerIdleState;
}
