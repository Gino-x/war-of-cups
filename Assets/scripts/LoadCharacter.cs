using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class LoadCharacter : MonoBehaviour
{
    public GameObject[] characterPrefabs;
   

    /// <summary>
    /// Start is called before the first frame update.
    /// Instantiates the selected character prefab and spawns it in the middle of the scene.
    /// </summary>
    void Start()
    {
        int selectedCharacter = PlayerPrefs.GetInt("selectedCharacter", 0);

        if (characterPrefabs == null || characterPrefabs.Length == 0)
        {
            Debug.LogWarning("No character prefabs assigned to LoadCharacter.");
            return;
        }

        selectedCharacter = Mathf.Clamp(selectedCharacter, 0, characterPrefabs.Length - 1);

        // Spawn the selected character in the middle of the scene
        GameObject player = Instantiate(characterPrefabs[selectedCharacter], Vector3.zero, Quaternion.identity);
        player.tag = "Player";

        // Enable player movement script if present
        var playerMovement = player.GetComponent<PlayerMovement>();
        if (playerMovement != null)
            playerMovement.enabled = true;

        // If the prefab contains a PlayerInput (Input System) component, enable it for the player.
        // We avoid a compile-time dependency on the Input System by finding the component by name at runtime.
        foreach (var comp in player.GetComponents<Component>())
        {
            if (comp == null) continue;
            if (comp.GetType().Name == "PlayerInput")
            {
                if (comp is Behaviour b)
                    b.enabled = true;
            }
        }
    }
}