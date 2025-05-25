using UnityEngine;
using System.Collections;

public class SwordController : WeaponController
{
    public GameObject swordProjectilePrefab;    //근접 범위 내에서 생성되는 투사체(궤적)
    private GameObject currentProjectile; // 궤적 오브젝트
    private bool isRotating = false;      // 현재 회전 중인지 체크 //오규빈) 검 공격 애니메이션 추가 위해서 넣은 코드
    // private Animator animator;
    // private SpriteRenderer spriteRenderer;

    protected override void Start()
    {
        weaponID = 2;
        base.Start();
        attackPower = 15f;
        // animator = GetComponent<Animator>();
        // spriteRenderer = GetComponent<SpriteRenderer>();
    }

    protected override void Update()
    {
        base.Update();
        UpdateProjectileFlip();
    }

    protected override void FireProjectile()
    {
        if (currentProjectile == null)
        {

            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0;

            //플레이어 -> 마우스 벡터
            Vector3 direction = (mousePos - transform.position).normalized;

            //플레이어로부터 떨어진 거리
            float offsetDistance = 0.5f;

            // 스폰 위치: 플레이어 위치에서 direction 방향으로 offsetDistance만큼 전진
            Vector3 spawnPosition = transform.position + direction * offsetDistance;


            Quaternion rotatedRotation = transform.rotation * Quaternion.Euler(0, 0, 0);
            currentProjectile = Instantiate(swordProjectilePrefab, spawnPosition, rotatedRotation);//투사체 생성
            currentProjectile.transform.SetParent(transform);   //투사체의 부모를 검으로 설정
            ProjectileController curProjectile = currentProjectile.GetComponent<ProjectileController>();

            Player player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();

            if (curProjectile != null)
            {
                curProjectile.SetDamage(attackPower + player.attackDamage);
            }

            //오규빈) 검 공격 애니메이션 추가 위해서 넣은 코드
            float rotationAngle = 180f;
            StartCoroutine(RotateSword(rotationAngle, 0.1f)); // 0.3초 동안 회전
        }
    }

    private void UpdateProjectileFlip()
    {
        if (currentProjectile != null)
        {
            SpriteRenderer projSpriteRenderer = currentProjectile.GetComponent<SpriteRenderer>();
            if (projSpriteRenderer != null)
            {
                Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                mousePos.z = 0;
                if (mousePos.x < transform.position.x)
                {
                    projSpriteRenderer.flipX = true;
                }
                else
                {
                    projSpriteRenderer.flipX = false;
                }
            }
        }
    }

    protected override void FollowMouse()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0;

        Vector3 direction = mousePosition - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        //마우스가 총보다 왼쪽에 있으면 180도 돌림. 총이 물구나무 서는 현상 방지.
        // if (mousePosition.x < transform.position.x - 90)
        // {
        //     spriteRenderer.flipX = true;           
        // }

        // else
        // {
        //     spriteRenderer.flipX = false;
        // }
        if (mousePosition.x < transform.position.x)
        {
            angle += 180f;
        }
        //Debug.Log("!!!");

        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    private IEnumerator RotateSword(float angle, float duration)//오규빈) 검 공격 애니메이션 추가 위해서 넣은 코드
    {
        if (isRotating) yield break;   // 이미 회전 중이면 중복 실행 금지
        isRotating = true;

        Quaternion startRotation = transform.rotation;
        Quaternion endRotation = startRotation * Quaternion.Euler(0, 0, angle);

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            transform.rotation = Quaternion.Slerp(startRotation, endRotation, elapsed / duration);
            yield return null;
        }

        transform.rotation = endRotation; // 마지막에 정확히 고정
        isRotating = false;
    }
}
