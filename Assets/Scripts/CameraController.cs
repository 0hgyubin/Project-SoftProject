using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    public Transform player;  // 따라갈 플레이어의 Transform
    public Vector3 offset;    // 플레이어와 카메라 간 거리

    void LateUpdate()
    {
        Vector3 targetPosition = player.position + offset;
        targetPosition.z = -10f; // 2D 카메라는 Z축 고정
        transform.position = Vector3.Lerp(transform.position, targetPosition, 0.1f);
    }
}
