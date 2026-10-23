using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float playerRadius = 0.5f;

    [Tooltip("0 = Player 1 (WASD), 1 = Player 2 (Arrow Keys)")]
    public int playerIndex = 0;

    private Rigidbody2D rb;
    private Vector2 moveInput;
    private Animator animator;
    private Camera mainCamera;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        mainCamera = Camera.main;
    }

    void Update()
    {
        ReadInput();
        rb.linearVelocity = moveInput * moveSpeed;
        ClampPlayerToCamera();
        UpdateAnimator();
    }

    private void ReadInput()
    {
        var kb = UnityEngine.InputSystem.Keyboard.current;
        if (kb == null)
        {
            moveInput = Vector2.zero;
            return;
        }

        float x = 0f;
        float y = 0f;

        if (playerIndex == 0)
        {
            if (kb.aKey.isPressed) x -= 1f;
            if (kb.dKey.isPressed) x += 1f;
            if (kb.sKey.isPressed) y -= 1f;
            if (kb.wKey.isPressed) y += 1f;
        }
        else
        {
            if (kb.fKey.isPressed) x -= 1f;
            if (kb.hKey.isPressed) x += 1f;
            if (kb.gKey.isPressed) y -= 1f;
            if (kb.tKey.isPressed) y += 1f;
        }

        moveInput = new Vector2(x, y).normalized;
    }

    private void UpdateAnimator()
    {
        if (animator == null) return;

        bool isWalking = moveInput != Vector2.zero;
        animator.SetBool("isWalking", isWalking);
        animator.SetFloat("InputX", moveInput.x);
        animator.SetFloat("InputY", moveInput.y);

        if (isWalking)
        {
            animator.SetFloat("LastInputX", moveInput.x);
            animator.SetFloat("LastInputY", moveInput.y);
        }
    }

    private void ClampPlayerToCamera()
    {
        Vector3 playerPos = transform.position;
        Vector3 screenPos = mainCamera.WorldToViewportPoint(playerPos);

        Vector3 radiusOffset = mainCamera.WorldToViewportPoint(playerPos + Vector3.right * playerRadius) - screenPos;
        float radiusX = Mathf.Abs(radiusOffset.x);
        float radiusY = playerRadius / mainCamera.orthographicSize;

        float clampedX = Mathf.Clamp(screenPos.x, radiusX, 1f - radiusX);
        float clampedY = Mathf.Clamp(screenPos.y, radiusY, 1f - radiusY);

        Vector3 clampedWorldPos = mainCamera.ViewportToWorldPoint(new Vector3(clampedX, clampedY, screenPos.z));
        transform.position = clampedWorldPos;
    }
}