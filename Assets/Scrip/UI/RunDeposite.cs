using UnityEngine;

/// <summary>
/// Zona donde el jugador puede entregar sus runas.
/// Al entrar en el área de colisión, se depositan automáticamente.
/// </summary>
public class RuneDepositZone : MonoBehaviour
{
    /// <summary>
    /// Se ejecuta cuando un objeto entra en el trigger de la zona de depósito.
    /// Si el objeto tiene un PlayerRuneInventory, se entregan las runas.
    /// </summary>
    /// <param name="other">Collider del objeto que entra en la zona.</param>
    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerRuneInventory inventory = other.GetComponent<PlayerRuneInventory>();

        // Si el objeto tiene inventario de runas, se entregan
        if (inventory != null)
        {
            inventory.DeliverRunes();
        }
    }
}