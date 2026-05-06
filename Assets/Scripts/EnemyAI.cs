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
    [SerializeField] private LayerMask obstacleLayer;

    private Transform playerTransform;
    private Rigidbody2D rb2d;
    private Animator animator;
    private bool isChasing = false;
    private Vector2 moveDirection = Vector2.zero;

    // Angles to check when direct path is blocked (in degrees)
    private readonly float[] steeringAngles = { 45f, -45f, 90f, -90f, 135f, -135f };

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

        // Get the best steering direction (avoids obstacles)
        moveDirection = GetSteeringDirection();

        // Move towards player
        rb2d.linearVelocity = moveDirection * moveSpeed;

        // Update animations based on movement direction
        UpdateAnimation(moveDirection);
    }

    /// <summary>
    /// Casts rays in multiple directions to find a clear path toward the player.
    /// If the direct path is blocked, it tries angled directions until it finds a free one.
    /// </summary>
    private Vector2 GetSteeringDirection()
    {
        Vector2 directionToPlayer = (playerTransform.position - transform.position).normalized;

        // Check if the direct path to the player is clear
        RaycastHit2D directHit = Physics2D.Raycast(
            transform.position,
            directionToPlayer,
            avoidanceRayLength,
            obstacleLayer
        );

        if (directHit.collider == null)
        {
            // No obstacle in the way, go straight to player
            return directionToPlayer;
        }

        // Direct path is blocked — try steering angles
        foreach (float angle in steeringAngles)
        {
            Vector2 testDirection = RotateVector(directionToPlayer, angle);

            RaycastHit2D testHit = Physics2D.Raycast(
                transform.position,
                testDirection,
                avoidanceRayLength,
                obstacleLayer
            );

            if (testHit.collider == null)
            {
                // Found a clear direction, steer that way
                return testDirection;
            }
        }

        // All directions are blocked — fallback to direct direction (edge case)
        return directionToPlayer;
    }

    /// <summary>
    /// Rotates a 2D vector by the given angle in degrees.
    /// </summary>
    private Vector2 RotateVector(Vector2 v, float degrees)
    {
        float radians = degrees * Mathf.Deg2Rad;
        float cos = Mathf.Cos(radians);
        float sin = Mathf.Sin(radians);
        return new Vector2(
            cos * v.x - sin * v.y,
            sin * v.x + cos * v.y
        );
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

        // Avoidance ray (direct path)
        if (playerTransform != null)
        {
            Vector2 directionToPlayer = (playerTransform.position - transform.position).normalized;
            Gizmos.color = Color.red;
            Gizmos.DrawRay(transform.position, directionToPlayer * avoidanceRayLength);
        }
    }
}