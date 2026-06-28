using UnityEngine;
using UnityEngine.InputSystem;

public class MenuScript : MonoBehaviour
{
    public GameObject menuCanvas;
    private bool isPaused = false;

    void Start()
    {
        menuCanvas.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current.tabKey.wasPressedThisFrame)
        {
            if (isPaused)
            {
                // Resume the game
                ResumeGame();
            }
            else
            {
                // Pause the game
                PauseGame();
            }
        }   
    }

    private void PauseGame()
    {
        isPaused = true;
        menuCanvas.SetActive(true);
        Time.timeScale = 0f;  // Freeze all game logic
        Debug.Log("Game Paused");
    }

    private void ResumeGame()
    {
        isPaused = false;
        menuCanvas.SetActive(false);
        Time.timeScale = 1f;  // Resume normal game speed
        Debug.Log("Game Resumed");
    }
}
