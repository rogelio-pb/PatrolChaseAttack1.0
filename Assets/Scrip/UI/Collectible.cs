using UnityEngine;

public class RuneCollectible : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerStateManager player = other.GetComponent<PlayerStateManager>();

        if (player != null)
        {
            player.CollectRune();

            Destroy(gameObject);
        }
    }
}