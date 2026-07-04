using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Image healthBarFill;
    [SerializeField] private TextMeshProUGUI healthText;
    private PlayerHealth playerHealth;

    [SerializeField] private Color fullHealthColor = Color.green;
    [SerializeField] private Color lowHealthColor = Color.red;
    [SerializeField] private float colorChangeThreshold = 0.3f;

    private bool connected = false;

    private void Start()
    {
        if (healthBarFill == null)
            healthBarFill = GetComponent<Image>();
    }

    private void Update()
    {
        // Keep searching for PlayerHealth until found
        if (!connected)
        {
            playerHealth = FindObjectOfType<PlayerHealth>();
            if (playerHealth != null)
            {
                connected = true;
                Debug.Log("✓ HealthBar connected to PlayerHealth!");
            }
            // If not found yet, keep trying next frame
            return;
        }

        // Update the health bar every frame
        UpdateHealthBar();
    }

    private void UpdateHealthBar()
    {
        if (playerHealth == null)
        {
            connected = false; // Reset if player is destroyed
            return;
        }

        // Calculate health percentage (0 to 1)
        float healthPercentage = (float)playerHealth.health / playerHealth.maxHealth;
        healthPercentage = Mathf.Clamp01(healthPercentage);

        // Update bar fill INSTANTLY
        healthBarFill.fillAmount = healthPercentage;

        // Change color based on health
        if (healthPercentage <= colorChangeThreshold)
        {
            healthBarFill.color = Color.Lerp(Color.red, Color.green, healthPercentage / colorChangeThreshold);
        }
        else
        {
            healthBarFill.color = Color.green;
        }

        // Update text
        if (healthText != null)
        {
            healthText.text = $"{playerHealth.health} / {playerHealth.maxHealth}";
        }
    }
}
