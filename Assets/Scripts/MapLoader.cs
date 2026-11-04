using UnityEngine;

// Put this in FightScene. Turns on only the background the player picked.
public class MapLoader : MonoBehaviour
{
    [Tooltip("One background object per map. Same order as the MapSelection previews.")]
    public GameObject[] backgrounds;

    private void Awake()
    {
        if (backgrounds == null || backgrounds.Length == 0)
            return;

        int selected = Mathf.Clamp(PlayerPrefs.GetInt("selectedMap", 0), 0, backgrounds.Length - 1);

        for (int i = 0; i < backgrounds.Length; i++)
            backgrounds[i].SetActive(i == selected);
    }
}
