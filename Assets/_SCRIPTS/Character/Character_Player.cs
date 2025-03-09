using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character_Player : Character
{
    [Header("PLAYER CONFIGURATION")]
    [SerializeField] private Interactor interactor;

    protected override void OnUpdate()
    {
        base.OnUpdate();

        if (!isControllable) return;

        MoveInput();

        if(Input.GetMouseButtonDown(0))
        {
            Attack();
        }

        if(Input.GetKeyDown(KeyCode.F) && canMove)
        {
            interactor.Interact();
        }
    }

    private void MoveInput()
    {
        targetDirection.x = Input.GetAxisRaw("Horizontal");
        targetDirection.y = Input.GetAxisRaw("Vertical");
        targetDirection.Normalize();

    }

    public override void TakeDamage(Character damageSource)
    {
        //base.TakeDamage(damageSource);
        if (isInvulnerable) return;

        BattleEvent.OnSetupBattle?.Invoke(BattleStartType.ENEMY_ADVANTAGE, this, damageSource as Character_Enemy);

    }

    public override void ReduceHealth(int amount)
    {
        base.ReduceHealth(amount);

    }

}
