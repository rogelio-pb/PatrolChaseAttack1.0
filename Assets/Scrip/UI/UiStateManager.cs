using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class UIStateManager : MonoBehaviour
{
    public enum UIState
    {
        MainMenu,
        Options,
        Gameplay,
        Pause
    }

    [Header("Panels")]
    [SerializeField] private GameObject panelMainMenu;
    [SerializeField] private GameObject panelOptions;
    [SerializeField] private GameObject panelHUB;
    [SerializeField] private GameObject panelPause;

    [Header("Debug")]
    [SerializeField] private TextMeshProUGUI txtStateDebug;

    private UIState currentState;

    private void Start()
    {
        ChangeState(UIState.MainMenu);
    }

    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (currentState == UIState.Gameplay)
            {
                ChangeState(UIState.Pause);
            }
            else if (currentState == UIState.Pause)
            {
                ChangeState(UIState.Gameplay);
            }
        }
    }

    public void ChangeState(UIState newState)
    {
        currentState = newState;

        // Apagar todos los paneles
        if (panelMainMenu != null) panelMainMenu.SetActive(false);
        if (panelOptions != null) panelOptions.SetActive(false);
        if (panelHUB != null) panelHUB.SetActive(false);
        if (panelPause != null) panelPause.SetActive(false);

        switch (currentState)
        {
            case UIState.MainMenu:
                if (panelMainMenu != null) panelMainMenu.SetActive(true);
                Time.timeScale = 0f;   //  PAUSA EL JUEGO
                break;

            case UIState.Options:
                if (panelOptions != null) panelOptions.SetActive(true);
                Time.timeScale = 0f;   // también pausado
                break;

            case UIState.Gameplay:
                if (panelHUB != null) panelHUB.SetActive(true);
                Time.timeScale = 1f;   // reanuda
                break;

            case UIState.Pause:
                if (panelPause != null) panelPause.SetActive(true);
                Time.timeScale = 0f;   // pausa
                break;
        }

        if (txtStateDebug != null)
            txtStateDebug.text = $"State: {currentState}";
    }

    public void OnClickStart()
    {
        ChangeState(UIState.Gameplay);
    }

    public void OnClickOptions()
    {
        ChangeState(UIState.Options);
    }

    public void OnClickBackToMainMenu()
    {
        Time.timeScale = 1f; 
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }


    public void OnClickResume()
    {
        ChangeState(UIState.Gameplay);
    }

    public void OnClickPause()
    {
        ChangeState(UIState.Pause);
    }

    public void OnClickExit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
