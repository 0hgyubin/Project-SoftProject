using UnityEngine;

/// 씬 전환 간 플레이어 스탯을 보존하는 싱글톤 매니저
public class PlayerStatsManager : MonoBehaviour
{
    public static PlayerStatsManager Instance { get; private set; }

    //  보존할 기본 스탯 
    [Header("필수 보존 스탯")]
    public float currentHP;
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
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// 전투 씬 시작 시 호출: Player 오브젝트에 저장된 스탯을 적용
    public void ApplyStatsTo(Player player)
    {
        player.currentHP = currentHP;
        player.strength = strength;
        player.moveSpeed = moveSpeed;
        player.maxJumpCnt = maxJumpCnt;
        player.dashCoolTime = dashCoolTime;

        // UI 업데이트
        if (player.hpUI != null)
            player.hpUI.SetHP(currentHP);
    }

    // 전투 종료 시 호출: Player 오브젝트의 현재 스탯을 저장
    public void SaveStatsFrom(Player player)
    {
        currentHP = player.currentHP;
        strength = player.strength;
        moveSpeed = player.moveSpeed;
        maxJumpCnt = player.maxJumpCnt;
        dashCoolTime = player.dashCoolTime;
    }

    // 맵 씬 복귀 직후, 혹은 필요할 때 수동으로 불러오기
    public void ResetPlayerStats(Player player)
    {
        ApplyStatsTo(player);
    }
}