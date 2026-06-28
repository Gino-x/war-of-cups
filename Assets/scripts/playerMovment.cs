using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float playerRadius = 0.5f;
    private Rigidbody2D rb;
    private Vector2 moveInput;
    private Animator animator;
    private Camera mainCamera;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        mainCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        rb.linearVelocity = moveInput * moveSpeed;
        ClampPlayerToCamera();
    }

    private void ClampPlayerToCamera()
    {
        Vector3 playerPos = transform.position;
        Vector3 screenPos = mainCamera.WorldToViewportPoint(playerPos);

        // Calculate radius offset in viewport space
        Vector3 radiusOffset = mainCamera.WorldToViewportPoint(playerPos + Vector3.right * playerRadius) - screenPos;
        float radiusX = Mathf.Abs(radiusOffset.x);
        float radiusY = playerRadius / mainCamera.orthographicSize; // Approximate Y radius in viewport space

        // Clamp viewport position with radius offset
        float clampedX = Mathf.Clamp(screenPos.x, radiusX, 1f - radiusX);
        float clampedY = Mathf.Clamp(screenPos.y, radiusY, 1f - radiusY);

        // Convert back to world position
        Vector3 clampedWorldPos = mainCamera.ViewportToWorldPoint(new Vector3(clampedX, clampedY, screenPos.z));
        transform.position = clampedWorldPos;
    }

    public void Move(InputAction.CallbackContext context)
    {
        animator.SetBool("isWalking", true);

        if (context.canceled)
        {
            animator.SetBool("isWalking", false);
            animator.SetFloat("LastInputX", moveInput.x);
            animator.SetFloat("LastInputY", moveInput.y);
        }

        moveInput = context.ReadValue<Vector2>();

        animator.SetFloat("InputX", moveInput.x);
        animator.SetFloat("InputY", moveInput.y);
    }
}
