using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


//1234
public class Player : MonoBehaviour
{
    public HPController hpUI;

    public int maxSpeed;
    public int curJumpCnt = 0;
    public float damageTimer = 2f;
    
    //데미지 연산은 ProjetcileController에서 진행할 예정

    public float damageInterval = 0.1f;
    public bool isDashed = false;
    public bool isTouched = false;
    public bool canDash = true;

    public Rigidbody2D PlayerRigidBody;
    public SpriteRenderer characterSpriteRender;
    public Collider2D CharacterColider;

    [Header("stats")]
    public float moveSpeed = 8f;
    public int maxJumpCnt = 2;
    public float dashCoolTime = 3f;
    public float strength = 3f; //주인공이 가진 힘 스텟. 플레이어 공격력 = 무기 공격력 + 플레이어 힘 스탯
    public float jumpForce = 30f;
    public float dashForce = 10f;
    [Header("Dash Effect")]
    public float blinkDuration = 1.6f;

    public bool isGrounded = true;
    public bool canDamaged = true;

    public AudioClip hitSound;         // 플레이어 피격 시 재생할 사운드 파일
    public AudioClip jumpSound;

    public AudioSource audioSource; // 재생 도구

    public bool isTouchingWall = false;
    private float wallSlideSpeed = -0.8f;
    // 벽 붙기 관련 함수
    public GameObject dashEffectPrefab;

    private Animator animator;

    public bool isDialoging = false; //대화 중인지 확인
    public bool canDialoging = false; //대화 가능한지 확인


    // Inspector에서 할당할 게임 오브젝트 (페이드 아웃용 이미지 오브젝트)
    [SerializeField]
    private GameObject fadePanelObject;

    // 페이드 아웃 이미지의 Image 컴포넌트 (선택적으로 사용)
    private Image canvasImage;

    [SerializeField]
    public float fadingSpeed = 10f;

    void Awake()
    {
        animator = GetComponent<Animator>();
        canvasImage = fadePanelObject.GetComponent<Image>();

        // PlayerStatsManager 확인 및 생성
        if (PlayerStatsManager.Instance == null)
        {
            GameObject go = new GameObject("PlayerStatsManager");
            PlayerStatsManager manager = go.AddComponent<PlayerStatsManager>();
            // 초기 HP 설정
            manager.currentHP = manager.maxHP;
        }
    }

    private void OnEnable()
    {
        // Player 오브젝트가 활성화될 때(씬 전환 후 맵 씬에 돌아올 때 등)
        // 저장된 스탯을 적용
        Debug.Log("OnEnable() 실행");
        if (PlayerStatsManager.Instance != null)
        {
            PlayerStatsManager.Instance.LoadStatsTo(this);
        }
    }

    private void OnDisable()
    {
        // Player 오브젝트가 비활성화될 때(맵 -> 전투 씬 전환 직전)
        // 현재 스탯을 저장
        Debug.Log("OnDisable() 실행");
        if (PlayerStatsManager.Instance != null)
            PlayerStatsManager.Instance.SaveStatsFrom(this);
    }
    void Update()
    {
        // 현재 색상을 가져옵니다
        Color currentColor = canvasImage.color;
        //죽었을 때 페이드아웃되고 씬 로드하도록 변경.
        if (hpUI.IsDead())
        {
            fadePanelObject.SetActive(true);
            Debug.Log("hp가 0 이하임");
            // 알파 값을 점차 감소시킵니다
            currentColor.a += fadingSpeed * Time.deltaTime;
            // 색상을 업데이트합니다
            canvasImage.color = currentColor;
            if (currentColor.a > 0.8)
            {
                SceneManager.LoadScene("ResultScene");
            }

        }
        else
        {
            currentColor.a = 0;
        }



        WallSlide(); //벽 매달리기 기능 추가

        float moveInput = 0f;

        FlipSpriteByMouse();

        // Jump and Down
        Jump();
        if (isGrounded == false) // falling
        {
            PlayerRigidBody.gravityScale = 3f;
        }

        // Move Left and Right
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
        {
            Moving(moveInput);
            animator.SetBool("Move", true);
        }
        else
        {
            animator.SetBool("Move", false);
        }

        if (Input.GetMouseButtonDown(1) && canDash && !isDialoging && !hpUI.IsDead())
        {
            Dash();
        }

        if (isTouched)
        {
            damageTimer += Time.deltaTime;
            if (damageTimer >= damageInterval)
            {
                hpUI.TakeDamaged(10f); // interval마다 데미지
                StartCoroutine(CharacterInvincible());
                damageTimer = 0f;
            }
        }
    }

    private void Moving(float moveInput)
    {
        if (!isDashed && !isDialoging && !hpUI.IsDead())
        {
            if (Input.GetKey(KeyCode.A))
            {
                PlayerRigidBody.linearVelocityX = -moveSpeed;
            }
            else if (Input.GetKey(KeyCode.D))
            {
                PlayerRigidBody.linearVelocityX = moveSpeed;
            }
        }
    }

