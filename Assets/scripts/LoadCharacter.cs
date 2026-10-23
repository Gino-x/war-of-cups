using UnityEngine;

public class LoadCharacter : MonoBehaviour
{
    public static LoadCharacter Instance;

    public GameObject[] characterPrefabs;
    public Transform spawnPointP1;
    public Transform spawnPointP2;

    private bool[] playerNeedsRespawn = new bool[2];

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        int gameMode = PlayerPrefs.GetInt("gameMode", 0);

        if (characterPrefabs == null || characterPrefabs.Length == 0)
        {
            Debug.LogWarning("No character prefabs assigned to LoadCharacter.");
            return;
        }

        if (gameMode == 0)
        {
            SpawnPlayer("selectedCharacter", spawnPointP1 != null ? spawnPointP1.position : Vector3.zero, 0);
        }
        else
        {
            SpawnPlayer("selectedCharacter_P1", spawnPointP1 != null ? spawnPointP1.position : new Vector3(-3, 0, 0), 0);
            SpawnPlayer("selectedCharacter_P2", spawnPointP2 != null ? spawnPointP2.position : new Vector3(3, 0, 0), 1);
        }
    }

    // Call this when a player dies but the other one is still alive.
    public void MarkPlayerForRespawn(int playerIndex)
    {
        if (playerIndex < 0 || playerIndex > 1) return;
        playerNeedsRespawn[playerIndex] = true;
    }

    // Call this right after a round is won, before the next round starts.
    public void RespawnMarkedPlayers()
    {
        for (int i = 0; i < playerNeedsRespawn.Length; i++)
        {
            if (playerNeedsRespawn[i])
            {
                RespawnPlayer(i);
                playerNeedsRespawn[i] = false;
            }
        }
    }

    private void RespawnPlayer(int playerIndex)
    {
        int gameMode = PlayerPrefs.GetInt("gameMode", 0);
        string prefsKey = gameMode == 0
            ? "selectedCharacter"
            : (playerIndex == 0 ? "selectedCharacter_P1" : "selectedCharacter_P2");

        Vector3 position = playerIndex == 0
            ? (spawnPointP1 != null ? spawnPointP1.position : new Vector3(-3, 0, 0))
            : (spawnPointP2 != null ? spawnPointP2.position : new Vector3(3, 0, 0));

        SpawnPlayer(prefsKey, position, playerIndex);
    }

    void SpawnPlayer(string prefsKey, Vector3 position, int playerIndex)
    {
        int selected = PlayerPrefs.GetInt(prefsKey, 0);
        selected = Mathf.Clamp(selected, 0, characterPrefabs.Length - 1);

        GameObject player = Instantiate(characterPrefabs[selected], position, Quaternion.identity);
        player.tag = "Player";

        var movement = player.GetComponent<PlayerMovement>();
        if (movement != null)
        {
            movement.enabled = true;
            movement.playerIndex = playerIndex;
        }

        var shoot = player.GetComponent<PlayerShoot>();
        if (shoot != null)
        {
            shoot.playerIndex = playerIndex;
        }

        var health = player.GetComponent<PlayerHealth>();
        if (health != null)
        {
            health.playerIndex = playerIndex;
        }

        foreach (var comp in player.GetComponents<Component>())
        {
            if (comp == null) continue;
            if (comp.GetType().Name == "PlayerInput")
            {
                if (comp is Behaviour b)
                    b.enabled = false;
            }
        }
    }
}