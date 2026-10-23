using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [Header("Speed Settings")]
    [SerializeField] private float moveSpeed = 3.5f;
    private float speedMultiplier = 1f;

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

    private readonly Vector2[] steeringDirections = new Vector2[]
    {
        Vector2.up, Vector2.down, Vector2.left, Vector2.right,
        new Vector2(1, 1).normalized, new Vector2(1, -1).normalized,
        new Vector2(-1, 1).normalized, new Vector2(-1, -1).normalized
    };

    private void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        if (rb2d == null) { enabled = false; return; }
    }

    private void Start()
    {
        if (rb2d == null) return;
        FindNearestPlayer();
    }

    private void Update()
    {
        if (rb2d == null) return;

        FindNearestPlayer();

        if (playerTransform == null)
        {
            isChasing = false;
            return;
        }

        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);
        isChasing = distanceToPlayer <= detectionDistance;
        if (!isChasing) moveDirection = Vector2.zero;
    }

    private void FixedUpdate()
    {
        if (rb2d == null) return;

        if (playerTransform == null || !isChasing)
        {
            rb2d.linearVelocity = Vector2.zero;
            moveDirection = Vector2.zero;
            UpdateAnimation(Vector2.zero);
            return;
        }

        moveDirection = GetSmartDirection();
        rb2d.linearVelocity = moveDirection * moveSpeed * speedMultiplier;
        UpdateAnimation(moveDirection);
    }

    public void SetSpeedMultiplier(float multiplier)
    {
        speedMultiplier = multiplier;
    }

    private Vector2 GetSmartDirection()
    {
        Vector2 directionToPlayer = (playerTransform.position - transform.position).normalized;
        if (IsPathClear(directionToPlayer)) return directionToPlayer;

        Vector2 bestDirection = directionToPlayer;
        float bestScore = -999f;

        foreach (Vector2 testDir in steeringDirections)
        {
            if (IsPathClear(testDir))
            {
                float score = Vector2.Dot(testDir, directionToPlayer);
                if (score > bestScore) { bestScore = score; bestDirection = testDir; }
            }
        }
        return bestDirection.normalized;
    }

    private bool IsPathClear(Vector2 direction)
    {
        if (obstacleLayer == 0) return true;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, avoidanceRayLength, obstacleLayer);
        return hit.collider == null;
    }

    private void UpdateAnimation(Vector2 direction)
    {
        if (animator == null) return;
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

    private void FindNearestPlayer()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        if (players.Length == 0)
        {
            playerTransform = null;
            return;
        }

        Transform nearest = players[0].transform;
        float nearestDist = Vector2.Distance(transform.position, nearest.position);

        for (int i = 1; i < players.Length; i++)
        {
            float dist = Vector2.Distance(transform.position, players[i].transform.position);
            if (dist < nearestDist)
            {
                nearestDist = dist;
                nearest = players[i].transform;
            }
        }

        playerTransform = nearest;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionDistance);
    }
}