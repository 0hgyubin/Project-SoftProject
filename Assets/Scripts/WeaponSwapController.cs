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
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            currentIndex = (currentIndex + 1) % weaponSprites.Length;
            UpdateWeaponUI();
        }
    }

    void UpdateWeaponUI()
    {
        int nextIndex = (currentIndex + 1) % weaponSprites.Length;

        // 이미지 교체
        currentWeaponImage.sprite = weaponSprites[currentIndex];
        nextWeaponImage.sprite = weaponSprites[nextIndex];

        // 크기 조절
        currentWeaponImage.rectTransform.sizeDelta = currentSize;
        nextWeaponImage.rectTransform.sizeDelta = nextSize;
    }
}
