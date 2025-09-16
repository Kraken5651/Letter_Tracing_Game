using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class TracingLevelManager : MonoBehaviour
{
    [Header("Level Panels")]
    public GameObject[] tracingPanels;     // Each panel = one letter/shape
    public GameObject levelCompletePanel;  // Panel shown when all levels are completed

    [Header("Popups")]
    public GameObject strokePopup;         // Popup after finishing one stroke
    public GameObject levelPopup;          // Popup after completing a full letter/shape
    public float popupDuration = 1.5f;

    [Header("Navigation Buttons")]
    public Button nextButton;
    public Button backButton;

    [Header("Pause Menu")]
    public GameObject pauseMenuPanel;  
    private bool isPaused = false;

    [Header("Audio")]
    public AudioSource musicSource;    // Background music
    private bool isMusicOn = true;

    private int currentLevel = 0;
    private int strokesCompleted = 0;
    private int totalStrokesInLevel = 0;

    void Start()
    {
        // Disable all level panels at the start
        foreach (var panel in tracingPanels)
            panel.SetActive(false);

        if (levelCompletePanel) levelCompletePanel.SetActive(false);
        if (strokePopup) strokePopup.SetActive(false);
        if (levelPopup) levelPopup.SetActive(false);
        if (pauseMenuPanel) pauseMenuPanel.SetActive(false);

        if (nextButton) nextButton.interactable = false;
        if (backButton) backButton.interactable = false;

        // Start first level if available
        if (tracingPanels.Length > 0)
        {
            tracingPanels[0].SetActive(true);
            ResetPanelTracing(tracingPanels[0]);
            InitLevel(0);
        }
    }

    private void InitLevel(int levelIndex)
    {
        strokesCompleted = 0;

        // Count total strokes = number of paths across all TracingBrushManagers in this panel
        totalStrokesInLevel = 0;
        var brushManagers = tracingPanels[levelIndex].GetComponentsInChildren<TracingBrushManager>(true);
        foreach (var bm in brushManagers)
            totalStrokesInLevel += bm.GetStrokeCount();

        Debug.Log($"▶️ Level {levelIndex} started. Total strokes = {totalStrokesInLevel}");

        if (nextButton) nextButton.interactable = false;
        if (backButton) backButton.interactable = (levelIndex > 0);
    }

    // Called whenever one stroke is completed
    public void OnStrokeCompleted()
    {
        strokesCompleted++;
        Debug.Log($"✅ Stroke {strokesCompleted}/{totalStrokesInLevel} done in Level {currentLevel}");

        if (strokePopup) StartCoroutine(ShowPopup(strokePopup));

        // If all strokes are completed → full letter/shape completed
        if (strokesCompleted >= totalStrokesInLevel)
        {
            Debug.Log($"🎉 Letter/Shape {currentLevel} completed!");
            if (levelPopup) StartCoroutine(ShowPopup(levelPopup));
            if (nextButton) nextButton.interactable = true;
        }
    }

    public void OnNextLevel()
    {
        if (currentLevel < tracingPanels.Length)
            tracingPanels[currentLevel].SetActive(false);

        currentLevel++;

        if (currentLevel < tracingPanels.Length)
        {
            tracingPanels[currentLevel].SetActive(true);
            ResetPanelTracing(tracingPanels[currentLevel]);
            InitLevel(currentLevel);
        }
        else
        {
            Debug.Log("🏆 All Levels Completed!");
            if (levelCompletePanel) levelCompletePanel.SetActive(true);

            // Disable navigation buttons at the end
            if (nextButton) nextButton.interactable = false;
            if (backButton) backButton.interactable = false;
        }
    }

    // Skip to next level immediately
    public void OnSkipLevel()
    {
        Debug.Log("⏭ Skip pressed - moving to next level");
        OnNextLevel();
    }

    public void OnBackLevel()
    {
        if (currentLevel <= 0)
        {
            Debug.Log("◀️ Already at first level, can't go back further.");
            return;
        }

        if (currentLevel < tracingPanels.Length)
            tracingPanels[currentLevel].SetActive(false);

        currentLevel--;

        if (currentLevel >= 0)
        {
            tracingPanels[currentLevel].SetActive(true);
            ResetPanelTracing(tracingPanels[currentLevel]);
            InitLevel(currentLevel);
        }
        else
        {
            currentLevel = 0;
        }
    }

    private IEnumerator ShowPopup(GameObject popup)
    {
        popup.SetActive(true);
        yield return new WaitForSeconds(popupDuration);
        popup.SetActive(false);
    }

    public void OnBackToMainMenu(string sceneName)
    {
        Debug.Log("🔙 Returning to Main Menu...");
        SceneManager.LoadScene(sceneName);
    }

    public void OnExitGame()
    {
        Debug.Log("🚪 Exiting the game...");
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    public void OnPauseGame()
    {
        if (pauseMenuPanel) pauseMenuPanel.SetActive(true);
        Time.timeScale = 0f;  // Freeze game
        isPaused = true;
        Debug.Log("⏸️ Game Paused");
    }

    public void OnResumeGame()
    {
        if (pauseMenuPanel) pauseMenuPanel.SetActive(false);
        Time.timeScale = 1f;  // Resume game
        isPaused = false;
        Debug.Log("▶️ Game Resumed");
    }

    public void OnToggleMusic()
    {
        if (musicSource == null) return;

        isMusicOn = !isMusicOn;
        musicSource.mute = !isMusicOn;

        Debug.Log(isMusicOn ? "🎵 Music ON" : "🔇 Music OFF");
    }

    // Reset all TracingBrushManager scripts in a panel
    private void ResetPanelTracing(GameObject panel)
    {
        if (panel == null) return;
        var brushManagers = panel.GetComponentsInChildren<TracingBrushManager>(true);
        foreach (var bm in brushManagers)
        {
            if (bm != null)
                bm.ResetTracing();
        }
    }
}
