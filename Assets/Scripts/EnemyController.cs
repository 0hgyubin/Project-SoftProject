using System.Collections;
using UnityEngine;

public class EnemyController : MonoBehaviour
{


    [SerializeField]
    public float detectionRadius = 5f; // 플레이어를 탐지할 수 있는 반경
    [SerializeField]
    public float attackRadius = 1f; // 플레이어를 공격할 수 있는 반경
    [SerializeField]
    public float moveSpeed = 2f; // 적의 이동 속도
    [SerializeField]
    public int enemyHealth = 100; // 적의 체력
    [SerializeField]
    public int playerDamage = 10; // 플레이어가 적에게 가하는 데미지
    [SerializeField]
    public float attackDelay = 1f; // 공격 준비 시간
    [SerializeField]
    public float attackDuration = 0.5f; // 공격 애니메이션 시간
    [SerializeField]
    public GameObject ProjectilePrefab; // 투사체 프리팹
    [SerializeField]
    public Transform shootTransform; // 투사체 발사 위치
    [SerializeField]
    public float projectileSpeed = 5f; // 투사체 속도
    [SerializeField]
    public bool isMeleeOrIsRange = true; //근거리인지 원거리인지
    [SerializeField]
    public bool isMoved = true;


    private Transform player; // 플레이어의 위치를 저장하는 변수
    private bool isPlayerInRange = false; // 플레이어가 탐지 범위 내에 있는지 여부
    private bool isPlayerInAttackRange = false; // 플레이어가 공격 범위 내에 있는지 여부
    private bool isPreparingAttack = false; // 공격 준비 상태 여부
    private float lastAttackTime; // 마지막 공격 시간
    private Vector2 attackDirection; // 공격 방향을 저장하는 변수
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform; // 태그가 "Player"인 오브젝트의 위치를 가져옴
        lastAttackTime = Time.time; // 현재 시간을 마지막 공격 시간으로 설정

    }

    // Update is called once per frame
    void Update()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position); // 적과 플레이어 사이의 거리 계산

        if (distanceToPlayer <= attackRadius)
        {
            isPlayerInAttackRange = true; // 플레이어가 공격 범위 내에 있음
            if (!isPreparingAttack)
            {
                StartCoroutine(PrepareAttack()); // 공격 준비 코루틴 시작
            }
        }
        else if (distanceToPlayer <= detectionRadius)
        {
            isPlayerInRange = true; // 플레이어가 탐지 범위 내에 있음
            if(isMoved){
                FollowPlayer(); // 플레이어를 따라감
            }
        }
        else
        {
            isPlayerInRange = false; // 플레이어가 탐지 범위 밖에 있음
            isPlayerInAttackRange = false; // 플레이어가 공격 범위 밖에 있음
        }
    }

    void FollowPlayer(){
        if(isPlayerInRange && !isPlayerInAttackRange){
            Vector2 targetPosition = new Vector2(player.position.x, transform.position.y);
            transform.position = Vector2.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
        }
    }

    IEnumerator PrepareAttack(){
        isPreparingAttack = true;
        attackDirection = (player.position - transform.position).normalized;
        yield return new WaitForSeconds(attackDelay);
        AttackPlayer();
        yield return new WaitForSeconds(attackDuration);
        isPreparingAttack = false;

        // 공격 후 플레이어가 탐지 범위 내에 있으면 다시 쫓아감
        if (Vector2.Distance(transform.position, player.position) > attackRadius && Vector2.Distance(transform.position, player.position) <= detectionRadius)
        {
            isPlayerInAttackRange = false;
            if(isMoved){
                FollowPlayer();
            }
        }
    }

    void AttackPlayer(){
        if(isMeleeOrIsRange){
            if(attackDirection.x > 0){
                Shoot(Vector2.right);
            }
            else{
                Shoot(Vector2.left);
            }
        }
        else{
            Shoot(attackDirection);
        }
    }

    void Shoot(Vector2 direction){
        GameObject projectile;
        if(!isMeleeOrIsRange){
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            Quaternion rotation = Quaternion.Euler(new Vector3(0, 0, angle));
            projectile = Instantiate(ProjectilePrefab, shootTransform.position, rotation);
        }
        else{
            projectile = Instantiate(ProjectilePrefab, shootTransform.position, Quaternion.identity);
        }
        Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
        rb.linearVelocity = direction * projectileSpeed;
    }

    public void TakeDamage(int damage){
        enemyHealth -= damage;
        if(enemyHealth <= 0){
            Destroy(gameObject);
        }
    }


}
