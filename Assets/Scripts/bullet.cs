using UnityEngine;

public class Bullet : MonoBehaviour
{
    // How long the bullet lives before auto-destroying (prevents memory leaks)
    [SerializeField] private float lifetime = 5f;

    private void Start()
    {
        Destroy(gameObject, lifetime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Must match the actual enemy script class name: EnemyAI
        if (collision.GetComponent<EnemyAI>() != null)
        {
            Destroy(collision.gameObject); // Kill the enemy
            Destroy(gameObject);           // Destroy the bullet
        }
    }
}
