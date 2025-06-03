using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HPController : MonoBehaviour
{

    public Image HpImage;

    public float maxHP = 100; // ������ ��

    [SerializeField]
    private float currentHP;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (PlayerStatsManager.Instance != null)
        {
            currentHP = PlayerStatsManager.Instance.currentHP;
            Debug.Log($"HPController Start - 불러온 HP: {currentHP}");
        }
        else
        {
            currentHP = maxHP;
            Debug.Log("PlayerStatsManager가 없어서 maxHP로 초기화됨");
        }
        UpdateHPUI();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void TakeDamaged(float damage)
    {
        float newHP = GetCurrentHP() - damage;
        SetCurrentHP(newHP);
    }

    void UpdateHPUI()
    {
        if (HpImage != null)
        {
            float fillRatio = currentHP / maxHP;
            HpImage.fillAmount = fillRatio;
        }
    }

    public bool IsDead()
    {
        if (currentHP <= 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    // 현재 HP를 외부에서 읽어올 수 있도록
    public float GetCurrentHP()
    {
        return currentHP;
    }

    // 외부에서 HP를 설정할 수 있도록 (UI도 함께 업데이트)
    public void SetCurrentHP(float hp)
    {
        currentHP = Mathf.Clamp(hp, 0f, maxHP);
        UpdateHPUI();

        // HP 변경시마다 저장
        if (PlayerStatsManager.Instance != null)
        {
            PlayerStatsManager.Instance.currentHP = currentHP;
            PlayerPrefs.SetFloat("PlayerHP", currentHP);
            PlayerPrefs.Save();
        }
    }
    public void SetMaxHP(float newMaxHP)
    {
        maxHP = newMaxHP;
    }

    public float GetMaxHP()
    {
        return maxHP;
    }
}
