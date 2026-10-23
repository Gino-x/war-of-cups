using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StorageUI : MonoBehaviour
{
    [SerializeField] private int playerIndex = 0; // 0 = Player 1, 1 = Player 2

    [SerializeField] private Image healSlotImage;
    [SerializeField] private Image shieldSlotImage;

    [SerializeField] private Color fullColor = Color.white;
    [SerializeField] private Color emptyColor = new Color(1, 1, 1, 0.25f);

    private PlayerInventory inventory;
    private bool connected = false;

    private void Update()
    {
        if (!connected)
        {
            FindMatchingPlayer();
            return;
        }

        if (inventory == null)
        {
            connected = false;
            return;
        }

        if (healSlotImage != null)
            healSlotImage.color = inventory.hasHealPotion ? fullColor : emptyColor;

        if (shieldSlotImage != null)
            shieldSlotImage.color = inventory.hasShieldPotion ? fullColor : emptyColor;
    }

    private void FindMatchingPlayer()
    {
        PlayerInventory[] all = FindObjectsOfType<PlayerInventory>();
        foreach (PlayerInventory inv in all)
        {
            PlayerHealth ph = inv.GetComponent<PlayerHealth>();
            if (ph != null && ph.playerIndex == playerIndex)
            {
                inventory = inv;
                connected = true;
                return;
            }
        }
    }
}