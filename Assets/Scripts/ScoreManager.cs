using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    [SerializeField] private TextMeshProUGUI scoreTextP1; // left side
    [SerializeField] private TextMeshProUGUI scoreTextP2; // right side

    private int scoreP1 = 0;
    private int scoreP2 = 0;

    private void Awake()
    {
        Instance = this;
        UpdateScoreText();
    }

    public void AddScore(int playerIndex, int points)
    {
        if (playerIndex == 1)
            scoreP2 += points;
        else
            scoreP1 += points;

        UpdateScoreText();
    }

    private void UpdateScoreText()
    {
        if (scoreTextP1 != null)
            scoreTextP1.text = "Score: " + scoreP1;

        if (scoreTextP2 != null)
            scoreTextP2.text = "Score: " + scoreP2;
    }
}
