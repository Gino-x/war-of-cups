using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [Header("Speed Settings")]
    [SerializeField] private float moveSpeed = 3.5f;

    [Header("Detection Settings")]
    [SerializeField] private float detectionDistance = 10f;

    [Header("Obstacle Avoidance Settings")]
    [SerializeField] private float avoidanceRayLength = 2f;
    [SerializeField] private LayerMask obstacleLayer;  // Set this in the Inspector to the layer your walls/obstacles are on

    private Transform playerTransform;
    private Rigidbody2D rb2d;
    private Animator animator;
    private bool isChasing = false;
    private Vector2 moveDirection = Vector2.zero;

    // 8 directions to test for clear paths
    private readonly Vector2[] steeringDirections = new Vector2[]
    {
        Vector2.up,
        Vector2.down,
        Vector2.left,
        Vector2.right,
        new Vector2(1, 1).normalized,
        new Vector2(1, -1).normalized,
        new Vector2(-1, 1).normalized,
        new Vector2(-1, -1).normalized
    };

    private void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        if (rb2d == null)
        {
            Debug.LogWarning("EnemyAI: No Rigidbody2D found on this enemy. Movement will not work.");
            enabled = false;
            return;
        }

        if (animator == null)
        {
            Debug.LogWarning("EnemyAI: No Animator found on this enemy. Animations will not work.");
        }
    }

    private void Start()
    {
        if (rb2d == null)
            return;

        FindPlayer();
    }

    private void Update()
    {
        if (rb2d == null)
            return;

        // Try to find player if not found yet
        if (playerTransform == null)
        {
            FindPlayer();
            isChasing = false;
            return;
        }

        // Calculate distance to player
        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);

        // Check if player is within detection distance
        if (distanceToPlayer <= detectionDistance)
        {
            isChasing = true;
        }
        else
        {
            isChasing = false;
            moveDirection = Vector2.zero;
        }
    }

    private void FixedUpdate()
    {
        if (rb2d == null)
            return;

        if (playerTransform == null || !isChasing)
        {
            // Stop moving if not chasing
            rb2d.linearVelocity = Vector2.zero;
            moveDirection = Vector2.zero;
            UpdateAnimation(Vector2.zero);
            return;
        }

        // Get the best direction to move (toward player or around obstacles)
        moveDirection = GetSmartDirection();

        // Move towards player
        rb2d.linearVelocity = moveDirection * moveSpeed;

        // Update animations based on movement direction
        UpdateAnimation(moveDirection);
    }

    /// <summary>
    /// Gets the best direction to move. Tries direct path first, then steers around obstacles.
    /// </summary>
    private Vector2 GetSmartDirection()
    {
        Vector2 directionToPlayer = (playerTransform.position - transform.position).normalized;

        // Try direct path first
        if (IsPathClear(directionToPlayer))
        {
            return directionToPlayer;
        }

        // Direct path blocked — find the best steering direction
        Vector2 bestDirection = directionToPlayer;  // fallback
        float bestScore = -999f;

        // Test each steering direction
        foreach (Vector2 testDir in steeringDirections)
        {
            if (IsPathClear(testDir))
            {
                // This direction is clear. Score it based on how close it is to the player
                float score = Vector2.Dot(testDir, directionToPlayer);

                if (score > bestScore)
                {
                    bestScore = score;
                    bestDirection = testDir;
                }
            }
        }

        return bestDirection.normalized;
    }

    /// <summary>
    /// Checks if a path in the given direction is clear of obstacles.
    /// Returns true if no obstacles are detected.
    /// </summary>
    private bool IsPathClear(Vector2 direction)
    {
        // If no obstacle layer is set, assume all paths are clear
        if (obstacleLayer == 0)
        {
            return true;
        }

        // Raycast in the given direction
        RaycastHit2D hit = Physics2D.Raycast(
            transform.position,
            direction,
            avoidanceRayLength,
            obstacleLayer
        );

        // If we hit something on the obstacle layer, path is blocked
        if (hit.collider != null)
        {
            return false;
        }

        return true;
    }

    private void UpdateAnimation(Vector2 direction)
    {
        if (animator == null)
            return;

        if (direction == Vector2.zero)
        {
            animator.SetBool("isWalking", false);
        }
        else
        {
            animator.SetBool("isWalking", true);
            animator.SetFloat("InputX", direction.x);
            animator.SetFloat("InputY", direction.y);
            animator.SetFloat("LastInputX", direction.x);
            animator.SetFloat("LastInputY", direction.y);
        }
    }

    private void FindPlayer()
    {
        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null)
            playerTransform = playerObj.transform;
    }

    // Visual debug to show detection range and avoidance rays
    private void OnDrawGizmosSelected()
    {
        // Detection range
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionDistance);

        // Avoidance ray (direct path to player)
        if (playerTransform != null)
        {
            Vector2 directionToPlayer = (playerTransform.position - transform.position).normalized;
            Gizmos.color = Color.red;
            Gizmos.DrawRay(transform.position, directionToPlayer * avoidanceRayLength);
        }
    }
}
