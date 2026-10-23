using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuScript : MonoBehaviour
{
    public GameObject menuCanvas;
    public Slider musicSlider;
    [SerializeField] private float volumeStep = 5f;
    private bool isPaused = false;

    void Start()
    {
        menuCanvas.SetActive(false);
    }

    void Update()
    {
        if (Keyboard.current == null) return;

        if (Keyboard.current.tabKey.wasPressedThisFrame)
        {
            if (isPaused)
                ResumeGame();
            else
                PauseGame();
        }

        if (!isPaused) return;

        if (Keyboard.current.digit1Key.wasPressedThisFrame || Keyboard.current.digit2Key.wasPressedThisFrame)
            OnRestartClick();

        if (Keyboard.current.escapeKey.wasPressedThisFrame)
            OnExitToStartClick();

        if (musicSlider != null)
        {
            bool plusPressed = Keyboard.current.numpadPlusKey.wasPressedThisFrame || Keyboard.current.equalsKey.wasPressedThisFrame;
            bool minusPressed = Keyboard.current.numpadMinusKey.wasPressedThisFrame || Keyboard.current.minusKey.wasPressedThisFrame;

            if (plusPressed)
                musicSlider.value = Mathf.Clamp(musicSlider.value + volumeStep, musicSlider.minValue, musicSlider.maxValue);

            if (minusPressed)
                musicSlider.value = Mathf.Clamp(musicSlider.value - volumeStep, musicSlider.minValue, musicSlider.maxValue);
        }
    }

    private void PauseGame()
    {
        isPaused = true;
        menuCanvas.SetActive(true);
        Time.timeScale = 0f;
    }

    private void ResumeGame()
    {
        isPaused = false;
        menuCanvas.SetActive(false);
        Time.timeScale = 1f;
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