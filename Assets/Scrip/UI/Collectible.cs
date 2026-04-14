using UnityEngine;

/// <summary>
/// Representa una runa coleccionable en el juego.
/// Al entrar en contacto con el jugador, intenta añadirse a su inventario.
/// </summary>
public class RuneCollectible : MonoBehaviour
{
    /// <summary>
    /// Se ejecuta cuando un objeto entra en el trigger de la runa.
    /// Si el objeto tiene un PlayerRuneInventory, intenta recogerla.
    /// </summary>
    /// <param name="other">Collider del objeto que entra en contacto con la runa.</param>
    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerRuneInventory inventory = other.GetComponent<PlayerRuneInventory>();

        // Si el objeto tiene inventario de runas, intenta recolectar la runa
        if (inventory != null)
        {
            bool collected = inventory.AddRune();

            // Si la runa fue recolectada exitosamente, se destruye del escenario
            if (collected)
            {
                Destroy(gameObject);
            }
        }
    }
}