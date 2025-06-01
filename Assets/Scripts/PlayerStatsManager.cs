using UnityEngine;

/// 씬 전환 간 플레이어 스탯을 보존하는 싱글톤 매니저
public class PlayerStatsManager : MonoBehaviour
{
    public static PlayerStatsManager Instance { get; private set; }

    //  보존할 기본 스탯 
    [Header("필수 보존 스탯")]
    public float currentHP;
    public float maxHP = 100;
    public float strength;
    public float weaponDamage;


    //  옵션 스탯 
    [Header("옵션 보존 스탯")]
    public float moveSpeed;
    public int maxJumpCnt;
    public float dashCoolTime;

    private void Awake()
    {
        // 싱글톤 패턴 구현
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // 최초 실행시 기본값 초기화
            if (!PlayerPrefs.HasKey("PlayerHP"))
            {
                InitializeDefaultStats();
            }
            else
            {
                LoadAllStats();
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeDefaultStats()
    {
        currentHP = maxHP;
        moveSpeed = 8f;  // 기본 이동 속도
        maxJumpCnt = 2;  // 기본 점프 횟수
        dashCoolTime = 3f;  // 기본 대시 쿨타임
        
        // 모든 스탯 저장
        SaveAllStats();
    }

    private void SaveAllStats()
    {
        PlayerPrefs.SetFloat("PlayerHP", currentHP);
        PlayerPrefs.SetFloat("MoveSpeed", moveSpeed);
        PlayerPrefs.SetInt("MaxJumpCnt", maxJumpCnt);
        PlayerPrefs.SetFloat("DashCoolTime", dashCoolTime);
        PlayerPrefs.Save();
    }

    private void LoadAllStats()
    {
        currentHP = PlayerPrefs.GetFloat("PlayerHP", maxHP);
        moveSpeed = PlayerPrefs.GetFloat("MoveSpeed", 8f);
        maxJumpCnt = PlayerPrefs.GetInt("MaxJumpCnt", 2);
        dashCoolTime = PlayerPrefs.GetFloat("DashCoolTime", 3f);
    }

    /// 전투 씬 시작 시 호출: Player 오브젝트에 저장된 스탯을 적용
    public void LoadStatsTo(Player player)
    {
        if (player != null && player.hpUI != null)
        {
            Debug.Log($"LoadStatsTo - currentHP: {currentHP}, moveSpeed: {moveSpeed}, maxJumpCnt: {maxJumpCnt}");
            player.hpUI.SetCurrentHP(currentHP);
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
            moveSpeed = player.moveSpeed;
            maxJumpCnt = player.maxJumpCnt;
            dashCoolTime = player.dashCoolTime;
            
            SaveAllStats();
            Debug.Log($"SaveStatsFrom - HP: {currentHP}, Speed: {moveSpeed}, JumpCnt: {maxJumpCnt}");
        }
    }

    // 맵 씬 복귀 직후, 혹은 필요할 때 수동으로 불러오기
    public void ResetPlayerStats(Player player)
    {
        LoadStatsTo(player);
    }
}