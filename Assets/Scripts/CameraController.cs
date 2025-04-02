using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    public Transform player;  // ���� �÷��̾��� Transform
    public Vector3 offset;    // �÷��̾�� ī�޶� �� �Ÿ�

    void LateUpdate()
    {
        Vector3 targetPosition = player.position + offset;
        targetPosition.z = -10f; // 2D ī�޶�� Z�� ����
        transform.position = Vector3.Lerp(transform.position, targetPosition, 0.1f);
    }
}
