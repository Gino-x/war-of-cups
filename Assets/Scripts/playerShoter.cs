using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShoot : MonoBehaviour
{
    [SerializeField] private GameObject _bulletPrefab;
    [SerializeField] private float _bulletSpeed = 10f;
    [SerializeField] private float _bulletZPosition = -1f;

    private Animator _animator;

    private void Start()
    {
        _animator = GetComponent<Animator>();
    }

    private void Update()
    {
        bool firePressed =
            Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame ||
            Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame ||
            Gamepad.current != null && Gamepad.current.rightTrigger.wasPressedThisFrame;

        if (firePressed)
            FireBullet();
    }

    private void FireBullet()
    {
        if (_bulletPrefab == null)
        {
            Debug.LogError("PlayerShoot: Bullet Prefab is not assigned!");
            return;
        }

        Vector2 shootDirection = GetFacingDirection();

        Vector3 bulletSpawnPosition = new Vector3(transform.position.x, transform.position.y, _bulletZPosition);
        GameObject bullet = Instantiate(_bulletPrefab, bulletSpawnPosition, Quaternion.identity);
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();

        if (rb != null)
            rb.linearVelocity = shootDirection * _bulletSpeed;
    }

    private Vector2 GetFacingDirection()
    {
        if (_animator == null)
            return Vector2.up;

        // InputX/Y = direction RIGHT NOW while walking
        float currentX = _animator.GetFloat("InputX");
        float currentY = _animator.GetFloat("InputY");

        // If the player is currently moving, use that live direction
        if (currentX != 0f || currentY != 0f)
            return new Vector2(currentX, currentY).normalized;

        // Player is standing still — use the last direction they faced
        float lastX = _animator.GetFloat("LastInputX");
        float lastY = _animator.GetFloat("LastInputY");

        if (lastX != 0f || lastY != 0f)
            return new Vector2(lastX, lastY).normalized;

        // Absolute fallback
        return Vector2.up;
    }
}
