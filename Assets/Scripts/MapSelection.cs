using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using TMPro;

public class MapSelection : MonoBehaviour
{
    [Tooltip("One preview object per map (image or sprite). Only the selected one is shown.")]
    public GameObject[] mapPreviews;
    [Tooltip("Optional. Same order as previews.")]
    [SerializeField] private string[] mapNames;
    [SerializeField] private TextMeshProUGUI mapNameText;

    private int selectedMap = 0;

    private void Start()
    {
        if (mapPreviews == null || mapPreviews.Length == 0)
        {
            Debug.LogWarning("MapSelection: no map previews assigned.");
            return;
        }

        selectedMap = Mathf.Clamp(PlayerPrefs.GetInt("selectedMap", 0), 0, mapPreviews.Length - 1);
        ShowMap();
    }

    private void Update()
    {
        if (Keyboard.current == null) return;
        if (mapPreviews == null || mapPreviews.Length == 0) return;

        // Next: D or H
        if (Keyboard.current.dKey.wasPressedThisFrame || Keyboard.current.hKey.wasPressedThisFrame)
            NextMap();

        // Back: A or F
        if (Keyboard.current.aKey.wasPressedThisFrame || Keyboard.current.fKey.wasPressedThisFrame)
            PreviousMap();

        // Confirm: 1 or 2
        if (Keyboard.current.digit1Key.wasPressedThisFrame || Keyboard.current.digit2Key.wasPressedThisFrame)
            StartGame();
    }

    public void NextMap()
    {
        selectedMap = (selectedMap + 1) % mapPreviews.Length;
        ShowMap();
    }

    public void PreviousMap()
    {
        selectedMap--;
        if (selectedMap < 0)
            selectedMap += mapPreviews.Length;
        ShowMap();
    }

    private void ShowMap()
    {
        for (int i = 0; i < mapPreviews.Length; i++)
            mapPreviews[i].SetActive(i == selectedMap);

        if (mapNameText != null)
        {
            if (mapNames != null && selectedMap < mapNames.Length)
                mapNameText.text = mapNames[selectedMap];
            else
                mapNameText.text = "Map " + (selectedMap + 1);
        }
    }

    public void StartGame()
    {
        PlayerPrefs.SetInt("selectedMap", selectedMap);
        PlayerPrefs.Save();
        SceneManager.LoadScene("FightScene");
    }
}
