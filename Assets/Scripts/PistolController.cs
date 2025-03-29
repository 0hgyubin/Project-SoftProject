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

        // Debug.Log("Pistol fired bullet in direction: " + fireDirection);
    }

    protected override void FollowMouse(){
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0;

        Vector3 direction = mousePosition - transform.position;
        float angle = Mathf.Atan2(direction.y,direction.x) * Mathf.Rad2Deg + 75f;  //degree 90도 빼줘야 '칼'이 정상적인 방향을 가르킴. '총'은 아님.
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0,0,angle), rotationSpeed * Time.deltaTime);
    }
    
}
