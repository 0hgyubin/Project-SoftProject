using UnityEngine;
using System.Collections;

public class SwordController: WeaponController
{
    public GameObject swordProjectilePrefab;    //근접 범위 내에서 생성되는 투사체(궤적)
    private GameObject currentProjectile; // 궤적 오브젝트
    private Animator animator;

    protected override void Start()
    {
        base.Start();
        weaponID = 0;
        attackPower = 10f;
        animator = GetComponent<Animator>();
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
            

            Quaternion rotatedRotation = transform.rotation * Quaternion.Euler(0, 0, -90f-30f);
            // currentProjectile = Instantiate(swordProjectilePrefab, transform.position, transform.rotation);//투사체 생성
            currentProjectile = Instantiate(swordProjectilePrefab, spawnPosition, rotatedRotation);//투사체 생성
            currentProjectile.transform.SetParent(transform);   //투사체의 부모를 검으로 설정
            currentProjectile.GetComponent<ProjectileController>().SetLifetime(0.4f);
        }
    }
}
