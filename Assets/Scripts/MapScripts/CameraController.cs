using UnityEngine;

public class CameraController : MonoBehaviour
{
    [HideInInspector] public Transform target; // 씬에서 카메라가 따라갈 대상

    public float followSpeed = 5f;              // 카메라가 대상을 부드럽게 따라가는 속도
    public Vector3 offset = new Vector3(0, 0, -10f); 

    void LateUpdate()
    {
        if (target == null)
        {
            Debug.Log("Player not found!");
            return;
        }

        Vector3 targetPos = target.position + offset;
        transform.position = Vector3.Lerp(transform.position, targetPos, followSpeed * Time.deltaTime);
    }
}