    private void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !isDialoging && !hpUI.IsDead()) //대화 중 점프 방지용

        {
            if (isTouchingWall && !isGrounded)
            {
                // 기존 속도 초기화
                PlayerRigidBody.linearVelocity = Vector2.zero;

                // 수평+수직 방향으로 힘 가하기
                PlayerRigidBody.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);

                // 점프 후 벽 상태 초기화
                isTouchingWall = false;
                isGrounded = false;
                curJumpCnt++;

                audioSource.PlayOneShot(jumpSound);
            }
            else if (curJumpCnt < maxJumpCnt)
            {
                audioSource.PlayOneShot(jumpSound);
                isGrounded = false;
                PlayerRigidBody.AddForceY(jumpForce, ForceMode2D.Impulse);
                curJumpCnt++;
            }
        }
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("EventNPC"))
        {
            canDialoging = true;
            Debug.Log("EventNPC IN");
        }

    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("EventNPC"))
        {
            canDialoging = false;
            Debug.Log("EventNPC OUT");
        }

    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("OneWayGround"))
        {
            curJumpCnt = 0;
            isGrounded = true;
        }

        if (collision.gameObject.CompareTag("Wall"))
        {
            isTouchingWall = true;
        }
    }

    void OnCollisionExit2D(Collision2D collision) //벽에서 떨어지는 판정
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            isTouchingWall = false;
        }
    }

    void Dash()
    {
        isDashed = true;
        canDash = false;
        canDamaged = false;

        if (dashEffectPrefab != null)
        {
            GameObject effect = Instantiate(dashEffectPrefab, transform.position, Quaternion.identity);

            // flipX 반영
            SpriteRenderer effectSR = effect.GetComponent<SpriteRenderer>();
            if (effectSR != null)
            {
                effectSR.flipX = characterSpriteRender.flipX;
            }
        }

        // 대시 힘 적용
        if (characterSpriteRender.flipX == false)
        {
            PlayerRigidBody.AddForceX(-dashForce, ForceMode2D.Impulse);
        }
        else
        {
            PlayerRigidBody.AddForceX(dashForce, ForceMode2D.Impulse);
        }

        Invoke("EndDash", 0.5f);         // 대시 종료
        Invoke("ResetDash", dashCoolTime); // 대시 쿨타임 초기화
    }



    IEnumerator CharacterInvincible()
    {
        SpriteRenderer SR = characterSpriteRender;

        float duration = blinkDuration; // 임의값 조정
        float curTime = 0f;
        float blinkInterval = 0.2f;

        while (curTime <= duration)
        {
            SR.enabled = false;
            yield return new WaitForSeconds(blinkInterval / 2);
            SR.enabled = true;
            yield return new WaitForSeconds(blinkInterval / 2);
            curTime += blinkInterval;
        }

        isTouched = false;
        damageTimer = 0f;
    }

    //데미지를 입었을때 호출될 함수
    public void TakeDamage(float damage)
    {
        isTouched = true;
        if (!canDamaged) return;
        hpUI.TakeDamaged(damage);

        damageTimer = 0f;

        if (hitSound != null && audioSource != null) // 피격음 재생
        {
            audioSource.PlayOneShot(hitSound);
        }

        StartCoroutine(CharacterInvincible());
    }

    private void FlipSpriteByMouse() //마우스 위치에 따라 캐릭터의 flipX 유무 결정하는 함수
    {
        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        float mouseX = mouseWorldPosition.x;
        float playerX = transform.position.x; // 이 스크립트를 지닌 객체의 X좌표 반환

        if (mouseX > playerX)
        {
            characterSpriteRender.flipX = true;
        }
        else
        {
            characterSpriteRender.flipX = false;
        }
    }


    void EndDash()
    {
        isDashed = false;
        canDamaged = true;
    }

    void ResetDash()
    {
        canDash = true;
    }

    void WallSlide()
    {
        if (isTouchingWall && !isGrounded && PlayerRigidBody.linearVelocity.y < 0)
        {
            PlayerRigidBody.linearVelocity = new Vector2(PlayerRigidBody.linearVelocity.x, Mathf.Max(PlayerRigidBody.linearVelocity.y, wallSlideSpeed));
        }
    }

    public static implicit operator Player(GameObject v)
    {
        throw new NotImplementedException();
    }


    //상점에서 바꿀 수 있는 스탯들 setter함수
    public void SetStrength(float newStrength)
    {
        strength = newStrength;
    }

    public void SetMaxHP(float newMaxHP)
    {
        hpUI.SetMaxHP(newMaxHP);
    }

    public void SetMoveSpeed(float newMoveSpeed)
    {
        moveSpeed = newMoveSpeed;
    }

    public void SetMaxJumpCnt(int newMaxJumpCnt)
    {
        maxJumpCnt = newMaxJumpCnt;
    }

    public void SetDashCoolTime(float newDashCoolTime)
    {
        dashCoolTime = newDashCoolTime;
    }


    //상점에서 바꿀 수 있는 스탯들 adder함수
    public void AddStrength(float addStrength) //dumbbell
    {
        strength += addStrength;
        strength = Mathf.Max(strength, 1f);
    }

    public void AddMaxHP(float addMaxHP) //heart
    {
        hpUI.SetMaxHP(hpUI.maxHP + addMaxHP);
        hpUI.TakeDamaged(-addMaxHP);
    }

    public void AddMoveSpeed(float addMoveSpeed) //feather
    {
        moveSpeed += addMoveSpeed;
        moveSpeed = Mathf.Max(moveSpeed, 1f);
    }

    public void AddMaxJumpCnt(int addMaxJumpCnt) //wings
    {
        maxJumpCnt += addMaxJumpCnt;
        maxJumpCnt = Mathf.Max(maxJumpCnt, 1);
    }

    public void DivideBy2DashCoolTime() //clock
    {
        dashCoolTime /= 2;
        dashCoolTime = Mathf.Clamp(dashCoolTime, 0.00001f, 5f);
    }

}