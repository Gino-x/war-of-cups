using UnityEngine;

public class EnemyDrop : MonoBehaviour
{
    [SerializeField] private GameObject potionPrefab;
    [Range(0f, 1f)]
    [SerializeField] private float dropChance = 0.3f;

    [SerializeField] private GameObject shieldPrefab;
    [Range(0f, 1f)]
    [SerializeField] private float shieldDropChance = 0.3f;

    public void DropLoot()
    {
        if (potionPrefab != null && Random.value <= dropChance)
            Instantiate(potionPrefab, transform.position, Quaternion.identity);

        if (shieldPrefab != null && Random.value <= shieldDropChance)
            Instantiate(shieldPrefab, transform.position, Quaternion.identity);
    }
}