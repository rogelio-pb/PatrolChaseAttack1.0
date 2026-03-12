using UnityEngine;

public class WrapZone : MonoBehaviour
{
    [SerializeField] private Transform targetPoint;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        Rigidbody2D rb = other.GetComponent<Rigidbody2D>();
        if (rb == null) return;

        rb.position = targetPoint.position;
    }
}