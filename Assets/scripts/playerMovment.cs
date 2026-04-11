using UnityEngine;
using UnityEngine.InputSystem;


/// <summary>
/// Controls player movement and animator parameters driven by the Input System.
/// Expects a Rigidbody2D and Animator on the same GameObject.
/// </summary>
public class playerMovment : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    private Rigidbody2D rb;
    private Vector2 moveInput;
    private Animator animator;

    /// <summary>
    /// Cache Rigidbody2D and Animator references.
    /// </summary>
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    /// <summary>
    /// Apply movement each frame by setting the Rigidbody2D velocity.
    /// </summary>
    void Update()
    {
        if (rb != null)
            rb.linearVelocity = moveInput * moveSpeed;
    }

    /// <summary>
    /// Input callback used by the Input System. Updates movement vector and animator parameters.
    /// </summary>
    /// <param name="context">Input callback context provided by the Input System.</param>
    public void Move(InputAction.CallbackContext context)
    {
        // When any Move input is received consider the player walking
        animator.SetBool("isWalking", true);

        if (context.canceled)
        {
            // On cancel, set last input values (used by animator to hold facing direction)
            animator.SetBool("isWalking", false);
            animator.SetFloat("LastInputX", moveInput.x);
            animator.SetFloat("LastInputY", moveInput.y);
        }

        moveInput = context.ReadValue<Vector2>();
        animator.SetFloat("InputX", moveInput.x);
        animator.SetFloat("InputY", moveInput.y);
    }

}
