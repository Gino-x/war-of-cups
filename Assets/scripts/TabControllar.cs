using UnityEngine;
using UnityEngine.UI;

public class TabControllar : MonoBehaviour
{
   public Image[] tabImages; // Array of Image components for the tabs
   public GameObject[] pages; // Array of GameObjects for the tab contents

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ActivateTab(0); // Activate the first tab by default

    }

    public void ActivateTab(int tabNo)
    {
        for (int i = 0; i < pages.Length; i++)
        {
            pages[i].SetActive(false);
            tabImages[i].color = Color.grey; // Set all tabs to white (inactive)
                                             // Activate the selected tab's page and deactivate others
        }
        pages[tabNo].SetActive(true);
        tabImages[tabNo].color = Color.white; // Set the selected tab to white (active)
    }

}
