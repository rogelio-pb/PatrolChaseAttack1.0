using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Gestiona la habilidad de escudo del jugador, incluyendo activación,
/// duración, cooldown y representación visual.
/// </summary>
public class PlayerShield : MonoBehaviour
{
    // =========================
    // CONFIGURACIÓN DEL ESCUDO
    // =========================

    /// <summary>
    /// Duración activa del escudo en segundos.
    /// </summary>
    [Header("Shield Settings")]
    [SerializeField] private float shieldDuration = 2f;

    /// <summary>
    /// Tiempo de espera antes de poder volver a usar el escudo.
    /// </summary>
    [SerializeField] private float cooldownTime = 5f;

    // =========================
    // VISUAL
    // =========================

    /// <summary>
    /// Objeto visual que representa el escudo.
    /// </summary>
    [Header("Visual")]
    [SerializeField] private GameObject shieldVisual;

    /// <summary>
    /// Indica si el escudo está actualmente activo.
    /// </summary>
    private bool isShieldActive = false;

    /// <summary>
    /// Indica si el escudo está en cooldown.
    /// </summary>
    private bool isOnCooldown = false;

    /// <summary>
    /// Temporizador de duración del escudo.
    /// </summary>
    private float shieldTimer = 0f;

    /// <summary>
    /// Temporizador del cooldown del escudo.
    /// </summary>
    private float cooldownTimer = 0f;

    /// <summary>
    /// Referencia al controlador del jugador.
    /// </summary>
    private PlayerStateManager player;

    /// <summary>
    /// Inicializa referencias y desactiva el visual del escudo.
    /// </summary>
    private void Start()
    {
        player = GetComponent<PlayerStateManager>();

        if (shieldVisual != null)
            shieldVisual.SetActive(false);
    }

    /// <summary>
    /// Se ejecuta cada frame para gestionar entrada, duración y cooldown del escudo.
    /// </summary>
    private void Update()
    {
        // ACTIVAR ESCUDO (SHIFT)
        if (Keyboard.current.leftShiftKey.wasPressedThisFrame)
        {
            ActivateShield();
        }

        // DURACIÓN DEL ESCUDO
        if (isShieldActive)
        {
            shieldTimer -= Time.deltaTime;

            if (shieldTimer <= 0f)
            {
                DeactivateShield();
            }
        }

        // COOLDOWN
        if (isOnCooldown)
        {
            cooldownTimer -= Time.deltaTime;

            if (cooldownTimer <= 0f)
            {
                isOnCooldown = false;
                Debug.Log("Escudo listo otra vez");
            }
        }
    }

    /// <summary>
    /// Activa el escudo si no está activo ni en cooldown.
    /// </summary>
    private void ActivateShield()
    {
        if (isOnCooldown || isShieldActive) return;

        isShieldActive = true;
        shieldTimer = shieldDuration;

        if (shieldVisual != null)
            shieldVisual.SetActive(true);

        Debug.Log("Escudo ACTIVADO");
    }

    /// <summary>
    /// Desactiva el escudo y activa el cooldown.
    /// </summary>
    private void DeactivateShield()
    {
        isShieldActive = false;
        isOnCooldown = true;
        cooldownTimer = cooldownTime;

        if (shieldVisual != null)
            shieldVisual.SetActive(false);

        Debug.Log("Escudo DESACTIVADO");
    }

    // =========================
    // ACCESO EXTERNO
    // =========================

    /// <summary>
    /// Indica si el escudo está actualmente activo.
    /// </summary>
    /// <returns>True si el escudo está activo, false en caso contrario.</returns>
    public bool IsShieldActive()
    {
        return isShieldActive;
    }
}