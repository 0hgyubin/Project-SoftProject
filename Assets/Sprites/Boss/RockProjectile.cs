using UnityEngine;

public class RockProjectile : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Ground") || collision.collider.CompareTag("Player"))
        {
            Destroy(gameObject);
        }
    }
}