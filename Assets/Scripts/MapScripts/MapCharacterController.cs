using UnityEngine;
using UnityEngine.SceneManagement;

public class MapCharacterController : MonoBehaviour
{
    public float moveSpeed = 50f;   // �̵��ӵ�
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

    // Ÿ�� ��ü �Լ�
    void ChangeTile()
    {
        MapController myMapController = MapController.Instance;
        float tileSize = myMapController.tileSize;

        // �÷��̾ ���� ��� �ִ� Ÿ���� {X,Y} (=��ġ) ����Ͽ� ��ȯ
        int cellX = Mathf.FloorToInt(transform.position.x / tileSize) + 1; //FloorToInt �Ҽ��� ������ ��ȯ. ��Ȯ�� ��ġ ����
        int cellY = Mathf.FloorToInt(transform.position.y / tileSize) + 1;

        // �÷��̾ ������ ��Ż �ߴٸ� (�����δ� �߻� X)
        if (cellX < 1 || cellX > myMapController.width ||
            cellY < 1 || cellY > myMapController.height)
            return;

        int tile = myMapController.GetTileType(cellX, cellY);

        // ������ �� Ÿ���̶��..
        if (tile == 1)
        {   
            //�ϴ� Ÿ�� ��ü
            myMapController.ReplaceTile(cellX, cellY, 2);
            
            // ������ ��ȯ
            SceneManager.LoadScene("CharacterTest");
        }
        // �̺�Ʈ(4), ����(5)�� �׳� �ٴ����� �ٲٱ⸸
        else if (tile == 4 || tile == 5)
        {
            myMapController.ReplaceTile(cellX, cellY, 2);
        }
    }
}