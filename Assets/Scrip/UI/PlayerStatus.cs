using UnityEngine;

public class PlayerStatus : MonoBehaviour
{
    private PlayerStateManager player;

    private float originalSpeed;

    private bool isFrozen = false;
    private bool isSlowed = false;

    private float slowTimer = 0f;
    private float freezeTimer = 0f;

    private float damageTimer = 0f;

    private void Start()
    {
        player = GetComponent<PlayerStateManager>();
        originalSpeed = player.GetMoveSpeed();
    }

    private void Update()
    {
        HandleSlow();
        HandleFreeze();
    }

    // =========================
    // SLOW
    // =========================
    public void ApplySlow(float duration, float slowMultiplier)
    {
        player.SetMoveSpeed(originalSpeed * slowMultiplier);

        isSlowed = true;
        slowTimer = duration;

        Debug.Log("Jugador LENTO");
    }

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
    public void ApplyFreeze(float duration)
    {
        player.SetMoveSpeed(0f);

        isFrozen = true;
        freezeTimer = duration;

        Debug.Log("Jugador CONGELADO");
    }

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
    public void ApplyDamageOverTime(int damagePerSecond, float duration)
    {
        StartCoroutine(DamageCoroutine(damagePerSecond, duration));
    }

    private System.Collections.IEnumerator DamageCoroutine(int damage, float duration)
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