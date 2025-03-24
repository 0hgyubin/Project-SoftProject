using UnityEngine;

public class PistolController : WeaponController
{
    public float bulletSpeed = 15f;
    
    protected override void FireProjectile(){
        GameObject bullet = Instantiate(projectilePrefab, transform.position, transform.rotation);

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0f;
        Vector3 fireDirection = (mousePos - transform.position).normalized;

        float angle = Mathf.Atan2(fireDirection.y,fireDirection.x) * Mathf.Rad2Deg - 90f;   //이미지 90도 회전
        bullet.transform.rotation = Quaternion.Euler(new Vector3(0,0,angle));   //Z축 회전.

        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        if(rb != null){
            Debug.Log("Shot!!");
            //  rb.velocity = fireDirection * bulletSpeed;
             rb.linearVelocity = fireDirection * bulletSpeed;
            // rb.linearVelocityY = fireDirection * bulletSpeed;
            // rb.linearVelocityX = fireDirection * bulletSpeed;
            // transform.Translate(fireDirection*bulletSpeed*Time.deltaTime);
        }
        else{
            Debug.Log("rb 가 null임.");
        }

        // Debug.Log("Pistol fired bullet in direction: " + fireDirection);
    }

    
}
