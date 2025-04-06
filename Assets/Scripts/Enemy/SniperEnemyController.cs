using System.Collections;
using UnityEngine;

public class SniperEnemyController : EnemyController
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
        // 총쏘기
        Shoot(attackDirection);
        
    }
}