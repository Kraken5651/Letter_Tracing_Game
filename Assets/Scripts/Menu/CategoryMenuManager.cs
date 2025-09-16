using UnityEngine;
using UnityEngine.SceneManagement;

public class CategoryMenuManager : MonoBehaviour
{
    public GameObject categoryPanel;
    public GameObject mainMenuPanel;

    // Switch from Category menu back to Main Menu
    public void OnBackToMain()
    {
        categoryPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }

    // Load Alphabets tracing scene
    public void OnAlphabetScene(string sceneName)
    {
        Debug.Log("Loading Alphabet Scene: " + sceneName);
        SceneManager.LoadScene(sceneName);
    }

    // Load Shapes tracing scene
    public void OnShapesScene(string sceneName)
    {
        Debug.Log("Loading Shapes Scene: " + sceneName);
        SceneManager.LoadScene(sceneName);
    }

    // Load Numbers tracing scene
    public void OnNumbersScene(string sceneName)
    {
        Debug.Log("Loading Numbers Scene: " + sceneName);
        SceneManager.LoadScene(sceneName);
    }
}
