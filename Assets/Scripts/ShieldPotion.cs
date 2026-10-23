using UnityEngine;

public class ShieldPotion : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerInventory inv = collision.GetComponent<PlayerInventory>();
        if (inv != null)
        {
            inv.PickupShield();
            Destroy(gameObject);
        }
    }
}