using UnityEngine;

public class TreasuerChestController : MonoBehaviour
{

    private SpriteRenderer spriteRenderer;

    [SerializeField]
    private GameObject coin;
    [SerializeField]
    private int floor;
    

    [SerializeField] private GameObject[] commonWeapons;
    [SerializeField] private GameObject[] rareWeapons;
    [SerializeField] private GameObject[] epicWeapons;
    [SerializeField] private GameObject[] legendaryWeapons;

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
                    GenerateWeapon();
                    Destroy(gameObject);
                }
            }

        }
    }

    private void GenerateWeapon()
    {
        // 등급 선택
        Debug.Log("GenerateWeapon");
        int grade = GetRandomGrade();

        GameObject weaponPrefab = null;

        // 등급에 따른 무기 프리팹 선택
        switch (grade)
        {
            case 0:
                weaponPrefab = commonWeapons[Random.Range(0, commonWeapons.Length)];
                break;
            case 1:
                weaponPrefab = rareWeapons[Random.Range(0, rareWeapons.Length)];
                break;
            case 2:
                weaponPrefab = epicWeapons[Random.Range(0, epicWeapons.Length)];
                break;
            case 3:
                weaponPrefab = legendaryWeapons[Random.Range(0, legendaryWeapons.Length)];
                break;
        }

        // 선택된 무기 프리팹을 생성
        if (weaponPrefab != null)
        {
            Instantiate(weaponPrefab, transform.position, Quaternion.identity);
        }
    }



    private int GetRandomGrade(){ //랜덤하게 grade 결정하기.
        Debug.Log("GetRandomGrade");
        float total = 0;
        foreach (float weight in gradeSelect)
        {
            total += weight;
        }

        float randomValue = Random.Range(0, total);
        float cumulative = 0;


        Debug.Log("randomValue = " + randomValue);



        for (int i = 0; i < gradeSelect.Length; i++)
        {
            cumulative += gradeSelect[i];

            if (randomValue < cumulative)
            {
                return i; // 선택된 등급 반환
            }
        }
        

        return 0; // 기본값 (Common)
    }
}
