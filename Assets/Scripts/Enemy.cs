using UnityEngine;

public class Enemy : MonoBehaviour
{
    public Collider2D enemyCollider;
    public NewMonoBehaviourScript PlayerScript;

    void Start()
    {

        if (PlayerScript == null)
        {
            PlayerScript = FindObjectOfType<NewMonoBehaviourScript>();
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
