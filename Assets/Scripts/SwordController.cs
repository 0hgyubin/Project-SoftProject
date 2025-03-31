using UnityEngine;
using System.Collections;
using UnityEditor.Tilemaps;

public class SwordController: WeaponController
{
    public GameObject swordProjectilePrefab;    //근접 범위 내에서 생성되는 투사체(궤적)
    private GameObject currentProjectile; // 궤적 오브젝트
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    protected override void Start()
    {
        base.Start();
        weaponID = 0;
        attackPower = 10f;
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    protected override void Update(){
        base.Update();
        UpdateProjectileFlip();
    }

    protected override void FireProjectile(){
        if(currentProjectile == null){

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
            // currentProjectile.GetComponent<ProjectileController>().SetLifetime(0.4f);

            // // 현재 생성된 투사체의 SpriteRenderer 가져오기
            // SpriteRenderer projSpriteRenderer = currentProjectile.GetComponent<SpriteRenderer>();
            // if(projSpriteRenderer != null){
            //     // 마우스가 플레이어 왼쪽에 있으면 flipX true, 아니면 false
            //     if(mousePos.x < transform.position.x){
            //         projSpriteRenderer.flipX = true;
            //     } else {
            //         projSpriteRenderer.flipX = false;
            //     }
            // }
        }
    }

    private void UpdateProjectileFlip(){
        if(currentProjectile != null){
            SpriteRenderer projSpriteRenderer = currentProjectile.GetComponent<SpriteRenderer>();
            if(projSpriteRenderer != null){
                Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                mousePos.z = 0;
                if(mousePos.x < transform.position.x){
                    projSpriteRenderer.flipX = true;
                } else {
                    projSpriteRenderer.flipX = false;
                }
            }
        }
    }

    protected override void FollowMouse(){
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
        Debug.Log("!!!");

        transform.rotation = Quaternion.Euler(0, 0, angle);
    }
}
