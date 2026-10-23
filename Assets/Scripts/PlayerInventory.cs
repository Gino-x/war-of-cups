using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInventory : MonoBehaviour
{
    public int healAmount = 3;
    public int shieldAmount = 5;

    public bool hasHealPotion = false;
    public bool hasShieldPotion = false;

    private PlayerHealth playerHealth;

    private void Start()
    {
        playerHealth = GetComponent<PlayerHealth>();
    }

    private void Update()
    {
        if (playerHealth == null || Keyboard.current == null) return;

        if (playerHealth.playerIndex == 0)
        {
            if (Keyboard.current.eKey.wasPressedThisFrame) UseHeal();
            if (Keyboard.current.qKey.wasPressedThisFrame) UseShield();
        }
        else
        {
            if (Keyboard.current.kKey.wasPressedThisFrame) UseHeal();
            if (Keyboard.current.lKey.wasPressedThisFrame) UseShield();
        }
    }

    public void PickupHeal()
    {
        if (hasHealPotion) playerHealth.Heal(healAmount);
        else hasHealPotion = true;
    }

    public void PickupShield()
    {
        if (hasShieldPotion) playerHealth.AddShield(shieldAmount);
        else hasShieldPotion = true;
    }

    private void UseHeal()
    {
        if (!hasHealPotion) return;
        hasHealPotion = false;
        playerHealth.Heal(healAmount);
    }

    private void UseShield()
    {
        if (!hasShieldPotion) return;
        hasShieldPotion = false;
        playerHealth.AddShield(shieldAmount);
    }
}