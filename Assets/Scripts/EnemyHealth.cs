using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private int hp = 1;          // bullets needed to kill
    [SerializeField] private int scoreValue = 10; // points for the killer

    private bool isDead = false;

    public void TakeHit(int killerIndex)
    {
        if (isDead) return;

        hp--;
        if (hp > 0) return;

        isDead = true;

        EnemyDrop drop = GetComponent<EnemyDrop>();
        if (drop != null)
            drop.DropLoot();

        if (ScoreManager.Instance != null)
            ScoreManager.Instance.AddScore(killerIndex, scoreValue);

        Destroy(gameObject);
    }
}
