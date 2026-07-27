using UnityEngine;

public class EnemyDrop : MonoBehaviour
{
    [SerializeField] private GameObject potionPrefab;
    [Range(0f, 1f)]
    [SerializeField] private float dropChance = 0.3f;

    public void DropLoot()
    {
        if (potionPrefab == null)
            return;

        if (Random.value <= dropChance)
            Instantiate(potionPrefab, transform.position, Quaternion.identity);
    }
}