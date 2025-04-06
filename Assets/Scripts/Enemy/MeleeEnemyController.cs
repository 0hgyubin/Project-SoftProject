using System.Collections;
using UnityEngine;

public class MeleeEnemyController : EnemyController
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Start()
    {
        isMelee = true;  // 근거리 적
        base.Start();
    }

    // Update is called once per frame
    protected override void AttackPlayer()
    {
        // 검 궤적 생성
        Shoot(attackDirection);
        //워낙 단순한 몬스터다 보니까 Shoot()만 들어감.
    }
}
