using UnityEngine;
using System.Collections;

/// <summary>
/// Gestiona los efectos de estado del jugador como ralentización,
/// congelamiento y daño en el tiempo.
/// </summary>
public class PlayerStatus : MonoBehaviour
{
    /// <summary>
    /// Referencia al controlador principal del jugador.
    /// </summary>
    private PlayerStateManager player;

    /// <summary>
    /// Velocidad original del jugador antes de aplicar efectos.
    /// </summary>
    private float originalSpeed;

    /// <summary>
    /// Indica si el jugador está congelado.
    /// </summary>
    private bool isFrozen = false;

    /// <summary>
    /// Indica si el jugador está ralentizado.
    /// </summary>
    private bool isSlowed = false;

    /// <summary>
    /// Temporizador del efecto de ralentización.
    /// </summary>
    private float slowTimer = 0f;

    /// <summary>
    /// Temporizador del efecto de congelamiento.
    /// </summary>
    private float freezeTimer = 0f;

    /// <summary>
    /// Inicializa referencias y guarda la velocidad original del jugador.
    /// </summary>
    private void Start()
    {
        player = GetComponent<PlayerStateManager>();
        originalSpeed = player.GetMoveSpeed();
    }

    /// <summary>
    /// Se ejecuta cada frame y gestiona los efectos activos.
    /// </summary>
    private void Update()
    {
        HandleSlow();
        HandleFreeze();
    }

    // =========================
    // SLOW
    // =========================

    /// <summary>
    /// Aplica un efecto de ralentización al jugador.
    /// </summary>
    /// <param name="duration">Duración del efecto en segundos.</param>
    /// <param name="slowMultiplier">Multiplicador de velocidad (ej. 0.5 = mitad de velocidad).</param>
    public void ApplySlow(float duration, float slowMultiplier)
    {
        player.SetMoveSpeed(originalSpeed * slowMultiplier);

        isSlowed = true;
        slowTimer = duration;

        Debug.Log("Jugador LENTO");
    }

    /// <summary>
    /// Gestiona la duración del efecto de ralentización.
    /// </summary>
    private void HandleSlow()
    {
        if (!isSlowed) return;

        slowTimer -= Time.deltaTime;

        if (slowTimer <= 0f)
        {
            player.SetMoveSpeed(originalSpeed);
            isSlowed = false;

            Debug.Log("Fin de lento");
        }
    }

    // =========================
    // FREEZE
    // =========================

    /// <summary>
    /// Aplica un efecto de congelamiento al jugador, deteniendo su movimiento.
    /// </summary>
    /// <param name="duration">Duración del efecto en segundos.</param>
    public void ApplyFreeze(float duration)
    {
        player.SetMoveSpeed(0f);

        isFrozen = true;
        freezeTimer = duration;

        Debug.Log("Jugador CONGELADO");
    }

    /// <summary>
    /// Gestiona la duración del efecto de congelamiento.
    /// </summary>
    private void HandleFreeze()
    {
        if (!isFrozen) return;

        freezeTimer -= Time.deltaTime;

        if (freezeTimer <= 0f)
        {
            player.SetMoveSpeed(originalSpeed);
            isFrozen = false;

            Debug.Log("Fin de congelado");
        }
    }

    // =========================
    // DAMAGE OVER TIME
    // =========================

    /// <summary>
    /// Aplica daño continuo al jugador durante un periodo de tiempo.
    /// </summary>
    /// <param name="damagePerSecond">Cantidad de daño aplicado por segundo.</param>
    /// <param name="duration">Duración total del efecto en segundos.</param>
    public void ApplyDamageOverTime(int damagePerSecond, float duration)
    {
        StartCoroutine(DamageCoroutine(damagePerSecond, duration));
    }

    /// <summary>
    /// Corrutina que aplica daño periódico al jugador.
    /// </summary>
    /// <param name="damage">Daño aplicado en cada intervalo.</param>
    /// <param name="duration">Duración total del efecto.</param>
    /// <returns>IEnumerator para la corrutina.</returns>
    private IEnumerator DamageCoroutine(int damage, float duration)
    {
        float timer = 0f;

        while (timer < duration)
        {
            player.TakeDamage(damage);

            yield return new WaitForSeconds(1f);

            timer += 1f;
        }
    }
}