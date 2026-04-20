using TMPro;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyShooter : MonoBehaviour
{
    public enum State { Patrol, Attack }

    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private Transform[] waypoints;
    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private TextMeshProUGUI txtStateDebug;

    [Header("Ranges")]
    [SerializeField] private float detectionRange = 6f;

    [Header("Movement")]
    [SerializeField] private float patrolSpeed = 2f;

    [Header("Shooting")]
    [SerializeField] private float shootCooldown = 1.5f;

    private Rigidbody2D rb;
    private State currentState;
    private int currentWaypointIndex;

    private Vector2 moveDirection;
    private float currentSpeed;
    private float shootTimer;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.freezeRotation = true;
    }

    private void Start()
    {
        ChangeState(State.Patrol);
    }

    private void Update()
    {
        shootTimer -= Time.deltaTime;

        float distance = Vector2.Distance(transform.position, player.position);

        switch (currentState)
        {
            case State.Patrol:
                Patrol();

                if (distance <= detectionRange)
                    ChangeState(State.Attack);
                break;

            case State.Attack:
                Attack();

                if (distance > detectionRange)
                    ChangeState(State.Patrol);
                break;
        }

        if (txtStateDebug != null)
            txtStateDebug.text = $"Shooter: {currentState}";
    }

    private void FixedUpdate()
    {
        rb.MovePosition(rb.position + moveDirection * currentSpeed * Time.fixedDeltaTime);
    }

    private void ChangeState(State newState)
    {
        currentState = newState;
    }

    private void Patrol()
    {
        MoveTo(waypoints[currentWaypointIndex], patrolSpeed);

        if (Vector2.Distance(transform.position, waypoints[currentWaypointIndex].position) < 0.2f)
        {
            currentWaypointIndex = Random.Range(0, waypoints.Length);
        }
    }

    private void Attack()
    {
        moveDirection = Vector2.zero;
        currentSpeed = 0f;

        if (shootTimer <= 0f)
        {
            Shoot();
            shootTimer = shootCooldown;
        }
    }

    private void Shoot()
    {
        if (projectilePrefab == null || firePoint == null) return;

        GameObject proj = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);

        Vector2 direction = ((Vector2)player.position - (Vector2)firePoint.position).normalized;

        EnemyProjectile p = proj.GetComponent<EnemyProjectile>();
        if (p != null)
            p.SetDirection(direction);
    }

    private void MoveTo(Transform target, float speed)
    {
        moveDirection = (target.position - transform.position).normalized;
        currentSpeed = speed;
    }
}