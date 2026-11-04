using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float lifetime = 5f;

    // Set by PlayerShoot. 0 = Player 1, 1 = Player 2
    [HideInInspector] public int ownerIndex = 0;

    private void Start()
    {
        Destroy(gameObject, lifetime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        EnemyAI enemy = collision.GetComponent<EnemyAI>();
        if (enemy == null) return;

        EnemyHealth health = collision.GetComponent<EnemyHealth>();
        if (health != null)
        {
            health.TakeHit(ownerIndex);
        }
        else
        {
            // Old enemy without EnemyHealth: dies in 1 hit, 10 points
            EnemyDrop drop = collision.GetComponent<EnemyDrop>();
            if (drop != null)
                drop.DropLoot();

            if (ScoreManager.Instance != null)
                ScoreManager.Instance.AddScore(ownerIndex, 10);

            Destroy(collision.gameObject);
        }

        Destroy(gameObject);
    }
}
