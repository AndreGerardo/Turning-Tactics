using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Spell", menuName = "ScriptableObjects/Spell/New Spell")]
public class Spell : ScriptableObject
{
    public string SpellName;
    public int Damage;
}
