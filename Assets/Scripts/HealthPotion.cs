using UnityEngine;

public class HealthPotion : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerInventory inv = collision.GetComponent<PlayerInventory>();
        if (inv != null)
        {
            inv.PickupHeal();
            Destroy(gameObject);
        }
    }
}