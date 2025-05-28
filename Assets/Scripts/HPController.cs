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
        currentHP = maxHP;
        // 외부에서 어차피 SetCurrentHP() 호출해줌.
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
        //Clamp()함수, min~max 사이 값으로 조정. 0~maxHP 벗어나지 않게, 상한, 하한 정해둠.
        currentHP = Mathf.Clamp(hp, 0f, maxHP);
        UpdateHPUI();
    }
}
