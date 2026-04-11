using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;


public class LoadCharacter : MonoBehaviour
{
    public GameObject[] characterPrefabs;
    // Assign 4 transforms in inspector (order: top-left, top-right, bottom-left, bottom-right).
    // If not assigned the script will compute corner positions from Camera.main.
    public Transform[] cornerSpawnPoints;
    // When cornerSpawnPoints are not provided the camera's viewport corners are used.
    // This padding moves spawns slightly inside the screen (0..0.5). Default 0.05 (5%).
    [Range(0f, 0.4f)] public float viewportPadding = 0.05f;

    // If set to true you can manually specify world-space spawn positions below.
    // Use this to change spawn coordinates directly from the inspector.
    public bool useCustomSpawnPositions = false;
    // Four custom spawn positions (world space). Order: top-left, top-right, bottom-left, bottom-right.
    public Vector3[] customSpawnPositions = new Vector3[4];

    // Choose how spawn positions are determined
    public enum SpawnMode { CameraCorners, CornerTransforms, CustomPositions, PerCharacterXY }
    [Tooltip("CameraCorners: use camera corners; CornerTransforms: use assigned corner transforms; CustomPositions: use customSpawnPositions; PerCharacterXY: use perCharacterPositions for each prefab")] 
    public SpawnMode spawnMode = SpawnMode.CameraCorners;

    // When using PerCharacterXY mode, set X,Y for each character prefab here (array length should match characterPrefabs).
    public Vector2[] perCharacterPositions;

    // The Z coordinate used when placing objects using 2D X/Y coordinates. Default 0.
    public float spawnZ = 0f;
   

    /// <summary>
    /// Start is called before the first frame update.
    /// It determines four spawn positions (either from assigned transforms, camera corners or custom positions),
    /// instantiates the selected player prefab and spawns three enemy prefabs. When using
    /// PerCharacterXY spawn mode each prefab will be placed at its corresponding X,Y entry
    /// in the `perCharacterPositions` array (index aligned with `characterPrefabs`).
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

        Vector3[] spawnPositions = new Vector3[4];

        // Determine spawn positions based on the selected spawnMode
        if (spawnMode == SpawnMode.CornerTransforms)
        {
            // Use assigned corner transforms
            if (cornerSpawnPoints != null && cornerSpawnPoints.Length >= 4)
            {
                for (int i = 0; i < 4; i++)
                    spawnPositions[i] = cornerSpawnPoints[i].position;
            }
            else
            {
                Debug.LogWarning("spawnMode is CornerTransforms but cornerSpawnPoints are not set. Falling back to CameraCorners.");
                spawnMode = SpawnMode.CameraCorners;
            }
        }

        if (spawnMode == SpawnMode.CameraCorners)
        {
            // Compute corners from the main camera, applying viewport padding
            Camera cam = Camera.main;
            if (cam == null)
            {
                Debug.LogWarning("No main camera found for CameraCorners spawn mode.");
                return;
            }
            float z = Mathf.Abs(cam.transform.position.z);
            // Apply padding to move spawns slightly inside the screen
            float left = viewportPadding;
            float right = 1f - viewportPadding;
            float top = 1f - viewportPadding;
            float bottom = viewportPadding;
            // Viewport coordinates: (left,top)=top-left, (right,top)=top-right, (left,bottom)=bottom-left, (right,bottom)=bottom-right
            spawnPositions[0] = cam.ViewportToWorldPoint(new Vector3(left, top, z));
            spawnPositions[1] = cam.ViewportToWorldPoint(new Vector3(right, top, z));
            spawnPositions[2] = cam.ViewportToWorldPoint(new Vector3(left, bottom, z));
            spawnPositions[3] = cam.ViewportToWorldPoint(new Vector3(right, bottom, z));
        }

        if (spawnMode == SpawnMode.CustomPositions)
        {
            // Use user-provided custom world-space positions
            if (customSpawnPositions != null && customSpawnPositions.Length >= 4)
            {
                for (int i = 0; i < 4; i++)
                    spawnPositions[i] = customSpawnPositions[i];
            }
            else
            {
                Debug.LogWarning("spawnMode is CustomPositions but customSpawnPositions are not set or too short. Falling back to CameraCorners.");
                // fallback compute camera corners
                spawnMode = SpawnMode.CameraCorners;
                Camera cam = Camera.main;
                if (cam == null)
                {
                    Debug.LogWarning("No main camera found for fallback CameraCorners.");
                    return;
                }
                float z = Mathf.Abs(cam.transform.position.z);
                float left = viewportPadding;
                float right = 1f - viewportPadding;
                float top = 1f - viewportPadding;
                float bottom = viewportPadding;
                spawnPositions[0] = cam.ViewportToWorldPoint(new Vector3(left, top, z));
                spawnPositions[1] = cam.ViewportToWorldPoint(new Vector3(right, top, z));
                spawnPositions[2] = cam.ViewportToWorldPoint(new Vector3(left, bottom, z));
                spawnPositions[3] = cam.ViewportToWorldPoint(new Vector3(right, bottom, z));
            }
        }

