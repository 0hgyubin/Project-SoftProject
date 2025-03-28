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
            currentProjectile = Instantiate(swordProjectilePrefab, transform.position, transform.rotation);//투사체 생성
            currentProjectile.transform.SetParent(transform);   //투사체의 부모를 검으로 설정
            currentProjectile.GetComponent<ProjectileController>().SetLifetime(0.4f);
        }
    }
}
