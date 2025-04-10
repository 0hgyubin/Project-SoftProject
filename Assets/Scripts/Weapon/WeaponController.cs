using UnityEngine;

public enum WeaponGrade
{
    [InspectorName("일반")]    Common = 0,
    [InspectorName("희귀")]    Rare   = 1,
    [InspectorName("에픽")]    Epic   = 2,
    [InspectorName("전설")]    Legendary = 3
    //전설 = 3 이런 식으로 할당 가능하기도 함. enum 형이라 한글로도 선언 가능.
}

public class WeaponController : MonoBehaviour
{
    
    [HideInInspector] //public 유지해줘야 함
    public int weaponID;    
    
    public float attackPower;   //공격력. (기본 공격력에서 바뀔 수 있는 값)
    public Transform player;    //플레이어 위치
    public GameObject projectilePrefab; //투사체 프리팹
    // public float rotationSpeed = 5f;
    public float projectileLifetime = 0.4f; //무기마다 lifetime을 다르게 설정하기 위한 변수

    // public GameObject swordPrefab;
    // public GameObject pistolPrefab;
    public WeaponRepository weaponRepository;

    protected virtual void Start(){
        WeaponData weaponData = weaponRepository.GetWeaponDataByID(weaponID);
        if(player == null){
            player = GameObject.FindWithTag("Player").transform;
        }

        if (weaponData != null) {
            attackPower = weaponData.attackPower;  // WeaponRepository에서 기본 공격력 할당, 
            //WeaponSwapController.cs에서 최초 생성될때 덮어씌어질 수도 있음.

            projectilePrefab = weaponData.projectilePrefab;
        }
        else {
            Debug.LogError("WeaponData not found for ID: " + weaponID);
        }
    }

    protected virtual void Update(){
        FollowPlayer();
        FollowMouse();
        if(Input.GetMouseButtonDown(0)){    //마우스 좌클릭시 투사체 발사
            FireProjectile();
        }
        FlipWeaponSpriteByMouse();
    }

    // public void SwitchWeapon(){
    //     if(currentWeapon != null){
    //         Destroy(currentWeapon);
    //     }

    //     if(weaponID == 2){
    //         currentWeapon = Instantiate(swordPrefab, transform.position, Quaternion.identity);
    //     }
    //     else if(weaponID == 1){
    //         currentWeapon = Instantiate(pistolPrefab, transform.position, Quaternion.identity);
    //     }
    // }

    protected virtual void FireProjectile(){

    }

    protected void FollowPlayer(){
        if(player!=null){
            transform.position = player.position;
        }
    }

    protected virtual void FollowMouse(){
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0;

        Vector3 direction = mousePosition - transform.position;

        //angle 값 설명
        //degree 90도 빼줘야 '칼'이 정상적인 방향을 가르킴. '총'은 아님.
        //sword처럼 무기의 끝이 12시 방향을 보고 있다면 -90f 해줘야 함.
        //pistol처럼 무기의 끝(총구)가 3시 방향을 보고 있다면 각도 조절 안해도 됨.
        //다만 flipX 바뀌면 rotation+=180도 해주는 코드를 추가해줘야 함.
        // float angle = Mathf.Atan2(direction.y,direction.x) * Mathf.Rad2Deg-90f+30f;  
        float angle = Mathf.Atan2(direction.y,direction.x) * Mathf.Rad2Deg;  
        transform.rotation = Quaternion.Euler(0, 0, angle);
        Debug.Log("???");
    }

    protected void FlipWeaponSpriteByMouse() {
        // 마우스 월드 좌표 구하기
        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        float mouseX = mouseWorldPosition.x;
        float weaponX = transform.position.x; // 무기의 현재 X 좌표

        // 무기의 SpriteRenderer 가져오기
        SpriteRenderer weaponSpriteRenderer = GetComponent<SpriteRenderer>();
        if(weaponSpriteRenderer != null) {
            // 마우스 위치에 따라 flipX 설정
            if(mouseX > weaponX) {
                weaponSpriteRenderer.flipX = false;
            }
            else {
                weaponSpriteRenderer.flipX = true;
            }
        }
    }
}

