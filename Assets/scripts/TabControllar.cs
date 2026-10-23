using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class TabControllar : MonoBehaviour
{
    public Image[] tabImages; // Array of Image components for the tabs
    public GameObject[] pages; // Array of GameObjects for the tab contents

    void Start()
    {
        ActivateTab(0); // Activate the first tab by default
    }

    void Update()
    {
        if (Keyboard.current == null) return;

        // Left tab (Menu): A or F
        if (Keyboard.current.aKey.wasPressedThisFrame || Keyboard.current.fKey.wasPressedThisFrame)
            ActivateTab(0);

        // Right tab (Settings): D or H
        if (Keyboard.current.dKey.wasPressedThisFrame || Keyboard.current.hKey.wasPressedThisFrame)
            ActivateTab(1);
    }

    public void ActivateTab(int tabNo)
    {
        for (int i = 0; i < pages.Length; i++)
        {
            pages[i].SetActive(false);
            tabImages[i].color = Color.grey;
        }
        pages[tabNo].SetActive(true);
        tabImages[tabNo].color = Color.white;
    }
}