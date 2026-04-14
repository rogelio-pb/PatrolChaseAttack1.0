using UnityEngine;

public class RuneDepositZone : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerRuneInventory inventory = other.GetComponent<PlayerRuneInventory>();

        if (inventory != null)
        {
            inventory.DeliverRunes();
        }
    }
}