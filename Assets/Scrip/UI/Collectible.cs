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
    /// 
    [SerializeField] private AudioClip collectSound;
    [SerializeField] private float volume = 1f;
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
                // Reproducir sonido en la posición de la runa
                if (collectSound != null)
                    AudioSource.PlayClipAtPoint(collectSound, transform.position, volume);

                Destroy(gameObject);
            }
        }
    }
}