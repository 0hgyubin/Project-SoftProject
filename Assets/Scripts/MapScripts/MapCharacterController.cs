using UnityEngine;
using UnityEngine.SceneManagement;

public class MapCharacterController : MonoBehaviour
{
    public float moveSpeed = 50f;   // 이동속도
    Vector3 moveDir = Vector3.zero;

    void Start()
    {
        GetComponent<Rigidbody2D>()
            .constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.A))
            moveDir += Vector3.left;
        else if (Input.GetKey(KeyCode.D))
            moveDir += Vector3.right;
        else if (Input.GetKey(KeyCode.W))
            moveDir += Vector3.up;
        else if (Input.GetKey(KeyCode.S))
            moveDir += Vector3.down;
        
        moveDir = moveDir.normalized;
    }

    void FixedUpdate()
    {
        transform.Translate(moveDir * moveSpeed * Time.deltaTime);
        moveDir = Vector3.zero;
        ChangeTile();
    }

    // 타일 교체 함수
    void ChangeTile()
    {
        MapController myMapController = MapController.Instance;
        float tileSize = myMapController.tileSize;

        // 플레이어가 지금 밟고 있는 타일의 {X,Y} (=위치) 계산하여 반환
        int cellX = Mathf.FloorToInt(transform.position.x / tileSize) + 1; //FloorToInt 소수면 정수로 변환. 정확한 위치 계산용
        int cellY = Mathf.FloorToInt(transform.position.y / tileSize) + 1;

        // 플레이어가 범위를 이탈 했다면 (실제로는 발생 X)
        if (cellX < 1 || cellX > myMapController.width ||
            cellY < 1 || cellY > myMapController.height)
            return;

        int tile = myMapController.GetTileType(cellX, cellY);

        // 밟은게 적 타일이라면..
        if (tile == 1)
        {   
            //일단 타일 교체
            myMapController.ReplaceTile(cellX, cellY, 2);
            
            // 전투씬 전환
            SceneManager.LoadScene("CharacterTest");
        }
        // 이벤트(4), 상점(5)은 그냥 바닥으로 바꾸기만
        else if (tile == 4 || tile == 5)
        {
            myMapController.ReplaceTile(cellX, cellY, 2);
        }
    }
}