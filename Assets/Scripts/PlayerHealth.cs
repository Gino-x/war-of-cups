using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int health;
    public int maxHealth = 10;
    public int playerIndex;

    public int shield;
    public int maxShield = 5;

    private void Awake()
    {
        health = maxHealth;
        shield = maxShield;
    }

    public void TakeDamage(int amount)
    {
        if (shield > 0)
        {
            int absorbed = Mathf.Min(shield, amount);
            shield -= absorbed;
            amount -= absorbed;
        }

        if (amount <= 0) return;

        health -= amount;
        if (health <= 0)
        {
            bool otherPlayersAlive = false;
            PlayerHealth[] allPlayers = FindObjectsOfType<PlayerHealth>();
            foreach (PlayerHealth player in allPlayers)
            {
                if (player != this && player.health > 0)
                {
                    otherPlayersAlive = true;
                    break;
                }
            }

            if (!otherPlayersAlive)
            {
                GameManager.Instance.ShowDeathScreen();
            }
            else if (LoadCharacter.Instance != null)
            {
                LoadCharacter.Instance.MarkPlayerForRespawn(playerIndex);
            }

            Destroy(gameObject);
        }
    }

    public void Heal(int amount)
    {
        health += amount;
        if (health > maxHealth) health = maxHealth;
    }

    public void AddShield(int amount)
    {
        shield += amount;
        if (shield > maxShield) shield = maxShield;
    }
}