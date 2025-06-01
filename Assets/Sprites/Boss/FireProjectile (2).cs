
using UnityEngine;

public class FireProjectile : MonoBehaviour
{
    public float moveSpeed = 3f;
    private Vector2 moveDirection;

    private void Start()
    {
        // 생성 시 왼쪽 또는 오른쪽 방향으로만 이동
        int dir = Random.Range(0, 2) == 0 ? -1 : 1;
        moveDirection = new Vector2(dir, 0);

        // 2초 뒤 파괴
        Destroy(gameObject, 4f);
    }

    private void Update()
    {
        transform.Translate(moveDirection * moveSpeed * Time.deltaTime);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Wall"))
        {
            // 수평 방향만 반전
            moveDirection = new Vector2(-moveDirection.x, moveDirection.y);
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
    }
}
