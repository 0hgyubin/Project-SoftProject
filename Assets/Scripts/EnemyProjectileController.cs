using UnityEngine;

public class EnemyProjectileController : MonoBehaviour
{
    [SerializeField]
    public int damage; // 투사체의 데미지
    [SerializeField]
    public float maxDistance = 1f; // 투사체의 최대 이동 거리

    private Vector2 startPosition; // 투사체의 시작 위치
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        float distanceMoved = Vector2.Distance(startPosition, transform.position);
        if(distanceMoved >= maxDistance){
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("hit!");
            Destroy(gameObject); // 투사체 파괴
        }
        else if (collision.CompareTag("Ground"))
        {
            Destroy(gameObject); // 장애물에 부딪히면 투사체 파괴
        }
    }
}
