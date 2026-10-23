using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private int pointsPerKill = 10;

    private int score = 0;

    private void Awake()
    {
        Instance = this;
        UpdateScoreText();
    }

    public void AddScore()
    {
        score += pointsPerKill;
        UpdateScoreText();
    }

    private void UpdateScoreText()
    {
        if (scoreText != null)
            scoreText.text = "Score: " + score;
    }
}