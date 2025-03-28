using UnityEngine;

public class ProjectileController : MonoBehaviour
{
    public float speed = 10f;
    public float lifeTime = 0.4f; //투사체가 사라지는 시간
    private float damage;
    public bool isMelee;   //근접 무기의 투사체인지 아닌지 구분.

//
    void Start()
    {
        Player player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        damage = player.attackDamage;
        Destroy(gameObject,lifeTime);
    }

    private void OnTriggerEnter2D(Collider2D collision){
        if(collision.gameObject.CompareTag("Enemy")){
            //박정태 수정
            EnemyController enemyController = collision.GetComponent<EnemyController>(); //충돌한 적 가져옴.
            enemyController.TakeDamage(damage);//EnemyController의 TakeDamage함수 사용해서 적 체력 감소.
            Debug.Log("Enemy 피격 (1)!" + damage); //체력 줄은 거 확인하기 위해 +damage 추가
            //13 데미지 나와야 정상
            if(!isMelee) Destroy(gameObject);
        }
        else if(!isMelee && collision.gameObject.CompareTag("Ground")){
            Destroy(gameObject);
        }
    }
    public void SetLifetime(float newLifetime){
        lifeTime = newLifetime;//
    }
}
