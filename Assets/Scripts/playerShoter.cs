using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShoot : MonoBehaviour
{
    public int playerIndex;
    [SerializeField] private GameObject _bulletPrefab;
    [SerializeField] private float _bulletSpeed = 10f;
    [SerializeField] private float _bulletZPosition = -1f;
    [SerializeField] private AudioClip shootSound;

    private Animator _animator;
    private AudioSource _audioSource;

    private void Start()
    {
        _animator = GetComponent<Animator>();
        _audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        bool firePressed = false;

        if (playerIndex == 0)
        {
            firePressed = Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame;
        }
        else if (playerIndex == 1)
        {
            firePressed = Keyboard.current != null && Keyboard.current.jKey.wasPressedThisFrame;
        }

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

        if (_audioSource != null && shootSound != null)
            _audioSource.PlayOneShot(shootSound);
    }

    private Vector2 GetFacingDirection()
    {
        if (_animator == null)
            return Vector2.up;

        float currentX = _animator.GetFloat("InputX");
        float currentY = _animator.GetFloat("InputY");

        if (currentX != 0f || currentY != 0f)
            return new Vector2(currentX, currentY).normalized;

        float lastX = _animator.GetFloat("LastInputX");
        float lastY = _animator.GetFloat("LastInputY");

        if (lastX != 0f || lastY != 0f)
            return new Vector2(lastX, lastY).normalized;

        return Vector2.up;
    }
}