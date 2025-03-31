using System.Collections;
using UnityEngine;

public class EnemyController : MonoBehaviour
{


    public float detectionRadius = 5f; // 플레이어를 탐지할 수 있는 반경
    public float attackRadius = 1f; // 플레이어를 공격할 수 있는 반경
    public float moveSpeed = 2f; // 적의 이동 속도
    public float enemyHealth = 100f; // 적의 체력

    public int playerDamage; // 플레이어가 적에게 가하는 데미지
    public float attackDelay = 1f; // 공격 준비 시간
    public float attackDuration = 0.5f; // 공격 애니메이션 시간
    public GameObject ProjectilePrefab; // 투사체 프리팹
    public Transform shootTransform; // 투사체 발사 위치
    public float projectileSpeed = 5f; // 투사체 속도
    public int damage = 5; //투사체 맞았을 때 플레이어가 입는 데미지
    public float maxDistance = 5f; //투사체가 최대로 갈 수 있는 거리
    public bool isMelee = true; //근거리인지 원거리인지
    public bool isMoved = true;

    private SpriteRenderer spriteRenderer;
    public Animator weaponAnimator; //원거리 적 무기의 애니메이터(활 당기기 등의 모션 관련)
    private Animator animator; //적 본인의 애니메이터
    private Transform player; // 플레이어의 위치를 저장하는 변수
    private bool isPlayerInRange = false; // 플레이어가 탐지 범위 내에 있는지 여부
    private bool isPlayerInAttackRange = false; // 플레이어가 공격 범위 내에 있는지 여부
    private bool isPreparingAttack = false; // 공격 준비 상태 여부
    private float lastAttackTime; // 마지막 공격 시간
    private Vector2 attackDirection; // 공격 방향을 저장하는 변수
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();
        if (!isMelee)//원거리 공격 적이면 활, 총 등의 애니메이터를 받아옴.
        {
            weaponAnimator = GetComponentInChildren<Animator>();
            if (weaponAnimator != null)
            {
                weaponAnimator.enabled = false;//있다면 해당 활, 총등의 애니메이터를 가져옴
            }
        }
        //만약 활, 총 등에 애니메이션 없는 적인데 원거리 적이라면 말 해주세요 내용 더 추가해야 합니다
        lastAttackTime = Time.time; // 현재 시간을 마지막 공격 시간으로 설정
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;        // 태그가 "Player"인 오브젝트의 위치를 가져옴
        float distanceToPlayer = Vector2.Distance(transform.position, player.position); // 적과 플레이어 사이의 거리 계산
        if (player != null && spriteRenderer != null) //플레이어가 왼쪽에 있냐 오른쪽에 있냐에 따라 왼쪽 오른쪽으로 이동
        {
            spriteRenderer.flipX = player.position.x > transform.position.x;
        }
        // 박정태 수정
        if (!isPreparingAttack) //공격 준비 중이 아닐 때 각도 계산해서 업데이트
        {
            Vector3 direction = player.position - shootTransform.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            shootTransform.rotation = Quaternion.Euler(new Vector3(0, 0, angle - 180));
        }


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
            Vector3 direction = player.position - shootTransform.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            shootTransform.rotation = Quaternion.Euler(new Vector3(0, 0, angle - 180));
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
        if (!isMelee) //원거리 적의 무기 애니메이션 관련 코드
        {
            weaponAnimator.enabled = true;
            weaponAnimator.Play(weaponAnimator.GetCurrentAnimatorStateInfo(0).shortNameHash, -1, 0f);
        }
        else if (isMelee) //근거리 적의 공격 애니메이션 관련 코드
        {
            animator.SetBool("isAttack", true);
            isMoved = false;
        }
        attackDirection = (player.position - transform.position).normalized;
        yield return new WaitForSeconds(attackDelay);
        AttackPlayer();
        yield return new WaitForSeconds(attackDuration);
        if (isMelee) //근거리 적의 공격 애니메이션 관련 코드
        {
            animator.SetBool("isAttack", false);
        }
        isPreparingAttack = false;
        isMoved = true;

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
            Shoot(attackDirection);
    }

    void Shoot(Vector2 direction){
        GameObject projectile;
        if(!isMelee){
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            Quaternion rotation = Quaternion.Euler(new Vector3(0, 0, angle));
            projectile = Instantiate(ProjectilePrefab, shootTransform.position, rotation);
            
        }
        else{
            projectile = Instantiate(ProjectilePrefab, shootTransform.position, Quaternion.identity);
        }
        EnemyProjectileController enemyProjectileController = projectile.GetComponent<EnemyProjectileController>();
        enemyProjectileController.SetDamage(damage);
        enemyProjectileController.SetMaxDistance(maxDistance);
        enemyProjectileController.SetIsMelee(isMelee);
        Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
        rb.linearVelocity = direction * projectileSpeed;
    }

    public void TakeDamage(float damage){
        enemyHealth -= damage;
        if(enemyHealth <= 0){
            Destroy(gameObject);
        }
    }

        private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Player playerComponent = collision.gameObject.GetComponent<Player>();
            if (playerComponent != null)
            {
                playerComponent.TakeDamage(damage); // 플레이어에게 데미지
            }
        }
    }

}
