using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerStateManager : MonoBehaviour
{
    // =========================================================
    // Inspector: Movement
    // =========================================================
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private bool rotateForVertical = true;

    // =========================================================
    // Inspector: Screen Wrap
    // =========================================================
    [Header("Screen Wrap")]
    [SerializeField] private float minX;
    [SerializeField] private float maxX;
    [SerializeField] private float minY;
    [SerializeField] private float maxY;

    // =========================================================
    // Inspector: Animation
    // =========================================================
    [Header("Animation")]
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer spriteRenderer;

    // =========================================================
    // Inspector: Debug
    // =========================================================
    [Header("Debug")]
    [SerializeField] private bool logWarnings = true;
    [SerializeField] private bool logMovement = false;
    [SerializeField] private TextMeshProUGUI txtStateDebug;

    // =========================================================
    // Inspector: Health
    // =========================================================
    [Header("Health")]
    [SerializeField] private int maxHealth = 5;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TextMeshProUGUI healthText;

    // =========================================================
    // Inspector: Score System
    // =========================================================
    [Header("Score System")]
    [SerializeField] private int currentPoints = 0;
    [SerializeField] private TextMeshProUGUI pointsText;
    [SerializeField] private GameObject winPanel;

    // =========================================================
    // Inspector: Score Gameplay
    // =========================================================
    [Header("Score Gameplay")]
    [SerializeField] private int runePoints = 10;
    [SerializeField] private int survivalPointsPerSecond = 1;
    [SerializeField] private int healthBonusPoints = 20;

    [SerializeField] private int totalRunes = 3; // runas necesarias para ganar
    [SerializeField] private TextMeshProUGUI finalScoreText;

    // =========================================================
    // Private runtime fields
    // =========================================================
    private Rigidbody2D rb;
    private Vector2 moveInput;

    private int currentHealth;
    private bool isDead = false;
    private bool hasWon = false;

    private float survivalTimer = 0f;
    private int survivalSeconds = 0;

    private int runesCollected = 0;

    private static readonly int HashIsMoving = Animator.StringToHash("isMoving");
    private static readonly int HashMoveX = Animator.StringToHash("moveX");
    private static readonly int HashMoveY = Animator.StringToHash("moveY");
    private static readonly int HashHit = Animator.StringToHash("hit");
    private static readonly int HashDie = Animator.StringToHash("die");

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

    private void Update()
    {
        // =====================================================
        // PUNTOS POR SUPERVIVENCIA
        // =====================================================
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

    private void FixedUpdate()
    {
        if (isDead || hasWon) return;

        Vector2 nextPos = rb.position + moveInput * moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(nextPos);
    }

    private void UpdateAnimator(Vector2 input)
    {
        if (animator == null) return;

        bool isMoving = input.sqrMagnitude > 0.0001f;

        animator.SetBool(HashIsMoving, isMoving);
        animator.SetFloat(HashMoveX, input.x);
        animator.SetFloat(HashMoveY, input.y);

        if (spriteRenderer == null || !isMoving) return;

        bool verticalDominant = Mathf.Abs(input.y) > Mathf.Abs(input.x);

        if (verticalDominant)
        {
            spriteRenderer.flipX = false;

            if (input.y > 0.01f)
                spriteRenderer.transform.localEulerAngles = new Vector3(0f, 0f, 90f);
            else if (input.y < -0.01f)
                spriteRenderer.transform.localEulerAngles = new Vector3(0f, 0f, -90f);
        }
        else
        {
            spriteRenderer.transform.localEulerAngles = Vector3.zero;

            if (Mathf.Abs(input.x) > 0.01f)
                spriteRenderer.flipX = input.x < 0f;
        }
    }

    // =========================================================
    // Damage System
    // =========================================================
    public void TakeDamage(int amount)
    {
        if (isDead || hasWon) return;

        currentHealth -= amount;

        if (animator != null)
            animator.SetTrigger(HashHit);

        UpdateHealthUI();

        if (currentHealth <= 0)
            Die();
    }

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

    private void ShowGameOver()
    {
        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);

        Time.timeScale = 0f;
    }

    // =========================================================
    // Rune System
    // =========================================================
    public void CollectRune()
    {
        runesCollected++;

        Debug.Log("Runas recolectadas: " + runesCollected);

        AddPoints(runePoints);

        if (runesCollected >= totalRunes)
        {
            Debug.Log("GANASTE");
            WinGame();
        }
    }

    // =========================================================
    // Score System
    // =========================================================
    public void AddPoints(int amount)
    {
        if (isDead || hasWon) return;

        currentPoints += amount;
        UpdatePointsUI();
    }

    private void WinGame()
    {
        hasWon = true;

        int runeScore = runesCollected * runePoints;
        int survivalScore = survivalSeconds * survivalPointsPerSecond;
        int healthBonus = currentHealth * healthBonusPoints;

        int finalScore = runeScore + survivalScore + healthBonus;

        currentPoints += healthBonus;

        UpdatePointsUI();

        Debug.Log("Runas: " + runeScore);
        Debug.Log("Supervivencia: " + survivalScore);
        Debug.Log("Bonus vida: " + healthBonus);
        Debug.Log("Score final: " + finalScore);

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

    private void UpdatePointsUI()
    {
        if (pointsText != null)
            pointsText.text = $"Puntos: {currentPoints}";
    }

    // =========================================================
    // Health UI
    // =========================================================
    private void UpdateHealthUI()
    {
        if (healthText != null)
            healthText.text = $"Vida: {currentHealth}";
    }
}