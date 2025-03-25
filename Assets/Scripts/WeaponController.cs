using UnityEngine;

public class WeaponController : MonoBehaviour
{
    // [SerializeField]
    public int weaponID;
    public float attackPower;
    public Transform player;    //플레이어 위치
    public GameObject projectilePrefab; //투사체 프리팹
    public float rotationSpeed = 5f;
    public float projectileLifetime = 0.4f; //무기마다 lifetime을 다르게 설정하기 위한 변수

    public GameObject swordPrefab;
    public GameObject pistolPrefab;
    private GameObject currentWeapon;

    protected virtual void Start(){
        if(player == null){
            player = GameObject.FindWithTag("Player").transform;
        }
        // SwitchWeapon();
    }

    protected virtual void Update(){
        FollowPlayer();
        FollowMouse();
        if(Input.GetMouseButtonDown(0)){    //마우스 좌클릭시 투사체 발사
            FireProjectile();
        }
    }

    public void SwitchWeapon(){
        if(currentWeapon != null){
            Destroy(currentWeapon);
        }

        if(weaponID == 0){
            currentWeapon = Instantiate(swordPrefab, transform.position, Quaternion.identity);
        }
        else if(weaponID == 1){
            currentWeapon = Instantiate(pistolPrefab, transform.position, Quaternion.identity);
        }
    }

    protected virtual void FireProjectile(){
        // GameObject projectile = Instantiate(projectilePrefab, transform.position, transform.rotation);
        // ProjectileController projectileController = projectile.GetComponent<ProjectileController>();
        // projectileController.SetLifetime(projectileLifetime);
    }

    protected void FollowPlayer(){
        if(player!=null){
            transform.position = player.position;
            // transform.position = Vector3.Lerp(transform.position,player.position, movementSpeed * Time.deltaTime);
            // Debug.Log("Player Position: " + player.position);  // 플레이어 위치 찍기
        }
    }

    protected virtual void FollowMouse(){
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0;

        Vector3 direction = mousePosition - transform.position;
        float angle = Mathf.Atan2(direction.y,direction.x) * Mathf.Rad2Deg-90f;  //degree 90도 빼줘야 '칼'이 정상적인 방향을 가르킴. '총'은 아님.
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0,0,angle), rotationSpeed * Time.deltaTime);
    }
}

