using UnityEngine;
using TMPro;

/// <summary>
/// Gestiona el inventario de runas del jugador, incluyendo recolección,
/// entrega, puntuación y condición de victoria.
/// </summary>
public class PlayerRuneInventory : MonoBehaviour
{
    // =========================
    // CONFIGURACIÓN DE RUNAS
    // =========================

    /// <summary>
    /// Número máximo de runas que el jugador puede cargar.
    /// </summary>
    [Header("Rune Settings")]
    [SerializeField] private int maxRunes = 3;

    /// <summary>
    /// Cantidad actual de runas que lleva el jugador.
    /// </summary>
    [SerializeField] private int currentRunes = 0;

    /// <summary>
    /// Total de runas entregadas durante la partida.
    /// </summary>
    [SerializeField] private int totalRunesDelivered = 0;

    /// <summary>
    /// Cantidad de runas necesarias para ganar.
    /// </summary>
    [SerializeField] private int runesToWin = 10;

    // =========================
    // PUNTUACIÓN
    // =========================

    /// <summary>
    /// Puntos obtenidos por cada runa entregada.
    /// </summary>
    [Header("Score")]
    [SerializeField] private int pointsPerRune = 10;

    // =========================
    // INTERFAZ DE USUARIO
    // =========================

    /// <summary>
    /// Texto de UI que muestra las runas actuales.
    /// </summary>
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI runeText;

    /// <summary>
    /// Referencia al controlador de estados del jugador.
    /// </summary>
    private PlayerStateManager player;

    /// <summary>
    /// Inicializa referencias al iniciar el juego.
    /// </summary>
    private void Start()
    {
        player = GetComponent<PlayerStateManager>();
        UpdateUI();
    }

    // =========================
    // RECOGER RUNA
    // =========================

    /// <summary>
    /// Intenta agregar una runa al inventario del jugador.
    /// </summary>
    /// <returns>
    /// true si la runa fue añadida correctamente, false si el inventario está lleno.
    /// </returns>
    public bool AddRune()
    {
        if (currentRunes >= maxRunes)
        {
            Debug.Log("Inventario lleno");
            return false;
        }

        currentRunes++;
        UpdateUI();

        Debug.Log("Runas cargadas: " + currentRunes);
        return true;
    }

    // =========================
    // ACCEDER A DATOS
    // =========================

    /// <summary>
    /// Obtiene el total de runas entregadas.
    /// </summary>
    /// <returns>Total de runas entregadas.</returns>
    public int GetTotalRunesDelivered()
    {
        return totalRunesDelivered;
    }

    // =========================
    // ENTREGAR RUNAS
    // =========================

    /// <summary>
    /// Entrega todas las runas actuales, suma puntos y verifica condición de victoria.
    /// </summary>
    public void DeliverRunes()
    {
        if (currentRunes <= 0) return;

        // Calcular puntos obtenidos
        int totalPoints = currentRunes * pointsPerRune;

        // Añadir puntos al jugador
        player.AddPoints(totalPoints);

        // Actualizar total de runas entregadas
        totalRunesDelivered += currentRunes;

        Debug.Log("Runas entregadas: " + currentRunes);
        Debug.Log("Total entregadas: " + totalRunesDelivered);

        // Reiniciar runas actuales
        currentRunes = 0;
        UpdateUI();

        // Verificar condición de victoria
        if (totalRunesDelivered >= runesToWin)
        {
            player.WinFromRunes();
        }
    }

    /// <summary>
    /// Actualiza el texto de la interfaz de usuario con las runas actuales.
    /// </summary>
    private void UpdateUI()
    {
        if (runeText != null)
            runeText.text = $"Runes: {currentRunes}/{maxRunes}";
    }
}