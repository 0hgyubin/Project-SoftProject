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

            // 최초 실행시 HP 초기화
            currentHP = maxHP; // 또는 원하는 초기 HP 값

            var player = FindAnyObjectByType<Player>();
            if (player != null)
            {
                SaveStatsFrom(player);
            }

            //HP를 100으로 초기화하는거 필요함 @@@@

        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// 전투 씬 시작 시 호출: Player 오브젝트에 저장된 스탯을 적용
    public void LoadStatsTo(Player player)
    {
        //잠시, 오류 처리문 없애봄. 오히려 오류 나는게 눈에 더 띄어서 도움될 듯.
        // if (player.hpUI != null)

        // 최대 체력 변화하는거 추가히기 @@@@@
        player.hpUI.SetCurrentHP(currentHP);
        player.strength = strength;
        player.moveSpeed = moveSpeed;
        player.maxJumpCnt = maxJumpCnt;
        player.dashCoolTime = dashCoolTime;
        Debug.Log("OnEnable() 내부에서 load 실행");
        Debug.Log("currentHP:" + currentHP);
    }

    // 전투 종료 시 호출: Player 오브젝트의 현재 스탯을 저장
    public void SaveStatsFrom(Player player)
    {
        //잠시, 오류 처리문 없애봄. 오히려 오류 나는게 눈에 더 띄어서 도움될 듯.
        // if (player.hpUI != null)
        currentHP = player.hpUI.GetCurrentHP();
        strength = player.strength;
        moveSpeed = player.moveSpeed;
        maxJumpCnt = player.maxJumpCnt;
        dashCoolTime = player.dashCoolTime;
        Debug.Log("OnDisable() 내부에서 load 실행");
        Debug.Log("currentHP:" + currentHP);
    }

    // 맵 씬 복귀 직후, 혹은 필요할 때 수동으로 불러오기
    public void ResetPlayerStats(Player player)
    {
        LoadStatsTo(player);
    }
}