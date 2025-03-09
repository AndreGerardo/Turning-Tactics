using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackHitBox : MonoBehaviour
{
        private Character character;
        
        public void Init(Character setChar)
        {
            character = setChar;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.TryGetComponent(out IAttackable damageable))
            {
                damageable.TakeDamage(character);
            }

        }
    }
