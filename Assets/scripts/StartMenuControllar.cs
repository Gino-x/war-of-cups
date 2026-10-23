using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class StartMenuControllar : MonoBehaviour
{
    void Update()
    {
        if (Keyboard.current == null) return;

        if (Keyboard.current.escapeKey.wasPressedThisFrame)
            OnExitClick();

        if (Keyboard.current.digit1Key.wasPressedThisFrame)
            OnStartClick();

        if (Keyboard.current.digit2Key.wasPressedThisFrame)
            OnTwoPlayerClick();
    }

    public void OnStartClick()
    {
        PlayerPrefs.SetInt("gameMode", 0); // 0 = solo
        PlayerPrefs.Save();
        SceneManager.LoadScene("CharactarSelection");
    }

    public void OnTwoPlayerClick()
    {
        PlayerPrefs.SetInt("gameMode", 1); // 1 = duo
        PlayerPrefs.Save();
        SceneManager.LoadScene("CharactarSelection");
    }

    public void OnExitClick()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
        Application.Quit();
    }
}