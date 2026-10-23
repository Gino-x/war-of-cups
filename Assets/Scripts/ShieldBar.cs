using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShieldBar : MonoBehaviour
{
    [SerializeField] private Image shieldBarFill;
    [SerializeField] private TextMeshProUGUI shieldText;
    [SerializeField] private int playerIndex = 0; // 0 = Player 1, 1 = Player 2

    [SerializeField] private Color fullShieldColor = Color.cyan;
    [SerializeField] private Color lowShieldColor = Color.blue;
    [SerializeField] private float colorChangeThreshold = 0.3f;

    private PlayerHealth playerHealth;
    private bool connected = false;

    private void Start()
    {
        if (shieldBarFill == null)
            shieldBarFill = GetComponent<Image>();
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

        UpdateShieldBar();
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

    private void UpdateShieldBar()
    {
        float shieldPercentage = (float)playerHealth.shield / playerHealth.maxShield;
        shieldPercentage = Mathf.Clamp01(shieldPercentage);

        shieldBarFill.fillAmount = shieldPercentage;

        if (shieldPercentage <= colorChangeThreshold)
            shieldBarFill.color = Color.Lerp(lowShieldColor, fullShieldColor, shieldPercentage / colorChangeThreshold);
        else
            shieldBarFill.color = fullShieldColor;

        if (shieldText != null)
            shieldText.text = $"{playerHealth.shield} / {playerHealth.maxShield}";
    }
}