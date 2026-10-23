using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public GameObject deathScreenPanel;

    private bool isDead = false;

    void Awake()
    {
        Instance = this;
        isDead = false;
    }

    void Update()
    {
        if (!isDead) return;
        if (Keyboard.current == null) return;

        if (Keyboard.current.digit1Key.wasPressedThisFrame ||
            Keyboard.current.digit2Key.wasPressedThisFrame ||
            Keyboard.current.numpad1Key.wasPressedThisFrame ||
            Keyboard.current.numpad2Key.wasPressedThisFrame)
        {
            OnRestartClick();
        }

        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            OnExitToStartClick();
        }
    }

    public void ShowDeathScreen()
    {
        isDead = true;
        Time.timeScale = 0f;
        if (deathScreenPanel != null)
            deathScreenPanel.SetActive(true);
    }

    public void OnRestartClick()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void OnExitToStartClick()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("StartScene");
    }
}