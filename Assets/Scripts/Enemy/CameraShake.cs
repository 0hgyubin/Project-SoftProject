using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public static CameraShake Instance;

    public float magnitude = 0.2f;
    private float shakeDuration = 0f;

    private Vector3 basePosition;  // FollowPlayer가 계산한 위치
    private Transform camTransform;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            camTransform = transform;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ShakeCamera(float duration, float intensity = 0.2f)
    {
        shakeDuration = duration;
        magnitude = intensity;
    }

    public void SetBasePosition(Vector3 position)
    {
        basePosition = position;
    }

    private void LateUpdate()
    {
        if (shakeDuration > 0)
        {
            float offsetX = Random.Range(-1f, 1f) * magnitude;
            float offsetY = Random.Range(-1f, 1f) * magnitude;
            camTransform.position = basePosition + new Vector3(offsetX, offsetY, 0f);
            shakeDuration -= Time.deltaTime;
        }
        else
        {
            camTransform.position = basePosition;
        }
    }
}
