using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

/// <summary>
/// Gestiona los diferentes estados de la interfaz de usuario del juego,
/// incluyendo menús, gameplay, pausa y pantalla de victoria.
/// También controla la activación de paneles y el flujo del tiempo.
/// </summary>
public class UIStateManager : MonoBehaviour
{
    /// <summary>
    /// Estados posibles de la interfaz de usuario.
    /// </summary>
    public enum UIState
    {
        /// <summary>Menú principal.</summary>
        MainMenu,

        /// <summary>Menú de opciones.</summary>
        Options,

        /// <summary>Estado de juego activo.</summary>
        Gameplay,

        /// <summary>Juego en pausa.</summary>
        Pause,

        /// <summary>Pantalla de victoria.</summary>
        Win
    }

    // =========================
    // PANELES DE UI
    // =========================

    /// <summary>Panel del menú principal.</summary>
    [Header("Panels")]
    [SerializeField] private GameObject panelMainMenu;

    /// <summary>Panel de opciones.</summary>
    [SerializeField] private GameObject panelOptions;

    /// <summary>Panel del HUD durante el gameplay.</summary>
    [SerializeField] private GameObject panelHUB;

    /// <summary>Panel de pausa.</summary>
    [SerializeField] private GameObject panelPause;

    /// <summary>Objeto del minimapa.</summary>
    [SerializeField] private GameObject MiniMapa;

    // =========================
    // DEBUG
    // =========================

    /// <summary>
    /// Texto de depuración que muestra el estado actual de la UI.
    /// </summary>
    [Header("Debug")]
    [SerializeField] private TextMeshProUGUI txtStateDebug;

    /// <summary>
    /// Estado actual de la interfaz.
    /// </summary>
    private UIState currentState;

    /// <summary>
    /// Inicializa el sistema en el menú principal.
    /// </summary>
    private void Start()
    {
        ChangeState(UIState.MainMenu);
    }

    /// <summary>
    /// Escucha entrada del usuario para alternar entre pausa y gameplay.
    /// </summary>
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

    /// <summary>
    /// Cambia el estado de la UI, activando y desactivando paneles
    /// y ajustando el flujo del tiempo.
    /// </summary>
    /// <param name="newState">Nuevo estado a aplicar.</param>
    public void ChangeState(UIState newState)
    {
        currentState = newState;

        // Apagar TODOS los paneles
        if (panelMainMenu != null) panelMainMenu.SetActive(false);
        if (panelOptions != null) panelOptions.SetActive(false);
        if (panelHUB != null) panelHUB.SetActive(false);
        if (panelPause != null) panelPause.SetActive(false);
        if (MiniMapa != null) MiniMapa.SetActive(false);

        switch (currentState)
        {
            case UIState.MainMenu:
                if (panelMainMenu != null) panelMainMenu.SetActive(true);
                MusicManager.Instance.PlayMenuMusic();
                Time.timeScale = 0f;
                break;

            case UIState.Options:
                if (panelOptions != null) panelOptions.SetActive(true);
                MusicManager.Instance.PlayMenuMusic();
                Time.timeScale = 0f;
                break;

            case UIState.Gameplay:
                if (panelHUB != null) panelHUB.SetActive(true);
                if (MiniMapa != null) MiniMapa.SetActive(true);
                MusicManager.Instance.PlayGameplayMusic();
                Time.timeScale = 1f;
                break;

            case UIState.Pause:
                if (panelPause != null) panelPause.SetActive(true);
                MusicManager.Instance.PlayGameplayMusic();
                Time.timeScale = 0f;
                break;

            case UIState.Win:
                // Se detiene el juego y se ocultan HUD/minimapa
                Time.timeScale = 0f;
                break;
        }

        // Actualizar texto de debug
        if (txtStateDebug != null)
            txtStateDebug.text = $"State: {currentState}";
    }

    // =========================
    // BOTONES UI
    // =========================

    /// <summary>
    /// Inicia el gameplay desde el menú principal.
    /// </summary>
    public void OnClickStart()
    {
        ChangeState(UIState.Gameplay);
    }

    /// <summary>
    /// Abre el menú de opciones.
    /// </summary>
    public void OnClickOptions()
    {
        ChangeState(UIState.Options);
    }

    /// <summary>
    /// Reinicia la escena actual (volver al menú principal).
    /// </summary>
    public void OnClickBackToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    /// <summary>
    /// Reanuda el juego desde pausa.
    /// </summary>
    public void OnClickResume()
    {
        ChangeState(UIState.Gameplay);
    }

    /// <summary>
    /// Activa el estado de pausa.
    /// </summary>
    public void OnClickPause()
    {
        ChangeState(UIState.Pause);
    }

    /// <summary>
    /// Sale del juego o detiene la ejecución en el editor.
    /// </summary>
    public void OnClickExit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}