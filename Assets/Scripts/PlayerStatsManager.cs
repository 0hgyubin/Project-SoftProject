using UnityEngine;

/// 씬 전환 간 플레이어 스탯을 보존하는 싱글톤 매니저
public class PlayerStatsManager : MonoBehaviour
{
    public static PlayerStatsManager Instance { get; private set; }

    //  보존할 기본 스탯 
    [Header("필수 보존 스탯")]
    public float currentHP = 100f;  // 기본값 직접 설정
    public float maxHP = 100f;
    public float strength = 3f;
    public float jumpForce = 20f;
    public float dashForce = 10f;
    public float weaponDamage;


    //  옵션 스탯 
    [Header("옵션 보존 스탯")]
    public float moveSpeed = 6f;
    public int maxJumpCnt = 2;
    public float dashCoolTime = 3f;

    private void Awake()
    {
        // 싱글톤 패턴 구현
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeDefaultStats();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void InitializeDefaultStats()
    {
        // 초기값으로 리셋
        maxHP = 100f;
        currentHP = maxHP;
        strength = 3f;
        jumpForce = 20f;
        dashForce = 10f;
        moveSpeed = 6f;
        maxJumpCnt = 2;
        dashCoolTime = 3f;
        
        Debug.Log("플레이어 스탯이 초기화되었습니다.");
    }

    /// 전투 씬 시작 시 호출: Player 오브젝트에 저장된 스탯을 적용
    public void LoadStatsTo(Player player)
    {
        if (player != null && player.hpUI != null)
        {
            Debug.Log($"LoadStatsTo - currentHP: {currentHP}, strength: {strength}, moveSpeed: {moveSpeed}");
            player.hpUI.SetCurrentHP(currentHP);
            player.hpUI.SetMaxHP(maxHP);
            player.strength = strength;
            player.jumpForce = jumpForce;
            player.dashForce = dashForce;
            player.moveSpeed = moveSpeed;
            player.maxJumpCnt = maxJumpCnt;
            player.dashCoolTime = dashCoolTime;
        }
    }

    // 전투 종료 시 호출: Player 오브젝트의 현재 스탯을 저장
    public void SaveStatsFrom(Player player)
    {
        if (player != null && player.hpUI != null)
        {
            currentHP = player.hpUI.GetCurrentHP();
            maxHP = player.hpUI.GetMaxHP();
            strength = player.strength;
            jumpForce = player.jumpForce;
            dashForce = player.dashForce;
            moveSpeed = player.moveSpeed;
            maxJumpCnt = player.maxJumpCnt;
            dashCoolTime = player.dashCoolTime;
            
            Debug.Log($"SaveStatsFrom - HP: {currentHP}, Strength: {strength}");
        }
    }

    // 맵 씬 복귀 직후, 혹은 필요할 때 수동으로 불러오기
    public void ResetPlayerStats(Player player)
    {
        LoadStatsTo(player);
    }
}