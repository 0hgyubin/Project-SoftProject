using UnityEngine;

public class AutoDestroyAfterAnimation : MonoBehaviour
{
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogWarning("Animator not found on " + gameObject.name);
            Destroy(gameObject, 1f); // ����ó��: Animator ������ 1�� �� ����
        }
    }

    void Update()
    {
        if (animator != null)
        {
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            if (stateInfo.normalizedTime >= 1f && !animator.IsInTransition(0))
            {
                Destroy(gameObject);
            }
        }
    }
}
