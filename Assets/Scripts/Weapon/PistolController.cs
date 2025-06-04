using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PistolController : WeaponController
{
    [Header("Magazine Settings")]
    public int magazineSize = 12;
    public float reloadTime = 1.0f;

    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI ammoText; // Canvas/AmmoText

    private int currentAmmo;
    private bool isReloading = false;


    public float bulletSpeed = 15f;

    protected override void Start()
    {
        weaponID = 1;
        base.Start();
        currentAmmo = 0;
        var tmp = GameObject.Find("Canvas/AmmoText");
        attackPower = 11f;
        if (tmp != null)
        {
            ammoText = tmp.GetComponent<TextMeshProUGUI>();
        }
        if (ammoText != null)
        {
            ammoText.enabled = true;
            UpdateAmmoUI();
            StartCoroutine(Reload());
        }
    }

    protected override void Update()
    {
        base.Update();
        //R키 누르면 재장전.
        if (ammoText != null && Input.GetKeyDown(KeyCode.R) && !isReloading)
        {
            StartCoroutine(Reload());
        }
    }

    protected override void FireProjectile()
    {
        if (isReloading) return;


        GameObject bullet = Instantiate(projectilePrefab, transform.position, transform.rotation);

        Player player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        bullet.GetComponent<ProjectileController>().SetDamage(attackPower + player.strength);

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0f;
        Vector3 fireDirection = (mousePos - transform.position).normalized;

        float angle = Mathf.Atan2(fireDirection.y, fireDirection.x) * Mathf.Rad2Deg;   //이미지 90도 회전
        bullet.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));   //Z축 회전.

        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = fireDirection * bulletSpeed;
        }
        else
        {
            Debug.Log("rb 가 null임.");
        }

        currentAmmo--;
        UpdateAmmoUI();

        if (ammoText != null && currentAmmo <= 0)
        {
            if (!isReloading)
            {
                StartCoroutine(Reload());
            }
            return;
        }

    }

    protected override void FollowMouse()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0;

        Vector3 direction = mousePosition - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;// '칼'은 -90, '총' -0도에 맞춰 조정

        //마우스가 총보다 왼쪽에 있으면 180도 돌림. 총이 물구나무 서는 현상 방지.
        if (mousePosition.x < transform.position.x)
        {
            angle += 180f;
        }
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    private void UpdateAmmoUI()
    {
        if (ammoText != null)
        {
            ammoText.text = $"{currentAmmo}/{magazineSize}";
        }
    }


    private IEnumerator Reload()
    {
        isReloading = true;
        //재장전 자막 출력
        SubtitleManager.Instance.ShowSubtitle("장전중..", reloadTime);

        yield return new WaitForSeconds(reloadTime);

        currentAmmo = magazineSize;
        UpdateAmmoUI();
        isReloading = false;
    }

    void OnDisable()
    {
        // ammoText.gameObject.SetActive(false);
        if (ammoText != null)
        {
            ammoText.enabled = false;
        }
    }
}
