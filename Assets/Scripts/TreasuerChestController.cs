using UnityEngine;

public class TreasuerChestController : MonoBehaviour
{

    private SpriteRenderer spriteRenderer;

    [SerializeField]
    private GameObject coin;
    [SerializeField]
    private int floor;
    



    [SerializeField] private float[] gradeSelect; // 등급 확률을 위한 변수. 
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.enabled = false; // 처음에 SpriteRenderer를 끕니다.
    }

    // Update is called once per frame
    void Update()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        if (enemies.Length == 0)
        {
            spriteRenderer.enabled = true; // 적이 모두 제거되면 SpriteRenderer를 켭니다.
                        GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                float distance = Vector3.Distance(transform.position, player.transform.position);
                if (distance <= 1.0f && Input.GetKeyDown(KeyCode.W))
                {
                    int randomCoin = floor * Random.Range(7, 10);
                    for(int i = 0 ; i < randomCoin; i++){
                        Instantiate(coin, transform.position, Quaternion.identity);
                    }
                    Destroy(gameObject);
                }
            }

        }
    }
}
