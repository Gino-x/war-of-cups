using UnityEngine;

public class LoadCharacter : MonoBehaviour
{
    public GameObject[] characterPrefabs;

    void Start()
    {
        int selectedCharacter = PlayerPrefs.GetInt("selectedCharacter", 0);

        if (characterPrefabs == null || characterPrefabs.Length == 0)
        {
            Debug.LogWarning("No character prefabs assigned to LoadCharacter.");
            return;
        }

        selectedCharacter = Mathf.Clamp(selectedCharacter, 0, characterPrefabs.Length - 1);

        // Spawn the selected character at the center of the map
        GameObject player = Instantiate(characterPrefabs[selectedCharacter], Vector3.zero, Quaternion.identity);
        player.tag = "Player";
    }
}