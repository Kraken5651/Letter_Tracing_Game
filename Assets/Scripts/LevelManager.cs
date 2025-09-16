using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static string selectedLetter; // Stores which letter was picked

    public void SelectLetter(string letter)
    {
        selectedLetter = letter;
        SceneManager.LoadScene("TracingScene");
    }
}
