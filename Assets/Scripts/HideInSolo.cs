using UnityEngine;

public class HideInSolo : MonoBehaviour
{
    void Start()
    {
        int gameMode = PlayerPrefs.GetInt("gameMode", 0);
        if (gameMode == 0)
        {
            gameObject.SetActive(false);
        }
    }
}