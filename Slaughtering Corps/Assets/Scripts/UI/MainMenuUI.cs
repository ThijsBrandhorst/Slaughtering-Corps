using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour {
    public void PlayGame() {
        SceneManager.LoadScene("GameScene");
    }

    public void OpenSettings() {
        Debug.Log("Settings...");
    }

    public void OpenCredits() {
        Debug.Log("Credits...");
    }

    public void QuitGame() {
        Debug.Log("Quitting to Deskop...");
        Application.Quit();
    }
}
