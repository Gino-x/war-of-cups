using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using TMPro;

public class CharacterSelection : MonoBehaviour
{
    public GameObject[] characters;
    public int selectedCharacter = 0;
    [SerializeField] private TextMeshProUGUI turnText;

    private int currentPlayer = 1; // 1 = picking P1, 2 = picking P2
    private bool isDuoMode;

    private void Start()
    {
        isDuoMode = PlayerPrefs.GetInt("gameMode", 0) == 1;

        for (int i = 0; i < characters.Length; i++)
        {
            characters[i].SetActive(false);
            DisableMovement(characters[i]);
        }

        characters[selectedCharacter].SetActive(true);
        UpdateTurnText();
    }

    private void Update()
    {
        if (Keyboard.current == null) return;

        // Next: D or H
        if (Keyboard.current.dKey.wasPressedThisFrame || Keyboard.current.hKey.wasPressedThisFrame)
            NextCharacter();

        // Back: A or F
        if (Keyboard.current.aKey.wasPressedThisFrame || Keyboard.current.fKey.wasPressedThisFrame)
            PreviousCharacter();

        if (Keyboard.current.digit1Key.wasPressedThisFrame || Keyboard.current.digit2Key.wasPressedThisFrame)
            StartGame();
    }

    public void NextCharacter()
    {
        characters[selectedCharacter].SetActive(false);
        selectedCharacter = (selectedCharacter + 1) % characters.Length;
        characters[selectedCharacter].SetActive(true);
    }

    public void PreviousCharacter()
    {
        characters[selectedCharacter].SetActive(false);
        selectedCharacter--;
        if (selectedCharacter < 0)
        {
            selectedCharacter += characters.Length;
        }
        characters[selectedCharacter].SetActive(true);
    }

    public void StartGame()
    {
        if (isDuoMode && currentPlayer == 1)
        {
            PlayerPrefs.SetInt("selectedCharacter_P1", selectedCharacter);
            currentPlayer = 2;
            UpdateTurnText();
            return;
        }

        if (isDuoMode)
        {
            PlayerPrefs.SetInt("selectedCharacter_P2", selectedCharacter);
        }
        else
        {
            PlayerPrefs.SetInt("selectedCharacter", selectedCharacter);
        }

        SceneManager.LoadScene("FightScene");
    }

    private void UpdateTurnText()
    {
        if (turnText == null)
            return;

        if (!isDuoMode)
        {
            turnText.text = "";
            return;
        }

        turnText.text = currentPlayer == 1 ? "Player 1" : "Player 2";
    }

    private void DisableMovement(GameObject character)
    {
        var movement = character.GetComponent<PlayerMovement>();
        if (movement != null)
            movement.enabled = false;

        var shoot = character.GetComponent<PlayerShoot>();
        if (shoot != null)
            shoot.enabled = false;

        foreach (var comp in character.GetComponents<Component>())
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