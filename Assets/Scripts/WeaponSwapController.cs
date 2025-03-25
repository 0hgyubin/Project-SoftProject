using UnityEngine;
using UnityEngine.UI;

public class WeaponSwapController : MonoBehaviour
{
    public Image currentWeaponImage;
    public Image nextWeaponImage;

    public Sprite[] weaponSprites; 
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
        if(currentWeaponController != null){
            currentWeaponController.weaponID = currentIndex;
            Debug.Log("Weapon swapped to: " + currentWeaponController.weaponID);
        }
    }
}
