using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int health;
    public int maxHealth = 10;

    // Awake is called before Start - initialize health early so HealthBar gets correct value
    private void Awake()
    {
        health = maxHealth;
    }

    // Start is called before the first frame update
    void Start()
    {
        // Health is already initialized in Awake
    }

    public void TakeDamage(int amount)
    {
        health -= amount;
        Debug.Log("Player took " + amount + " damage! Current health: " + health);
        if (health <= 0)
        {
            Debug.Log("Player died!");
            Destroy(gameObject);
        }
    }
}
