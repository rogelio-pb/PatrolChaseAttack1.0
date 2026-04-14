using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

/// <summary>
/// Controlador principal del jugador. Gestiona movimiento, animaciones,
/// vida, daño, sistema de puntos y condición de victoria/derrota.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerStateManager : MonoBehaviour
{
    // =========================================================
    // MOVIMIENTO
    // =========================================================

    /// <summary>Velocidad de movimiento del jugador.</summary>
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;

    // =========================================================
    // SCREEN WRAP (No implementado aún en lógica)
    // =========================================================

    /// <summary>Límite mínimo en X.</summary>
    [Header("Screen Wrap")]
    [SerializeField] private float minX;

    /// <summary>Límite máximo en X.</summary>
    [SerializeField] private float maxX;

    /// <summary>Límite mínimo en Y.</summary>
    [SerializeField] private float minY;

    /// <summary>Límite máximo en Y.</summary>
    [SerializeField] private float maxY;

    // =========================================================
    // ANIMACIÓN
    // =========================================================

    /// <summary>Animator del jugador.</summary>
    [Header("Animation")]
    [SerializeField] private Animator animator;

    /// <summary>SpriteRenderer del jugador.</summary>
    [SerializeField] private SpriteRenderer spriteRenderer;

    // =========================================================
    // DEBUG
    // =========================================================

    /// <summary>Activa logs de advertencia.</summary>
    [Header("Debug")]
    [SerializeField] private bool logWarnings = true;

    /// <summary>Activa logs de movimiento.</summary>
    [SerializeField] private bool logMovement = false;

    /// <summary>Texto de depuración del estado.</summary>
    [SerializeField] private TextMeshProUGUI txtStateDebug;

    // =========================================================
    // VIDA
    // =========================================================

    /// <summary>Vida máxima del jugador.</summary>
    [Header("Health")]
    [SerializeField] private int maxHealth = 5;

    /// <summary>Panel de Game Over.</summary>
    [SerializeField] private GameObject gameOverPanel;

    /// <summary>Texto de vida en UI.</summary>
    [SerializeField] private TextMeshProUGUI healthText;

    // =========================================================
    // PUNTOS
    // =========================================================

    /// <summary>Puntos actuales del jugador.</summary>
    [Header("Score System")]
    [SerializeField] private int currentPoints = 0;

    /// <summary>Texto de puntos en UI.</summary>
    [SerializeField] private TextMeshProUGUI pointsText;

    /// <summary>Panel de victoria.</summary>
    [SerializeField] private GameObject winPanel;

    // =========================================================
    // CONFIGURACIÓN DE SCORE
    // =========================================================

    /// <summary>Puntos por runa entregada.</summary>
    [Header("Score Gameplay")]
    [SerializeField] private int runePoints = 10;

    /// <summary>Puntos ganados por segundo sobrevivido.</summary>
    [SerializeField] private int survivalPointsPerSecond = 1;

    /// <summary>Bonus de puntos por cada punto de vida restante.</summary>
    [SerializeField] private int healthBonusPoints = 20;

    /// <summary>Texto final del score.</summary>
    [SerializeField] private TextMeshProUGUI finalScoreText;

    // =========================================================
    // VARIABLES INTERNAS
    // =========================================================

    /// <summary>Referencia al Rigidbody2D.</summary>
    private Rigidbody2D rb;

    /// <summary>Input de movimiento actual.</summary>
    private Vector2 moveInput;

    /// <summary>Vida actual del jugador.</summary>
    private int currentHealth;

    /// <summary>Indica si el jugador está muerto.</summary>
    private bool isDead = false;

    /// <summary>Indica si el jugador ha ganado.</summary>
    private bool hasWon = false;

    /// <summary>Temporizador de supervivencia.</summary>
    private float survivalTimer = 0f;

    /// <summary>Segundos sobrevividos.</summary>
    private int survivalSeconds = 0;

    // Hashes de animación
    private static readonly int HashIsMoving = Animator.StringToHash("isMoving");
    private static readonly int HashMoveX = Animator.StringToHash("moveX");
    private static readonly int HashMoveY = Animator.StringToHash("moveY");
    private static readonly int HashHit = Animator.StringToHash("hit");
    private static readonly int HashDie = Animator.StringToHash("die");

    /// <summary>
    /// Inicializa componentes, vida y UI.
    /// </summary>
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        rb.gravityScale = 0f;
        rb.freezeRotation = true;

        if (animator == null) animator = GetComponentInChildren<Animator>(true);
        if (spriteRenderer == null) spriteRenderer = GetComponentInChildren<SpriteRenderer>(true);

        if (logWarnings)
        {
            if (animator == null)
                Debug.LogWarning("[PlayerStateManager] Animator NOT found.");

            if (spriteRenderer == null)
                Debug.LogWarning("[PlayerStateManager] SpriteRenderer NOT found.");
        }

        currentHealth = maxHealth;
        UpdateHealthUI();

        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        if (winPanel != null)
            winPanel.SetActive(false);

        UpdatePointsUI();
    }

    /// <summary>
    /// Gestiona entrada, animaciones y puntos por supervivencia.
    /// </summary>
    private void Update()
    {
        // Puntos por supervivencia
        if (!isDead && !hasWon)
        {
            survivalTimer += Time.deltaTime;

            if (survivalTimer >= 1f)
            {
                survivalSeconds++;
                AddPoints(survivalPointsPerSecond);
                survivalTimer = 0f;
            }
        }

        if (isDead || hasWon)
        {
            moveInput = Vector2.zero;
            UpdateAnimator(moveInput);
            return;
        }

        if (Keyboard.current == null)
        {
            moveInput = Vector2.zero;
            UpdateAnimator(moveInput);
            return;
        }

        float x = 0f;
        float y = 0f;

        if (Keyboard.current.aKey.isPressed) x -= 1f;
        if (Keyboard.current.dKey.isPressed) x += 1f;
        if (Keyboard.current.wKey.isPressed) y += 1f;
        if (Keyboard.current.sKey.isPressed) y -= 1f;

        moveInput = new Vector2(x, y).normalized;

        if (logMovement)
            Debug.Log($"[PlayerStateManager] moveInput={moveInput}");

        UpdateAnimator(moveInput);
    }

    /// <summary>
    /// Aplica el movimiento físico del jugador.
    /// </summary>
    private void FixedUpdate()
    {
        if (isDead || hasWon) return;

        Vector2 nextPos = rb.position + moveInput * moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(nextPos);
    }

    /// <summary>
    /// Actualiza parámetros del Animator según el input.
    /// </summary>
    /// <param name="input">Vector de movimiento.</param>
    private void UpdateAnimator(Vector2 input)
    {
        if (animator == null) return;

        bool isMoving = input.sqrMagnitude > 0.0001f;

        animator.SetBool(HashIsMoving, isMoving);
        animator.SetFloat(HashMoveX, input.x);
        animator.SetFloat(HashMoveY, input.y);

        if (spriteRenderer == null || !isMoving) return;

        spriteRenderer.transform.localEulerAngles = Vector3.zero;

        if (Mathf.Abs(input.x) > 0.01f)
        {
            spriteRenderer.flipX = input.x < 0f;
        }
    }

    // =========================================================
    // DAMAGE SYSTEM
    // =========================================================

    /// <summary>
    /// Aplica daño al jugador, considerando escudo activo.
    /// </summary>
    /// <param name="amount">Cantidad de daño.</param>
    public void TakeDamage(int amount)
    {
        if (isDead || hasWon) return;

        PlayerShield shield = GetComponent<PlayerShield>();

        if (shield != null && shield.IsShieldActive())
        {
            Debug.Log("DAÑO BLOQUEADO POR ESCUDO");
            return;
        }

        currentHealth -= amount;

        if (animator != null)
            animator.SetTrigger(HashHit);

        UpdateHealthUI();

        if (currentHealth <= 0)
            Die();
    }

    /// <summary>
    /// Maneja la muerte del jugador.
    /// </summary>
    private void Die()
    {
        if (isDead) return;

        isDead = true;

        if (animator != null)
            animator.SetTrigger(HashDie);

        moveInput = Vector2.zero;
        rb.linearVelocity = Vector2.zero;

        Invoke(nameof(ShowGameOver), 1f);
    }

    /// <summary>
    /// Muestra la pantalla de Game Over.
    /// </summary>
    private void ShowGameOver()
    {
        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);

        Time.timeScale = 0f;
    }

    // =========================================================
    // VICTORIA
    // =========================================================

    /// <summary>
    /// Se llama cuando el jugador gana mediante runas.
    /// </summary>
    public void WinFromRunes()
    {
        if (hasWon) return;

        Debug.Log("GANASTE POR RUNAS");
        WinGame();
    }

    // =========================================================
    // PUNTOS
    // =========================================================

    /// <summary>
    /// Añade puntos al jugador.
    /// </summary>
    /// <param name="amount">Cantidad de puntos.</param>
    public void AddPoints(int amount)
    {
        if (isDead || hasWon) return;

        currentPoints += amount;
        UpdatePointsUI();
    }

    /// <summary>
    /// Calcula el score final y muestra la pantalla de victoria.
    /// </summary>
    private void WinGame()
    {
        hasWon = true;

        PlayerRuneInventory inventory = GetComponent<PlayerRuneInventory>();

        int deliveredRunes = (inventory != null) ? inventory.GetTotalRunesDelivered() : 0;

        int runeScore = deliveredRunes * runePoints;
        int survivalScore = survivalSeconds * survivalPointsPerSecond;
        int healthBonus = currentHealth * healthBonusPoints;

        int finalScore = runeScore + survivalScore + healthBonus;

        currentPoints += healthBonus;

        UpdatePointsUI();

        if (finalScoreText != null)
        {
            finalScoreText.text =
            "RUNAS: " + runeScore +
            "\nTIEMPO: " + survivalScore +
            "\nVIDA BONUS: " + healthBonus +
            "\n\nSCORE FINAL: " + finalScore;
        }

        if (winPanel != null)
            winPanel.SetActive(true);

        Time.timeScale = 0f;
    }

    /// <summary>
    /// Actualiza el texto de puntos en UI.
    /// </summary>
    private void UpdatePointsUI()
    {
        if (pointsText != null)
            pointsText.text = $"Puntos: {currentPoints}";
    }

    // =========================================================
    // VIDA UI
    // =========================================================

    /// <summary>
    /// Actualiza el texto de vida en UI.
    /// </summary>
    private void UpdateHealthUI()
    {
        if (healthText != null)
            healthText.text = $"Vida: {currentHealth}";
    }

    // =========================================================
    // GETTERS & SETTERS
    // =========================================================

    /// <summary>
    /// Obtiene la velocidad de movimiento actual.
    /// </summary>
    /// <returns>Velocidad del jugador.</returns>
    public float GetMoveSpeed()
    {
        return moveSpeed;
    }

    /// <summary>
    /// Establece una nueva velocidad de movimiento.
    /// </summary>
    /// <param name="newSpeed">Nueva velocidad.</param>
    public void SetMoveSpeed(float newSpeed)
    {
        moveSpeed = newSpeed;
    }
}