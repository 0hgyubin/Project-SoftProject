
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    public Transform player;
    public Vector3 offset;

    void LateUpdate()
    {
        Vector3 targetPosition = player.position + offset;
        targetPosition.z = -10f;

        Vector3 lerpedPosition = Vector3.Lerp(transform.position, targetPosition, 0.1f);

        // CameraShake가 존재하면 기준점 전달
        if (CameraShake.Instance != null)
        {
            CameraShake.Instance.SetBasePosition(lerpedPosition);
        }
        else
        {
            transform.position = lerpedPosition;
        }
    }
}
