using UnityEngine;

public class EnemyProjectileController : MonoBehaviour
{

    public float damage; // 투사체의 데미지
    private float maxDistance = 100000; // 투사체의 최대 이동 거리
    private bool isMelee;

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
                        Player playerComponent = collision.GetComponent<Player>();
            if (playerComponent.isDashed)
            {
                Invoke("DestroyProjectile", 3f);
                return;
            }
            if (playerComponent != null)
            {
                playerComponent.TakeDamage(damage); // 'damage'는 이 투사체가 가하는 데미지의 양

            }
            if(!isMelee){
                Destroy(gameObject); // 몬스터가 원거리 공격일 때는 투사체 파괴 근거리는 파괴 안 하도록
            }
        }
        else if (collision.CompareTag("Ground"))
        {
            Destroy(gameObject); // 장애물에 부딪히면 투사체 파괴
        }
    }

    public void SetDamage(int damage){
        this.damage = damage;
    }
    public void SetMaxDistance(float maxDistance){
        this.maxDistance = maxDistance;
    }
    public void SetIsMelee(bool isMelee){
        this.isMelee = isMelee;
    }
}