        // PerCharacterXY: each prefab has an X,Y position defined in perCharacterPositions
        if (spawnMode == SpawnMode.PerCharacterXY)
        {
            if (perCharacterPositions != null && perCharacterPositions.Length >= characterPrefabs.Length)
            {
                // Place the selected player at its configured position
                spawnPositions[0] = new Vector3(perCharacterPositions[selectedCharacter].x, perCharacterPositions[selectedCharacter].y, spawnZ);
                // For enemies, use their corresponding perCharacterPositions entries
                int enemyCorner = 1;
                for (int i = 0; i < characterPrefabs.Length && enemyCorner <= 3; i++)
                {
                    if (i == selectedCharacter) continue;
                    spawnPositions[enemyCorner] = new Vector3(perCharacterPositions[i].x, perCharacterPositions[i].y, spawnZ);
                    enemyCorner++;
                }
                // If there were fewer other prefabs than 3, remaining spawnPositions remain default (Vector3.zero)
            }
            else
            {
                Debug.LogWarning("spawnMode is PerCharacterXY but perCharacterPositions array is missing or too short. Falling back to CameraCorners.");
                // fallback to camera corners
                spawnMode = SpawnMode.CameraCorners;
                Camera cam = Camera.main;
                if (cam == null)
                {
                    Debug.LogWarning("No main camera found for fallback CameraCorners.");
                    return;
                }
                float z = Mathf.Abs(cam.transform.position.z);
                float left = viewportPadding;
                float right = 1f - viewportPadding;
                float top = 1f - viewportPadding;
                float bottom = viewportPadding;
                spawnPositions[0] = cam.ViewportToWorldPoint(new Vector3(left, top, z));
                spawnPositions[1] = cam.ViewportToWorldPoint(new Vector3(right, top, z));
                spawnPositions[2] = cam.ViewportToWorldPoint(new Vector3(left, bottom, z));
                spawnPositions[3] = cam.ViewportToWorldPoint(new Vector3(right, bottom, z));
            }
        }

        // If the user specified custom world-space spawn positions in the inspector, override the computed positions.
        if (useCustomSpawnPositions && customSpawnPositions != null && customSpawnPositions.Length >= 4)
        {
            for (int i = 0; i < 4; i++)
                spawnPositions[i] = customSpawnPositions[i];
        }

        // Spawn the selected character in the top-left (index 0)
        GameObject player = Instantiate(characterPrefabs[selectedCharacter], spawnPositions[0], Quaternion.identity);
        // Mark as player so enemies can target it
        player.tag = "Player";
        // Enable player movement script if present
        var playerMovement = player.GetComponent<playerMovment>();
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

        // Spawn three other prefabs in the remaining corners (1..3). If there are fewer prefabs
        // the available prefabs are reused (excluding the selected one when possible).
        List<int> otherIndices = Enumerable.Range(0, characterPrefabs.Length).Where(i => i != selectedCharacter).ToList();
        if (otherIndices.Count == 0)
        {
            Debug.LogWarning("Only one prefab available; nothing else to spawn.");
            return;
        }

        for (int corner = 1; corner <= 3; corner++)
        {
            int idx = otherIndices[(corner - 1) % otherIndices.Count];
            GameObject enemy = Instantiate(characterPrefabs[idx], spawnPositions[corner], Quaternion.identity);

            // Ensure enemy cannot be controlled by player input or movement scripts
            var enemyMovement = enemy.GetComponent<playerMovment>();
            if (enemyMovement != null)
                enemyMovement.enabled = false;

            // Remove any PlayerInput components from enemies (avoids input system pairing errors at runtime).
            foreach (var comp in enemy.GetComponents<Component>())
            {
                if (comp == null) continue;
                if (comp.GetType().Name == "PlayerInput")
                {
                    Object.Destroy(comp);
                }
            }

            // Add or configure NavMeshAgent for pathfinding. If NavMesh is not available at runtime,
            // EnemyAI falls back to a simple seek movement.
            var agent = enemy.GetComponent<NavMeshAgent>();
            if (agent == null)
                agent = enemy.AddComponent<NavMeshAgent>();
            agent.speed = 3.5f;
            agent.angularSpeed = 120f;

            // Add EnemyAI and point it at the player
            var ai = enemy.GetComponent<EnemyAI>() ?? enemy.AddComponent<EnemyAI>();
            ai.target = player.transform;
        }
    }
}