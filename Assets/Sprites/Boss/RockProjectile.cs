using UnityEngine;

public class RockProjectile : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Ground"))
        {
            Destroy(gameObject);
        }
        if (collision.collider.CompareTag("Player"))
        {
            Player playerComponent = collision.collider.GetComponent<Player>();
            if (playerComponent.isDashed)
            {
                Destroy(gameObject);
            }
            if (playerComponent != null)
            {
                Destroy(gameObject);
                playerComponent.TakeDamage(10); // 'damage'는 이 투사체가 가하는 데미지의 양

            }
        }
        if (collision.collider.CompareTag("Enemy"))
        {
            EnemyController enemy = collision.collider.GetComponent<EnemyController>();
            enemy.TakeDamage(10f);
            Destroy(gameObject);
        }
    }
}