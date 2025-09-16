using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    public GameObject mainMenuPanel;
    public GameObject settingsPanel;
    public GameObject categoryPanel;

    // Start button = show category, hide main
    public void OnStartButton()
    {
        mainMenuPanel.SetActive(false);
        categoryPanel.SetActive(true);
    }

    // Settings button = show settings, hide main
    public void OnSettingsButton()
    {
        mainMenuPanel.SetActive(false);
        settingsPanel.SetActive(true);
    }

    // Back button from settings = back to main
    public void OnBackFromSettings()
    {
        settingsPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }

    // Quit game
    public void OnQuitButton()
    {
        Debug.Log("Quit Game!");
        Application.Quit();
    }
}
