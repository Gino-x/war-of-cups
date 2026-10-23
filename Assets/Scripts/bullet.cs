using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float lifetime = 5f;

    private void Start()
    {
        Destroy(gameObject, lifetime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        EnemyAI enemy = collision.GetComponent<EnemyAI>();
        if (enemy != null)
        {
            EnemyDrop drop = collision.GetComponent<EnemyDrop>();
            if (drop != null)
                drop.DropLoot();

            if (ScoreManager.Instance != null)
                ScoreManager.Instance.AddScore();

            Destroy(collision.gameObject);
            Destroy(gameObject);
        }
    }
}