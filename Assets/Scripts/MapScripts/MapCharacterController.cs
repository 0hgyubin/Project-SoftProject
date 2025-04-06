using UnityEngine;

public class MapCharacterController : MonoBehaviour
{
    public float moveSpeed = 50f;  // �̵��ӵ�

    void Update()
    {
        Vector3 moveDir = Vector3.zero;
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
        moveDir = moveDir.normalized; // �밢�� �̵� �� �ӵ� ����

        transform.Translate(moveDir * moveSpeed * Time.deltaTime);
    }
}
