using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour
{
    public HPController hpUI;

    private int maxSpeed;
    private int CurJumpCnt = 0;
    private float damageTimer = 2f;
    private float DashCoolTime = 1f;

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

    // Start is called once before the first execution of Update after the MonoBehaviour is created

    // Update is called once per frame
    void Update()
    {
        float moveInput = 0f;

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
                characterSpriteRender.flipX = false;
                PlayerRigidBody.linearVelocityX = -MoveSpeed;
            }
            if (Input.GetKey(KeyCode.D))
            {
                moveInput = 0.1f;
                characterSpriteRender.flipX = true;

                PlayerRigidBody.linearVelocityX = MoveSpeed;
            }

        }
    }

    private void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && CurJumpCnt < MaxJumpCnt)
        {
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
        Debug.Log("Dash");
        isDashed = true;
        canDash = false;

        if (characterSpriteRender.flipX == false)
        {
            PlayerRigidBody.AddForceX(-DashForce, ForceMode2D.Impulse);
            Invoke("EndDash", 0.5f);
            Invoke("ResetDash", DashCoolTime);
            //CharacterColider.isTrigger = true;
        }
        if (characterSpriteRender.flipX == true)
        {
            PlayerRigidBody.AddForceX(DashForce, ForceMode2D.Impulse);
            Invoke("EndDash", 0.5f);
            Invoke("ResetDash", DashCoolTime);
           // CharacterColider.isTrigger = true;
        }
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
        Debug.Log("Hit in PlayerController");
        hpUI.TakeDamaged(damage); // **데미지는 항상 올바르게 설정할 것
        damageTimer = 0f;
        StartCoroutine(CharacterInvincible());
    }

    void EndDash()
    {
        isDashed = false;
    }

    void ResetDash()
    {
        canDash = true;
    }
}