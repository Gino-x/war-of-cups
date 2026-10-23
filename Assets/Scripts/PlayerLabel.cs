using UnityEngine;
using TMPro;

public class PlayerLabel : MonoBehaviour
{
    [SerializeField] private int playerIndex = 0; // 0 = Player 1, 1 = Player 2
    [SerializeField] private TextMeshProUGUI labelText;

    private PlayerHealth playerHealth;
    private bool connected = false;

    private void Start()
    {
        if (labelText == null)
            labelText = GetComponent<TextMeshProUGUI>();
    }

    private void Update()
    {
        if (!connected)
        {
            FindMatchingPlayer();
            return;
        }

        if (playerHealth == null)
        {
            connected = false;
            return;
        }

        UpdateLabel();
    }

    private void FindMatchingPlayer()
    {
        PlayerHealth[] all = FindObjectsOfType<PlayerHealth>();
        foreach (PlayerHealth p in all)
        {
            if (p.playerIndex == playerIndex)
            {
                playerHealth = p;
                connected = true;
                return;
            }
        }
    }

    private void UpdateLabel()
    {
        if (labelText != null)
            labelText.text = playerIndex == 0 ? "Player 1" : "Player 2";
    }
}
