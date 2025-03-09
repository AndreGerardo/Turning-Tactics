using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New Stats", menuName = "ScriptableObjects/Stats/New Stats")]
public class Stats : ScriptableObject
{
    [Header("MOVEMENT")]
    public float MoveSpeed;

    [Header("BATTLE")]
    public int MaxHealth;
    public float Speed;
    public float Accuracy;
    public int Attack;
    public Spell Spell;
    public int Defend;
    public float AttackDelay;
    
}
