using System.Collections;
using UnityEngine;

public class RangeEnemyController : EnemyController
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Start()
    {
        isMelee = false;  // 원거리 적
        base.Start();
    }

    // Update is called once per frame
    protected override void AttackPlayer()
    {
        // 검 궤적 생성
        Shoot(attackDirection);
        //기본형 몬스터다 보니까 Shoot()만 들어감.
    }
}
