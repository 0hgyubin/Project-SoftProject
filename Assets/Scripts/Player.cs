using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
//1234
public class Player : MonoBehaviour
{
    public HPController hpUI;

    [SerializeField]
    private int maxSpeed;
    [SerializeField]
    private int CurJumpCnt = 0;
    [SerializeField]
    private float damageTimer = 2f;
    [SerializeField]
    private float DashCoolTime = 3f;

    //박정태 수정
    [SerializeField]
    public float strength = 3f; //주인공이 가진 힘 스텟. 플레이어 공격력 = 무기 공격력 + 플레이어 힘 스탯
    
    
    public float attackDamage; //플레이어 공격력.
    private float weaponDamage; //무기 공격력

    //데미지 연산은 ProjetcileController에서 진행할 예정

    public float damageInterval = 0.1f;
    public bool isDashed = false;
    public bool isTouched = false;
    public bool canDash = true;

    public Rigidbody2D PlayerRigidBody;
    public SpriteRenderer characterSpriteRender;
    public Collider2D CharacterColider;

    public float MoveSpeed = 8f;
    public int MaxJumpCnt = 2;
    public float JumpForce = 30f;
    public float DashForce = 10f;
    public float BlinkDuration = 1.6f;
    
    public bool isGrounded = true;
    public bool canDamaged = true;

    public AudioClip hitSound;         // 플레이어 피격 시 재생할 사운드 파일
    public AudioClip jumpSound;
    public AudioClip BattleBGM;

    public AudioSource audioSource; // 재생 도구


    void Start()
    {
        audioSource.PlayOneShot(BattleBGM);
    }
    void Update()
    {
        //박정태 수정정
        WeaponController weaponController = GameObject.FindGameObjectWithTag("Weapon").GetComponent<WeaponController>(); //무기 받아오기
        weaponDamage = weaponController.attackPower;//무기에 있는 attackPower라는 값 가져오기.
        attackDamage = strength + weaponDamage; //플레이어 공격력 = 힘 + 무기 공격력
        //여기서 몇 데미지를 줄지 결정함.



        float moveInput = 0f;

        FlipSpriteByMouse();

        // Jump and Down
        Jump();
        if (isGrounded  == false) // falling
        {
            PlayerRigidBody.gravityScale = 3f;
        }

        // Move Left and Right
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
            Moving(moveInput);

        if (Input.GetMouseButtonDown(1) && canDash)
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
        if (!isDashed)
        {
            if (Input.GetKey(KeyCode.A))
            {
                PlayerRigidBody.linearVelocityX = -MoveSpeed;
            }
            if (Input.GetKey(KeyCode.D))
            {
                moveInput = 0.1f;
                PlayerRigidBody.linearVelocityX = MoveSpeed;
            }

        }
    }

    private void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && CurJumpCnt < MaxJumpCnt)
        {
            audioSource.PlayOneShot(jumpSound);
            isGrounded = false;
            PlayerRigidBody.AddForceY(JumpForce, ForceMode2D.Impulse);
            CurJumpCnt++;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            CurJumpCnt = 0;
            isGrounded = true;
        }
        
    }

    void Dash()
    {
        isDashed = true;
        canDash = false;
        canDamaged = false;

        if (characterSpriteRender.flipX == false)
        {
            PlayerRigidBody.AddForceX(-DashForce, ForceMode2D.Impulse);
        }
        else
        {
            PlayerRigidBody.AddForceX(DashForce, ForceMode2D.Impulse);
        }

        Invoke("EndDash", 0.5f);         // 대시 종료
        Invoke("ResetDash", DashCoolTime); // 대시 쿨타임 초기화
    }



    IEnumerator CharacterInvincible()
    {
        SpriteRenderer SR = characterSpriteRender;

        float duration = BlinkDuration; // 임의값 조정
        float curTime = 0f;
        float blinkInterval = 0.2f;

        while (curTime <= duration)
        {
            SR.enabled = false;
            yield return new WaitForSeconds(blinkInterval/2);
            SR.enabled = true;
            yield return new WaitForSeconds(blinkInterval/2);
            curTime +=  blinkInterval;
        }

        isTouched = false;
        damageTimer = 0f;
    }

    public void TakeDamage(float damage) //데미지를 입었을때 호출될 함수
    {
        isTouched = true;
        if (canDamaged == true)
        {
            //Debug.Log("Hit in PlayerController");
            hpUI.TakeDamaged(damage); // **데미지는 항상 올바르게 설정할 것
            damageTimer = 0f;

            if (hitSound != null && audioSource != null) // 피격음 재생
            {
                audioSource.PlayOneShot(hitSound);
            }

            StartCoroutine(CharacterInvincible());
        }
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
}