using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseMenuUI : MonoBehaviour {
    // --------------------
    //     References
    // --------------------
    [Header("References")]
    public GameObject pauseMenuPanel;

    private PlayerControls playerControls;
    private bool isPaused = false;

    // =====================================================================
    //                          Unity Methods
    // =====================================================================
    private void Awake() {
        playerControls = new PlayerControls();

        playerControls.UI.Pause.performed += _ => TogglePause();
    }

    private void Start() {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void OnEnable() {
        playerControls.Enable();
    }

    private void OnDisable() {
        playerControls.Disable();
    }

    // =====================================================================
    //                          Pause Methods
    // =====================================================================
    public void TogglePause() {
        isPaused = !isPaused;

        pauseMenuPanel.SetActive(isPaused);
        Time.timeScale = isPaused ? 0 : 1;

        if (isPaused) {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        } else {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    public void ResumeGame() {
        isPaused = false;
        pauseMenuPanel.SetActive(false);
        Time.timeScale = 1;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void OpenSettings() {
        Debug.Log("Settings menu opened (not implemented yet).");
        //TO-DO: Adding Settings
    }

    public void QuitToMenu() {
        Time.timeScale = 1;
        SceneManager.LoadScene("MainMenu");
    }

    public void QuitToDesktop() {
        Debug.Log("Quitting to desktop...");
        Application.Quit();
    }
}
