using UnityEngine;
using UnityEngine.InputSystem;

public class PauseManager : MonoBehaviour
{
    public GameObject pauseMenuUI;
    [SerializeField] private InputActionAsset	InputAction;
    private InputAction		_escapeAction;
    private bool isPaused = false;

    void Start()
    {
        _escapeAction = InputAction.FindActionMap("Player").FindAction("Escape");

    }
    
    void Update()
    {
        if (_escapeAction.WasPressedThisFrame())
        {
            if (isPaused)
                Resume();
            else
                Pause();
        }
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
    }

    void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
    }

    public void QuitGame()
    {
        // Pour quitter le jeu (ne marche pas dans l��diteur)
        Application.Quit();
    }
}
