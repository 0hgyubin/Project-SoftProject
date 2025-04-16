using UnityEngine;
using UnityEngine.UI;

public class WeaponSwapController : MonoBehaviour
{
    public Image currentWeaponImage;
    public Image nextWeaponImage;

    public WeaponRepository weaponRepository;
    private int currentSlotIndex = 0;   //0또는 1

    public int[] equippedWeaponIDs = new int[2] { 2, 1 };

    Vector2 currentSize = new Vector2(100, 100); //size1
    Vector2 nextSize = new Vector2(60, 60); // size2


    void Start()
    {
        UpdateWeaponUI();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            currentSlotIndex = (currentSlotIndex - 1 + equippedWeaponIDs.Length) % equippedWeaponIDs.Length;
            UpdateWeaponUI();
            SwapWeapon();
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            currentSlotIndex = (currentSlotIndex + 1) % equippedWeaponIDs.Length;
            UpdateWeaponUI();
            SwapWeapon();
        }
    }

    void UpdateWeaponUI()
    {
         int nextSlotIndex = (currentSlotIndex + 1) % equippedWeaponIDs.Length;

        // 현재 무기 데이터와 다음 무기 데이터 가져오기
        WeaponData currentWeaponData = weaponRepository.GetWeaponDataByID(equippedWeaponIDs[currentSlotIndex]);
        WeaponData nextWeaponData = weaponRepository.GetWeaponDataByID(equippedWeaponIDs[nextSlotIndex]);

        // 현재 슬롯의 무기 스프라이트 가져오기
        if (currentWeaponData != null && currentWeaponData.weaponPrefab != null) {
            Sprite currentWeaponSprite = currentWeaponData.weaponPrefab.GetComponent<SpriteRenderer>().sprite;
            currentWeaponImage.sprite = currentWeaponSprite;

        } else {
            currentWeaponImage.sprite = null;  // 빈 슬롯 처리
        }

        // 다음 슬롯의 무기 스프라이트 가져오기
        if (nextWeaponData != null && nextWeaponData.weaponPrefab != null) {
            Sprite nextWeaponSprite = nextWeaponData.weaponPrefab.GetComponent<SpriteRenderer>().sprite;
            nextWeaponImage.sprite = nextWeaponSprite;

        } else {
            nextWeaponImage.sprite = null;  // 빈 슬롯 처리
        }

        // 사이즈 교체
        currentWeaponImage.rectTransform.sizeDelta = currentSize;
        nextWeaponImage.rectTransform.sizeDelta = nextSize;
    }

    void SwapWeapon(){
        WeaponController currentWeaponController = FindAnyObjectByType<WeaponController>();
        
        if (currentWeaponController != null) {
            //기존 무기 제거
            Destroy(currentWeaponController.gameObject);
        }

        // 현재 슬롯에서 무기 ID 가져오기
        int selectedWeaponID = equippedWeaponIDs[currentSlotIndex];
        if (selectedWeaponID == -1) {
            Debug.Log("현재 슬롯(" + currentSlotIndex + ")은 빈 슬롯입니다.");
            return;  // 빈 슬롯이면 더 이상 진행하지 않음.
        }
        // WeaponRepository에서 무기 데이터 획득
        WeaponData newWeaponData = weaponRepository.GetWeaponDataByID(selectedWeaponID);
        if (newWeaponData == null) {
            Debug.LogError("WeaponData not found for ID: " + selectedWeaponID);
            return;
        }

        Instantiate(newWeaponData.weaponPrefab, transform.position, Quaternion.identity);
    }
}
