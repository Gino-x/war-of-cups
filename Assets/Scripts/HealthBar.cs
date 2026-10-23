using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Image healthBarFill;
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private int playerIndex = 0; // 0 = Player 1, 1 = Player 2

    private PlayerHealth playerHealth;
    private bool connected = false;

    [SerializeField] private Color fullHealthColor = Color.green;
    [SerializeField] private Color lowHealthColor = Color.red;
    [SerializeField] private float colorChangeThreshold = 0.3f;

    private void Start()
    {
        if (healthBarFill == null)
            healthBarFill = GetComponent<Image>();
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

        UpdateHealthBar();
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

    private void UpdateHealthBar()
    {
        float healthPercentage = (float)playerHealth.health / playerHealth.maxHealth;
        healthPercentage = Mathf.Clamp01(healthPercentage);

        healthBarFill.fillAmount = healthPercentage;

        if (healthPercentage <= colorChangeThreshold)
            healthBarFill.color = Color.Lerp(Color.red, Color.green, healthPercentage / colorChangeThreshold);
        else
            healthBarFill.color = Color.green;

        if (healthText != null)
            healthText.text = $"{playerHealth.health} / {playerHealth.maxHealth}";
    }
}