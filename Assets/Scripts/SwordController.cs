using UnityEngine;
using System.Collections;

public class SwordController: WeaponController
{
    // [SerializeField]
    // public Transform player;
    // public GameObject trailPrefab;  //궤적 프리팹
    public GameObject swordProjectilePrefab;    //근접 범위 내에서 생성되는 투사체(궤적)
    private GameObject currentProjectile; // 궤적 오브젝트
    // private GameObject currentTrail; // 궤적 오브젝트
    private Animator animator;

    protected override void Start()
    {
        base.Start();
        weaponID = 0;
        attackPower = 10f;
        animator = GetComponent<Animator>();
    }

    // protected override void Update()
    // {
    //     base.Update();
    //     if(Input.GetMouseButtonDown(0)){
    //         Attack();
    //     }
    // }

    protected override void FireProjectile(){
        // if(currentTrail == null){
        //     currentTrail = Instantiate(trailPrefab, transform.position, transform.rotation);
        //     currentTrail.transform.SetParent(transform);
        // }
        

        if(currentProjectile == null){
            currentProjectile = Instantiate(swordProjectilePrefab, transform.position, transform.rotation);//투사체 생성
            currentProjectile.transform.SetParent(transform);   //투사체의 부모를 검으로 설정
            // StartCoroutine(DestroyProjectileAfterTime(0.5f));
            currentProjectile.GetComponent<ProjectileController>().SetLifetime(0.4f);;
        }

        // animator.SetTrigger("SwingSword");
        // Debug.Log("Sword 공격! Damage:" + attackPower);
    }

    // private IEnumerator DestroyProjectileAfterTime(float delay){
    //     yield return new WaitForSeconds(delay);
    //     if(currentProjectile != null){
    //         Destroy(currentProjectile);
    //     }
    // }
}
