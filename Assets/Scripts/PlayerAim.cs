using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAim : MonoBehaviour
{
    private Animator animator;
    private Camera mainCamera;

    private void Start()
    {
        animator = GetComponent<Animator>();
        mainCamera = Camera.main;
    }

    private void LateUpdate()
    {
        if (animator == null || mainCamera == null || Mouse.current == null)
            return;

        Vector2 mouseScreenPos = Mouse.current.position.ReadValue();
        Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(
            new Vector3(mouseScreenPos.x, mouseScreenPos.y, -mainCamera.transform.position.z));
        mouseWorldPos.z = transform.position.z;

        Vector2 aimDirection = (mouseWorldPos - transform.position).normalized;

        if (aimDirection == Vector2.zero)
            return;

        animator.SetFloat("InputX", aimDirection.x);
        animator.SetFloat("InputY", aimDirection.y);
        animator.SetFloat("LastInputX", aimDirection.x);
        animator.SetFloat("LastInputY", aimDirection.y);
    }
}