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
            
            // 플레이어와 마우스 사이의 방향 계산
            Vector3 direction = (mousePos - transform.position).normalized;
            
            // 원하는 오프셋 거리 (예: 1.0f)
            float offsetDistance = 0.5f;
            
            // 스폰 위치: 플레이어 위치에서 direction 방향으로 offsetDistance만큼 전진
            Vector3 spawnPosition = transform.position + direction * offsetDistance;
            

            Quaternion rotatedRotation = transform.rotation * Quaternion.Euler(0, 0, -90f);
            // currentProjectile = Instantiate(swordProjectilePrefab, transform.position, transform.rotation);//투사체 생성
            currentProjectile = Instantiate(swordProjectilePrefab, spawnPosition, rotatedRotation);//투사체 생성
            currentProjectile.transform.SetParent(transform);   //투사체의 부모를 검으로 설정
            currentProjectile.GetComponent<ProjectileController>().SetLifetime(0.4f);
        }
    }
}
