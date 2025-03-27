using UnityEngine;

public class Enemy : MonoBehaviour
{
    public Collider2D enemyCollider;
    public Player PlayerScript;

    void Start()
    {

        if (PlayerScript == null)
        {
            PlayerScript = FindObjectOfType<Player>();
        }
    }

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
