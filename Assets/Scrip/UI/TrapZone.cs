using UnityEngine;

public class TrapZone : MonoBehaviour
{
    public enum TrapType
    {
        Slow,
        Freeze,
        Damage
    }

    [Header("Trap Type")]
    public TrapType trapType;

    [Header("Values")]
    public float duration = 2f;
    public float slowMultiplier = 0.5f;
    public int damagePerSecond = 1;

    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerStatus status = other.GetComponent<PlayerStatus>();

        if (status == null) return;

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