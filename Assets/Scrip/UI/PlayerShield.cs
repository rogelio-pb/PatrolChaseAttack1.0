using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShield : MonoBehaviour
{
    [Header("Shield Settings")]
    [SerializeField] private float shieldDuration = 2f;
    [SerializeField] private float cooldownTime = 5f;

    [Header("Visual")]
    [SerializeField] private GameObject shieldVisual;

    private bool isShieldActive = false;
    private bool isOnCooldown = false;

    private float shieldTimer = 0f;
    private float cooldownTimer = 0f;

    private PlayerStateManager player;

    private void Start()
    {
        player = GetComponent<PlayerStateManager>();

        if (shieldVisual != null)
            shieldVisual.SetActive(false);
    }

    private void Update()
    {
        // ACTIVAR ESCUDO (SHIFT)
        if (Keyboard.current.leftShiftKey.wasPressedThisFrame)
        {
            ActivateShield();
        }

        // DURACIÓN DEL ESCUDO
        if (isShieldActive)
        {
            shieldTimer -= Time.deltaTime;

            if (shieldTimer <= 0f)
            {
                DeactivateShield();
            }
        }

        // COOLDOWN
        if (isOnCooldown)
        {
            cooldownTimer -= Time.deltaTime;

            if (cooldownTimer <= 0f)
            {
                isOnCooldown = false;
                Debug.Log("Escudo listo otra vez");
            }
        }
    }

    private void ActivateShield()
    {
        if (isOnCooldown || isShieldActive) return;

        isShieldActive = true;
        shieldTimer = shieldDuration;

        if (shieldVisual != null)
            shieldVisual.SetActive(true);

        Debug.Log("Escudo ACTIVADO");
    }

    private void DeactivateShield()
    {
        isShieldActive = false;
        isOnCooldown = true;
        cooldownTimer = cooldownTime;

        if (shieldVisual != null)
            shieldVisual.SetActive(false);

        Debug.Log("Escudo DESACTIVADO");
    }

    // ESTE MÉTODO LO USA EL PLAYER PARA SABER SI TIENE ESCUDO
    public bool IsShieldActive()
    {
        return isShieldActive;
    }
}