using UnityEngine;
//작성자: 이태건
public class Enemy : MonoBehaviour
{
    public Collider2D enemyCollider;
    public Player PlayerScript;

    void Update()
    {
        if (PlayerScript == null) return;

        if (PlayerScript.isDashed)
        {
            enemyCollider.isTrigger = true;
        }
        else
        {
            enemyCollider.isTrigger = false;
        }
    }
}
