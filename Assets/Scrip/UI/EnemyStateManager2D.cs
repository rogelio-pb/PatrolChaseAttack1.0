
using TMPro;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyStateManager2D : MonoBehaviour
{
    public enum EnemyState
    {
        Patrol,
        Chase,
        Attack
    }

    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private Transform[] waypoints;
    [SerializeField] private TextMeshProUGUI txtStateDebug;

    [Header("Ranges")]
    [SerializeField] private float detectionRange = 6f;
    [SerializeField] private float attackRange = 1.5f;

    [Header("Movement")]
    [SerializeField] private float patrolSpeed = 2f;
    [SerializeField] private float chaseSpeed = 3.5f;

    [Header("Attack")]
    [SerializeField] private float attackCooldown = 1.2f;

    [Header("Damage")]
    [SerializeField] private int damageAmount = 1;

    [Header("Animation")]
    [SerializeField] private Animator animator;

    private static readonly int HashIsChasing = Animator.StringToHash("isChasing");
    private static readonly int HashAttack = Animator.StringToHash("attack");

    private EnemyState currentState;
    private int currentWaypointIndex;
    private float attackTimer;

    private Rigidbody2D rb;

    //  Movimiento físico controlado correctamente
    private Vector2 moveDirection;
    private float currentSpeed;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.freezeRotation = true;
    }

    void Start()
    {
        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        ChangeState(EnemyState.Patrol);
    }

    void Update()
    {
        switch (currentState)
        {
            case EnemyState.Patrol:
                Patrol();
                break;

            case EnemyState.Chase:
                Chase();
                break;

            case EnemyState.Attack:
                Attack();
                break;
        }

        if (txtStateDebug != null)
            txtStateDebug.text = $"Enemy State: {currentState}";
    }

    private void FixedUpdate()
    {
        Vector2 newPos = rb.position + moveDirection * currentSpeed * Time.fixedDeltaTime;
        rb.MovePosition(newPos);
    }

    private void ChangeState(EnemyState newState)
    {
        currentState = newState;
        attackTimer = attackCooldown;
    }

    // =========================================================
    // STATES
    // =========================================================

    private void Patrol()
    {
        if (animator != null)
            animator.SetBool(HashIsChasing, false);

        MoveTo(waypoints[currentWaypointIndex], patrolSpeed);

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer <= detectionRange)
        {
            ChangeState(EnemyState.Chase);
            return;
        }

        if (Vector2.Distance(transform.position, waypoints[currentWaypointIndex].position) < 0.2f)
        {
            int nextIndex = currentWaypointIndex;

            while (nextIndex == currentWaypointIndex && waypoints.Length > 1)
                nextIndex = Random.Range(0, waypoints.Length);

            currentWaypointIndex = nextIndex;
        }
    }

    private void Chase()
    {
        if (animator != null)
            animator.SetBool(HashIsChasing, true);

        MoveTo(player, chaseSpeed);

        float distance = Vector2.Distance(transform.position, player.position);

        if (distance <= attackRange)
        {
            ChangeState(EnemyState.Attack);
            return;
        }

        if (distance > detectionRange)
        {
            ChangeState(EnemyState.Patrol);
        }
    }

    private void Attack()
    {
        moveDirection = Vector2.zero;
        currentSpeed = 0f;

        if (animator != null)
            animator.SetBool(HashIsChasing, false);

        float distance = Vector2.Distance(transform.position, player.position);

        if (distance > attackRange)
        {
            ChangeState(EnemyState.Chase);
            return;
        }

        attackTimer -= Time.deltaTime;

        if (attackTimer <= 0f)
        {
            if (animator != null)
                animator.SetTrigger(HashAttack);

            PlayerStateManager playerHealth = player.GetComponent<PlayerStateManager>();
            if (playerHealth != null)
                playerHealth.TakeDamage(damageAmount);

            attackTimer = attackCooldown;
        }
    }

    // =========================================================
    // MOVEMENT
    // =========================================================

    private void MoveTo(Transform target, float speed)
    {
        moveDirection = (target.position - transform.position).normalized;
        currentSpeed = speed;

        SpriteRenderer sr = GetComponentInChildren<SpriteRenderer>();
        if (sr != null && Mathf.Abs(moveDirection.x) > 0.01f)
            sr.flipX = moveDirection.x < 0f;
    }
}