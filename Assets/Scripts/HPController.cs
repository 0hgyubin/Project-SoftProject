using UnityEngine;
using UnityEngine.UI;

public class HPController : MonoBehaviour
{

    public Image HpImage;

    public float maxHP = 100f; // ������ ��
    private float currentHP;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentHP = maxHP;
        UpdateHPUI();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TakeDamaged(float damage)
    {
        Debug.Log("Hit");
        currentHP -= damage;
        currentHP = Mathf.Clamp(currentHP, 0, maxHP); // HP���� 0~max ����
        UpdateHPUI();
    }

    void UpdateHPUI()
    {
        Debug.Log("HpChanged");
        if (HpImage != null)
        {
            float fillRatio = currentHP / maxHP;
            HpImage.fillAmount = fillRatio;
        }
    }
}
