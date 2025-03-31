using UnityEngine;

public class PistolController : WeaponController
{
    public float bulletSpeed = 15f;
    
    protected override void Start()
    {
        base.Start();
        weaponID = 1;
        attackPower = 10f;
    }
    protected override void FireProjectile(){
        GameObject bullet = Instantiate(projectilePrefab, transform.position, transform.rotation);

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0f;
        Vector3 fireDirection = (mousePos - transform.position).normalized;

        float angle = Mathf.Atan2(fireDirection.y,fireDirection.x) * Mathf.Rad2Deg;   //이미지 90도 회전
        bullet.transform.rotation = Quaternion.Euler(new Vector3(0,0,angle));   //Z축 회전.

        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        if(rb != null){
             rb.linearVelocity = fireDirection * bulletSpeed;
        }
        else{
            Debug.Log("rb 가 null임.");
        }
    }

    protected override void FollowMouse(){
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0;

        Vector3 direction = mousePosition - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;// '칼'은 -90, '총' -0도에 맞춰 조정

        //마우스가 총보다 왼쪽에 있으면 180도 돌림. 총이 물구나무 서는 현상 방지.
        if(mousePosition.x < transform.position.x) {
            angle += 180f;
        }
        Debug.Log("!!!");
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }
}
