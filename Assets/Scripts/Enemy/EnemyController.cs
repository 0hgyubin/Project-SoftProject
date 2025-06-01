using System.Collections;
using System.Linq;
using UnityEngine;

public class EnemyController : MonoBehaviour
{

    [Header("Detection & Movement")]
    public float detectionRadius = 5f; // 플레이어를 탐지할 수 있는 반경
    public float attackRadius = 1f; // 플레이어를 공격할 수 있는 반경
    public float moveSpeed = 2f; // 적의 이동 속도

    [Header("Health")]
    public float enemyHealth = 100f; // 적의 체력

    [Header("Attack time")]
    public float attackDelay = 1f; // 공격 준비 시간
    public float attackDuration = 0.5f; // 공격 애니메이션 시간

    [Header("Projectile Settings")]
    public GameObject ProjectilePrefab; // 투사체 프리팹
    public Transform shootTransform; // 투사체 발사 위치
    public float projectileSpeed = 5f; // 투사체 속도
    public int damage = 5; //투사체 맞았을 때 플레이어가 입는 데미지
    public float maxDistance = 5f; //투사체가 최대로 갈 수 있는 거리

    [Header("bool")]
    public bool isMelee; //근거리인지 원거리인지
    public bool bowIsSymmetry = true;
    public bool isFly; //공중의 적인지 확인 (현재 근접 박쥐만 사용)

    protected SpriteRenderer spriteRenderer;
    public SpriteRenderer weaponRenderer; //적 무기의 스프라이트 렌더러(스나이퍼 Flipx를 위해 구현)
    public Animator weaponAnimator; //원거리 적 무기의 애니메이터(활 당기기 등의 모션 관련)
    protected Animator animator; //적 본인의 애니메이터
    protected Transform player; // 플레이어의 위치를 저장하는 변수

    protected Color originalColor; // 원래 색상을 저장하는 변수

    protected bool isPlayerInRange = false; // 플레이어가 탐지 범위 내에 있는지 여부
    protected bool isPlayerInAttackRange = false; // 플레이어가 공격 범위 내에 있는지 여부
    protected bool isPreparingAttack = false; // 공격 준비 상태 여부
    // private float lastAttackTime; // 마지막 공격 시간
    protected Vector2 attackDirection; // 공격 방향을 저장하는 변수

    protected virtual void Start()
    {
        animator = GetComponent<Animator>();
        if (!isMelee)//원거리 공격 적이면 활, 총 등의 애니메이터를 받아옴.
        {
            weaponRenderer = GetComponentsInChildren<SpriteRenderer>()
                    .FirstOrDefault(r => r.gameObject != this.gameObject);
            weaponAnimator = GetComponentInChildren<Animator>();
            if (weaponAnimator != null)
            {
                weaponAnimator.enabled = false;//있다면 해당 활, 총등의 애니메이터를 가져옴
            }

        }
        //만약 활, 총 등에 애니메이션 없는 적인데 원거리 적이라면 말 해주세요 내용 더 추가해야 합니다
        // lastAttackTime = Time.time; // 현재 시간을 마지막 공격 시간으로 설정
        spriteRenderer = GetComponent<SpriteRenderer>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        originalColor = spriteRenderer.color; // 원래 색상
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if (player == null){
            player = GameObject.FindGameObjectWithTag("Player").transform;        // 태그가 "Player"인 오브젝트의 위치를 가져옴
        }
        if (player == null){
            Debug.Log("Error! Player를 찾을 수 없습니다.");
        }

        float distanceToPlayer = Vector2.Distance(transform.position, player.position); // 적과 플레이어 사이의 거리 계산

        //플레이어 위치에 따라 스프라이트 좌우 반전
        FlipSpriteTowardsPlayer();

        //공격 준비 아니면서, 탐지 범위 내라면,  발사 각도 업데이트
        if (!isPreparingAttack && distanceToPlayer <= detectionRadius){
            UpdateShootRotation();
        }

        // 플레이어가 공격 범위 내에 있음
        if (distanceToPlayer <= attackRadius){
            isPlayerInAttackRange = true; 
            if (!isPreparingAttack){
                StartCoroutine(PrepareAttack()); // 공격 준비 코루틴 시작
            }
        }
        // 플레이어가 탐지 범위 내에 있음
        else if (distanceToPlayer <= detectionRadius)
        {
            isPlayerInRange = true;
            if (!isPreparingAttack){
                FollowPlayer(); // 플레이어를 따라감
            }
        }
        else
        {
            isPlayerInRange = false; // 플레이어가 탐지 범위 밖에 있음
            isPlayerInAttackRange = false; // 플레이어가 공격 범위 밖에 있음
        }
    }


    //플레이어 위치에 따라 스프라이트 좌우 반전
    protected void FlipSpriteTowardsPlayer()
    {
        if (spriteRenderer != null) 
        {
            spriteRenderer.flipX = player.position.x > transform.position.x;
        }
    }

    //발사 위치 회전 업데이트    
    protected void UpdateShootRotation()
    {
        Vector3 direction = player.position - shootTransform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        shootTransform.rotation = Quaternion.Euler(new Vector3(0, 0, angle - 180f));

        if (!bowIsSymmetry && weaponRenderer != null) // 비대칭 활이 아니라면 flip 처리
        {
            // 플레이어가 오른쪽에 있을 경우에는 flipY를 true로
            weaponRenderer.flipY = player.position.x > transform.position.x;
        }
    }

    protected void FollowPlayer(){
        if(isPlayerInRange && !isPlayerInAttackRange){
            if (!isFly)
            {
                Vector2 targetPosition = new Vector2(player.position.x, transform.position.y);
                transform.position = Vector2.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
            }
            else
            {
                Vector2 targetPosition = new Vector2(player.position.x, player.position.y);
                transform.position = Vector2.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
            } 
        }
    }

    protected IEnumerator PrepareAttack(){
        isPreparingAttack = true;

        if (!isMelee && weaponAnimator != null) //원거리 적의 무기 애니메이션 관련 코드
        {
            weaponAnimator.enabled = true;
            weaponAnimator.Play(weaponAnimator.GetCurrentAnimatorStateInfo(0).shortNameHash, -1, 0f);
        }
        else if (isMelee) //근거리 적의 공격 애니메이션 관련 코드
        {
            animator.SetBool("isAttack", true);
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
        
        // 공격 후 플레이어가 탐지 범위 내에 있으면 다시 쫓아감
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        if (distanceToPlayer > attackRadius && distanceToPlayer <= detectionRadius){
            isPlayerInAttackRange = false;
            if(!isPreparingAttack){
                FollowPlayer();
            }
        }
    }

    protected virtual void AttackPlayer(){
        Shoot(attackDirection);
    }

    protected void Shoot(Vector2 direction){
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

    public void TakeDamage(float damage)
    {
        enemyHealth -= damage;
        if (enemyHealth <= 0f)
        { // 적이 죽었을 때
            Destroy(gameObject);
        }
        else
        {
            // 게임오브젝트의 색을 빨간 색으로 바꿔주기
            if (spriteRenderer != null)
            {
                StopAllCoroutines(); // 만약 연속으로 피격되어도 코루틴이 겹치지 않도록
                StartCoroutine(ResetColor());
            }
        }
    }

    private IEnumerator ResetColor()
    {
        spriteRenderer.color = Color.red; // 빨간색으로 변경
        yield return new WaitForSeconds(0.3f);
        spriteRenderer.color = originalColor;
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
