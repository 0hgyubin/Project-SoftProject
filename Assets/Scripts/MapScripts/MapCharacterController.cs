using UnityEngine;

public class MapCharacterController : MonoBehaviour
{
    public float moveSpeed = 50f;  // 이동속도
    Vector3 moveDir = Vector3.zero;


    private void Start()
    {
        GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;
    }
    void Update()
    {
        if (Input.GetKey(KeyCode.A))
        {
            moveDir += Vector3.left;
        }
        if (Input.GetKey(KeyCode.D))
        {
            moveDir += Vector3.right;
        }
        if (Input.GetKey(KeyCode.W))
        {
            moveDir += Vector3.up;
        }
        if (Input.GetKey(KeyCode.S))
        {
            moveDir += Vector3.down;
        }
        moveDir = moveDir.normalized; // 대각선 이동 시 속도 보정
    }
    private void FixedUpdate()
    {
        transform.Translate(moveDir * moveSpeed * Time.deltaTime);
        moveDir = Vector3.zero;
    }
}
