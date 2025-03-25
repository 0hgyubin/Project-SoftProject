using UnityEngine;
using UnityEngine.UI;

public class WeaponSwapController : MonoBehaviour
{
    public Image currentWeaponImage;
    public Image nextWeaponImage;

    public Sprite[] weaponSprites; 
    // public GameObject[] weaponPrefab;
    public GameObject swordPrefab;
    public GameObject pistolPrefab;
    private int currentIndex = 0;

    public Vector2 currentSize = new Vector2(100, 100); //size1
    public Vector2 nextSize = new Vector2(60, 60); // size2

    public WeaponController currentWeaponController;

    void Start()
    {
        UpdateWeaponUI();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            currentIndex = (currentIndex - 1 + weaponSprites.Length) % weaponSprites.Length;
            UpdateWeaponUI();
            SwapWeapon();
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            currentIndex = (currentIndex + 1) % weaponSprites.Length;
            UpdateWeaponUI();
            SwapWeapon();
        }
    }

    void UpdateWeaponUI()
    {
        int nextIndex = (currentIndex + 1) % weaponSprites.Length;

        // �̹��� ��ü
        currentWeaponImage.sprite = weaponSprites[currentIndex];
        nextWeaponImage.sprite = weaponSprites[nextIndex];

        // ũ�� ����
        currentWeaponImage.rectTransform.sizeDelta = currentSize;
        nextWeaponImage.rectTransform.sizeDelta = nextSize;
    }

    void SwapWeapon(){
        WeaponController currentWeaponController = FindObjectOfType<WeaponController>();
        
        if(currentWeaponController != null){
            //기존 무기 제거
            Destroy(currentWeaponController.gameObject);

            //무기 ID에 맞는 새로운 WeaponController 생성
            WeaponController newWeaponController = null;

            if(currentIndex == 0){
                newWeaponController = Instantiate(swordPrefab, transform.position, Quaternion.identity).GetComponent<WeaponController>();
            }
            else if(currentIndex == 1){
                newWeaponController = Instantiate(pistolPrefab, transform.position, Quaternion.identity).GetComponent<WeaponController>();
            }

            if(newWeaponController != null){
                currentWeaponController.weaponID = currentIndex;
                Debug.Log("Weapon swapped to: " + currentWeaponController.weaponID);
            }
        }
    }
}
