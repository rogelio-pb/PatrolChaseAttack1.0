using UnityEngine;

/// <summary>
/// Representa una zona de trampa que aplica efectos al jugador
/// al entrar en su área de colisión.
/// </summary>
public class TrapZone : MonoBehaviour
{
    /// <summary>
    /// Tipos de trampas disponibles.
    /// </summary>
    public enum TrapType
    {
        /// <summary>
        /// Reduce la velocidad del jugador.
        /// </summary>
        Slow,

        /// <summary>
        /// Inmoviliza al jugador temporalmente.
        /// </summary>
        Freeze,

        /// <summary>
        /// Aplica daño continuo al jugador.
        /// </summary>
        Damage
    }

    // =========================
    // CONFIGURACIÓN DE TRAMPA
    // =========================

    /// <summary>
    /// Tipo de trampa que se aplicará.
    /// </summary>
    [Header("Trap Type")]
    public TrapType trapType;

    /// <summary>
    /// Duración del efecto de la trampa en segundos.
    /// </summary>
    [Header("Values")]
    public float duration = 2f;

    /// <summary>
    /// Multiplicador de velocidad cuando se aplica el efecto Slow.
    /// </summary>
    public float slowMultiplier = 0.5f;

    /// <summary>
    /// Cantidad de daño por segundo para la trampa de tipo Damage.
    /// </summary>
    public int damagePerSecond = 1;

    // =========================
    // DETECCIÓN DE COLISIÓN
    // =========================

    /// <summary>
    /// Se ejecuta cuando un objeto entra en el trigger de la trampa.
    /// Aplica el efecto correspondiente si el objeto tiene un componente PlayerStatus.
    /// </summary>
    /// <param name="other">Collider del objeto que entra en la trampa.</param>
    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerStatus status = other.GetComponent<PlayerStatus>();

        // Si el objeto no tiene PlayerStatus, no se aplica ningún efecto
        if (status == null) return;

        // Aplicar efecto según el tipo de trampa
        switch (trapType)
        {
            case TrapType.Slow:
                status.ApplySlow(duration, slowMultiplier);
                break;

            case TrapType.Freeze:
                status.ApplyFreeze(duration);
                break;

            case TrapType.Damage:
                status.ApplyDamageOverTime(damagePerSecond, duration);
                break;
        }
    }
}