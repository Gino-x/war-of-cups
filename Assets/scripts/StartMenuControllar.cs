    using UnityEngine;
    using UnityEngine.SceneManagement;

public class StartMenuControllar : MonoBehaviour
{
   public void OnStartClick()
    {
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
// FightScene  ,CharactarSelection