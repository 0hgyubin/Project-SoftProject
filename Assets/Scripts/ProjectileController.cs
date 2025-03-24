using UnityEngine;

public class ProjectileController : MonoBehaviour
{
    public float speed = 10f;
    // public Vector3 direction;
    public float lifeTime = 0.4f; //투사체가 사라지는 시간

    void Start()
    {
        // direction = transform.up;   //무기의 앞 방향
        Destroy(gameObject,lifeTime);
    }

    // Update is called once per frame
    void Update()
    {
        // transform.Translate(direction * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision){
        // Debug.Log("Trigger!");
        if(collision.gameObject.CompareTag("Enemy")){
            Debug.Log("Enemy 피격 (1)!");
            //monster.TakeDamage(공격력);
            Destroy(gameObject);
        }
        else if(collision.gameObject.CompareTag("Ground")){
            // Debug.Log("바닥 충돌!");
            Destroy(gameObject);
        }
    }
    private void OnCollisionEnter2D(Collision2D collision){
        
        if(collision.gameObject.CompareTag("Enemy")){
            Debug.Log("Enemy 피격!");
            //monster.TakeDamage(공격력);
            Destroy(gameObject);
        }
        else if(collision.gameObject.CompareTag("Ground")){
            // Debug.Log("바닥 충돌!");
            Destroy(gameObject);
        }
    }
    public void SetLifetime(float newLifetime){
        lifeTime = newLifetime;
    }
}
