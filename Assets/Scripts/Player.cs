using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class NewMonoBehaviourScript : MonoBehaviour
{
    public HPController hpUI;

    private int maxSpeed;
    private int CurJumpCnt = 0;
    private float damageTimer = 1f;
    private float DashCoolTime = 5f;

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

        if (collision.gameObject.tag == "Enemy")
        {
            isTouched = true;
            Debug.Log("Hit in PlayerController");
            hpUI.TakeDamaged(10f); // **데미지는 항상 올바르게 설정할 것
            damageTimer = 0f;
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            isTouched = false;
            damageTimer = 0f;
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


    void EndDash()
    {
        isDashed = false;
    }

    void ResetDash()
    {
        canDash = true;
    }
}