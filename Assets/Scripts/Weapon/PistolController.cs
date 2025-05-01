using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PistolController : WeaponController
{
    [Header("Magazine Settings")]
    public int magazineSize = 12;
    public float reloadTime = 1.5f;

    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI ammoText; // Canvas/AmmoText

    private int currentAmmo;
    private bool isReloading = false;


    public float bulletSpeed = 15f;
    
    protected override void Start()
    {
        weaponID = 1;
        base.Start();
        // if(프리팹 없다면??){}
        if(ammoText == null){
            ammoText = GameObject.Find("Canvas/AmmoText").GetComponent<TextMeshProUGUI>();
        }
        currentAmmo = magazineSize;
        UpdateAmmoUI();
        ammoText.gameObject.SetActive(true);
    }

    protected override void FireProjectile(){
        if(isReloading) return;

        

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

        currentAmmo--;
        UpdateAmmoUI();

        if(currentAmmo <= 0){
            if(!isReloading){
                StartCoroutine(Reload());
            }
            return ;
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
        //Debug.Log("!!!");
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    private void UpdateAmmoUI(){
        ammoText.text = $"{currentAmmo}/{magazineSize}";
    }

    private IEnumerator Reload(){
        isReloading = true;

        //재장전 자막 출력
        // SubtitleManager.Instance.ShowSubtitle("재 장전 중...", reloadTime);
        SubtitleManager.Instance.ShowSubtitle("Ammo Remaining....", reloadTime);

        yield return new WaitForSeconds(reloadTime);

        currentAmmo = magazineSize;
        UpdateAmmoUI();
        isReloading = false;
    }

    void OnDisable(){
        ammoText.gameObject.SetActive(false);
        
    }
}
