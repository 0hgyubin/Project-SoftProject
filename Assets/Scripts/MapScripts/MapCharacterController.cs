using UnityEngine;
using UnityEngine.SceneManagement;
using SoftProject.Enums;
using static SoftProject.Enums.MAP_TILE;

public class MapCharacterController : MonoBehaviour
{
    public float moveSpeed = 50f;   
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

    // 현재 밟고 있는 타일을 확인하고 바꾸는 함수
    void ChangeTile()
    {
        MapController myMapController = MapController.Instance;
        float tileSize = myMapController.tileSize;

        // 월드 좌표를 타일 인덱스 {X, Y}(=셀 위치)로 변환
        int cellX = Mathf.FloorToInt(transform.position.x / tileSize) + 1; //FloorToInt 로 좌표는 오직 정수로 반환
        int cellY = Mathf.FloorToInt(transform.position.y / tileSize) + 1;

        // 캐릭터가 맵의 범위를 벗어났을 때 (인덱스 에러 방지용)
        if (cellX < 1 || cellX > myMapController.width ||
            cellY < 1 || cellY > myMapController.height)
            return;

        MAP_TILE tile = myMapController.GetTileType(cellX, cellY);

        // 적 타일 이라면..
        if (tile == MAP_ENEMY)
        {   
            //현재 밟힌 타일
            myMapController.ReplaceTile(cellX, cellY, MAP_FLOOR);
            
            // 전투씬 전환
            SceneManager.LoadScene("CharacterTest");
        }
        //이벤트(4), 상점(5)를 밟았을 때 처리
        else if (tile == MAP_EVENT || tile == MAP_SHOP)
        {
            myMapController.ReplaceTile(cellX, cellY, MAP_FLOOR);
        }
    }
}