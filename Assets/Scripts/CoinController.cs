using System.Data;
using UnityEngine;

public class CoinController : MonoBehaviour
{
    bool isOnGroun = false;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
        Debug.Log("Coin");
        Rigidbody2D rigidbody = GetComponent<Rigidbody2D>();

        float randomJumpForce = Random.Range(3f, 10f);
        Vector2 jumpVelocity = Vector2.up * randomJumpForce;
        jumpVelocity.x = Random.Range(-1f, 1f);

        rigidbody.AddForce(jumpVelocity, ForceMode2D.Impulse); 
        Destroy(gameObject, 60f);
    }

    // Update is called once per frame
    void Update()
    {
        if(isOnGroun){
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                float distance = Vector3.Distance(transform.position, player.transform.position);
                if (distance <= 0.5f)
                {
                    GameManager.insatnce.IncreaseCoin();
                    Destroy(gameObject);
                }
            }
        }
    }
    private void OnCollisionEnter2D(Collision2D other) {
        if(other.gameObject.CompareTag("Ground") || other.gameObject.CompareTag("OneWayGround"))
        {
            Rigidbody2D rb = GetComponent<Rigidbody2D>();
                rb.linearVelocity = Vector2.zero;
                rb.angularVelocity = 0f;
                isOnGroun = true;
        }
    }
}

