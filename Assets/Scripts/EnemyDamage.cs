using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDamage : MonoBehaviour
{
    [SerializeField] private int damage = 2;
    [SerializeField] private float attackCooldown = 1f;
    [Tooltip("1 = normal enemy. Heavy enemy: 3 means 3x damage.")]
    [SerializeField] private float damageMultiplier = 1f;

    private float lastAttackTime = -999f;

    public void SetDamage(int newDamage)
    {
        damage = Mathf.RoundToInt(newDamage * damageMultiplier);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            if (Time.time >= lastAttackTime + attackCooldown)
            {
                PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();
                if (playerHealth != null)
                {
                    playerHealth.TakeDamage(damage);
                    lastAttackTime = Time.time;
                }
            }
        }
    }
}
