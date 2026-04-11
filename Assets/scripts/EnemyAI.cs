using UnityEngine;
using UnityEngine.AI;

// EnemyAI: uses NavMeshAgent for path planning when available but applies movement via
// Rigidbody/Rigidbody2D so prefab animations and movement setup remain unchanged.
/// <summary>
/// EnemyAI controls non-player characters. It uses NavMeshAgent for path planning when available
/// but applies movement through Rigidbody or Rigidbody2D so the prefab's existing animation and
/// movement behavior remain unchanged. Animator parameters are updated to match the movement so
/// animations defined in the prefab continue to work as expected.
/// </summary>
public class EnemyAI : MonoBehaviour
{
    // The transform the enemy will attempt to reach (typically the player)
    public Transform target;
    // Movement speed used when applying velocity directly
    public float speed = 3.5f;
    // Minimum distance to stop moving toward the target
    public float stoppingDistance = 0.2f;

    NavMeshAgent agent;
    Rigidbody rb3d;
    Rigidbody2D rb2d;
    Animator animator;

    Vector2 lastInput = Vector2.zero;

    /// <summary>
    /// Initialize references to NavMeshAgent, Rigidbody/Rigidbody2D and Animator.
    /// Configure NavMeshAgent for path planning only (we will move the character ourselves).
    /// </summary>
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        rb3d = GetComponent<Rigidbody>();
        rb2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        if (agent != null)
        {
            agent.speed = speed;
            agent.angularSpeed = 120f;
            // Use agent only for path planning; disable automatic updates to avoid interfering with prefab movement
            agent.updateRotation = false;
            agent.updatePosition = false;
        }
    }

    /// <summary>
    /// Called every frame. If a target exists, request a path from the NavMeshAgent (if available)
    /// and apply the desired velocity via Rigidbody/Rigidbody2D/transform accordingly.
    /// </summary>
    void Update()
    {
        if (target == null)
            return;

        // If NavMeshAgent is present and on a baked NavMesh, use it for pathfinding
        if (agent != null && agent.isOnNavMesh)
        {
            agent.SetDestination(target.position);
            Vector3 desiredVel = agent.desiredVelocity;

            // Convert desired velocity to 2D or 3D movement depending on available rigidbody
            if (rb2d != null)
            {
                Vector2 move = new Vector2(desiredVel.x, desiredVel.z);
                Apply2DMovement(move);
            }
            else if (rb3d != null)
            {
                Vector3 move = desiredVel;
                Apply3DMovement(move);
            }
            else
            {
                // No rigidbody: move transform directly while preserving animations
                Vector3 move = desiredVel.normalized * speed * Time.deltaTime;
                transform.position += move;
                if (move.sqrMagnitude > 0.001f)
                    transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(desiredVel), Time.deltaTime * 5f);
                UpdateAnimator(desiredVel.x, desiredVel.z);
            }
        }
        else
        {
            // Fallback: simple seek movement (works for 2D and 3D)
            Vector3 dir3 = target.position - transform.position;
            if (rb2d != null)
            {
                Vector2 dir2 = new Vector2(dir3.x, dir3.y);
                if (dir2.sqrMagnitude > 0.01f)
                    Apply2DMovement(dir2.normalized * speed);
                else
                    Apply2DMovement(Vector2.zero);
            }
            else if (rb3d != null)
            {
                dir3.y = 0f;
                if (dir3.sqrMagnitude > 0.01f)
                    Apply3DMovement(dir3.normalized * speed);
                else
                    Apply3DMovement(Vector3.zero);
            }
            else
            {
                // No rigidbody: direct transform move
                dir3.y = 0f;
                if (dir3.sqrMagnitude > 0.01f)
                {
                    Vector3 move = dir3.normalized * speed * Time.deltaTime;
                    transform.position += move;
                    transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir3), Time.deltaTime * 5f);
                    UpdateAnimator(move.x, move.z);
                }
                else
                {
                    UpdateAnimator(0f, 0f);
                }
            }
        }
    }

    /// <summary>
    /// Apply 2D movement to Rigidbody2D by setting its velocity and update animator parameters.
    /// </summary>
    /// <param name="velocity">World-space 2D velocity (x,y)</param>
    void Apply2DMovement(Vector2 velocity)
    {
        if (rb2d != null)
        {
            rb2d.linearVelocity = velocity;
            UpdateAnimator(velocity.x, velocity.y);
        }
    }

    /// <summary>
    /// Apply 3D movement to Rigidbody by updating horizontal velocity components and animator.
    /// </summary>
    /// <param name="velocity">World-space 3D velocity (x,z)</param>
    void Apply3DMovement(Vector3 velocity)
    {
        if (rb3d != null)
        {
            // Keep the y velocity (gravity) and only set horizontal velocity
            Vector3 newVel = new Vector3(velocity.x, rb3d.linearVelocity.y, velocity.z);
            rb3d.linearVelocity = newVel;
            if (velocity.sqrMagnitude > 0.001f)
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(velocity), Time.deltaTime * 5f);
            UpdateAnimator(velocity.x, velocity.z);
        }
    }

    /// <summary>
    /// Updates animator parameters expected by the prefab animator so animations play as designed.
    /// </summary>
    /// <param name="inputX">Horizontal input value</param>
    /// <param name="inputY">Vertical input value</param>
    void UpdateAnimator(float inputX, float inputY)
    {
        if (animator == null)
            return;

        Vector2 input = new Vector2(inputX, inputY);
        bool walking = input.sqrMagnitude > 0.01f;
        animator.SetBool("isWalking", walking);
        animator.SetFloat("InputX", input.x);
        animator.SetFloat("InputY", input.y);

        if (!walking)
        {
            animator.SetFloat("LastInputX", lastInput.x);
            animator.SetFloat("LastInputY", lastInput.y);
        }
        else
        {
            lastInput = input;
        }
    }
}
