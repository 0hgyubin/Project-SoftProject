using UnityEngine;
using System.Collections.Generic;

public class MapController : MonoBehaviour
{
    public GameObject EnemyTilePrefab; // 적 타일(빨간 색)
    public GameObject FloorTilePrefab; // 바닥 타일(흰 색)
    public GameObject NextTilePrefab; // 다음 층으로 올라가는 타일(노란 색)
    public GameObject WallTilePrefab; // 벽 타일(곤색)
    public GameObject PlayerPrefab; //플레이어

    public int width = 10;
    public int height = 10;
    public float tileSize = 1f;

    private int[,] Map;
    private Vector2Int startPos; // 시작 지점
    private Vector2Int desPos;   // 도착 지점

    void Start()
    {
        GenerateEmptyMap();      // 1. 모든 맵을 벽으로 초기화
        SetStartAndGoal();       // 2. 시작점과 목적지 정하기
        MakePathToDes();  // 3. 경로 강제 연결 + 주변 랜덤 확장
        RenderMap();             // 4. 전체 타일 렌더링
        SpawnPlayer();           // 5. 플레이어 생성 및 카메라 연결
    }
    // 0 : Wall, 1 : Enemy, 2 : Floor, 3 : des
    void GenerateEmptyMap()
    {
        Map = new int[width, height];
        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
                Map[x, y] = 0; // 0 = wall
    }

    void SetStartAndGoal()
    {
        startPos = new Vector2Int(Random.Range(0, width), Random.Range(0, height));
        Map[startPos.x, startPos.y] = 2; // 바닥

        desPos = GetFarthestFrom(startPos);
        Map[desPos.x, desPos.y] = 3; // 목적지
    }

    Vector2Int GetFarthestFrom(Vector2Int startPos)
    {
        Vector2Int farthest = startPos;
        float maxDist = 0f;
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                float dist = Vector2Int.Distance(startPos, new Vector2Int(x, y));
                if (dist > maxDist)
                {
                    maxDist = dist;
                    farthest = new Vector2Int(x, y);
                }
            }
        }
        return farthest; // 시작지점으로부터 가장 먼 위치 반환
    }

    // 목적지까지 도달하는 하나의 경로는 무조건 보장
    void MakePathToDes()
    {
        List<Vector2Int> path = BFS(startPos, desPos);
        if (path == null || path.Count == 0)
        {
            GenerateEmptyMap();
            BFS(startPos, desPos);
            return;
        }

        // 경로를 따라 floor/enemy 타일 배치 (시작점과 목적지는 제외)
        for (int i = 1; i < path.Count - 1; i++)
        {
            Vector2Int pos = path[i];
            if (Random.value < 0.1f)
            {
                Map[pos.x, pos.y] = 1;
            }
            else
            {
                Map[pos.x, pos.y] = 2;
            }
        }

        // 이 코드는 진짜 랜덤하게 길 만드는 로직 (하나의 확실한 루트가 확보 되었을 때만)
        Queue<Vector2Int> Q = new Queue<Vector2Int>();
        bool[,] visited = new bool[width, height];
        Vector2Int[] dirs = new Vector2Int[] {
            //(0,1), (0,-1), (-1,0), (0,1)에 대응
            Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right 
        };

        Q.Enqueue(startPos);
        visited[startPos.x, startPos.y] = true;

        while (Q.Count > 0)
        {
            Vector2Int current = Q.Dequeue();
            foreach (Vector2Int dir in dirs)
            {
                Vector2Int next = current + dir;
                if (IsInBounds(next) && !visited[next.x, next.y] && Map[next.x, next.y] == 0)
                {
                    visited[next.x, next.y] = true;

                    if (next != desPos)
                    {
                        if(Random.value < 0.1f)
                        {
                            Map[next.x, next.y] = 1;
                        }
                        else
                        {
                            Map[next.x, next.y] = 0;
                        }
                        Q.Enqueue(next);
                    }
                }
            }
        }
    }
    
    //BFS
    List<Vector2Int> BFS(Vector2Int start, Vector2Int des)
    {
        Queue<Vector2Int> queue = new Queue<Vector2Int>();
        Dictionary<Vector2Int, Vector2Int> parent = new Dictionary<Vector2Int, Vector2Int>(); //map에 대응
        bool[,] visited = new bool[width, height];

        queue.Enqueue(start);
        visited[start.x, start.y] = true;

        Vector2Int[] dirs = new Vector2Int[] {
            Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right
        };

        while (queue.Count > 0)
        {
            Vector2Int current = queue.Dequeue();
            if (current == des) break;

            foreach (Vector2Int dir in dirs)
            {
                Vector2Int next = current + dir;
                if (IsInBounds(next) && !visited[next.x, next.y])
                {
                    visited[next.x, next.y] = true;
                    parent[next] = current;
                    queue.Enqueue(next);
                }
            }
        }

        if (!parent.ContainsKey(des)) return null; // 시작지 - 목적지까지의 루트 생성 실패시 null 값 반환

        // 경로 재구성
        List<Vector2Int> path = new List<Vector2Int>();
        Vector2Int step = des;
        while (step != start)
        {
            path.Add(step);
            step = parent[step];
        }
        path.Add(start);
        path.Reverse();
        return path;
    }

    void RenderMap() // Help
    {
        GameObject mapParent = new GameObject("Map"); // 게임 실행 시 Map에 다 쑤셔박아서 관리 가능

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                GameObject prefab = null;
                switch (Map[x, y])
                {
                    case 0: prefab = WallTilePrefab; break;
                    case 1: prefab = EnemyTilePrefab; break;
                    case 2: prefab = FloorTilePrefab; break;
                    case 3: prefab = NextTilePrefab; break;
                }

                if (prefab != null)
                {
                    float zPos = manageZ(Map[x, y]);
                    Vector3 pos = new Vector3(x * tileSize + tileSize * 0.5f, y * tileSize + tileSize * 0.5f, zPos);
                    Instantiate(prefab, pos, Quaternion.identity, mapParent.transform);
                }
            }
        }
    }

    float manageZ(int type)
    {
        switch (type)
        {
            case 0: return -0.5f; // Wall 제일 위로
            case 1: return -0.6f; // Enemy
            case 2: return -1.0f; // Floor 가장 아래
            case 3: return -0.8f; // NextTile
            default: return 0f;
        }
    }

    void SpawnPlayer()
    {
        Vector3 pos = new Vector3(startPos.x * tileSize + tileSize * 0.5f, startPos.y * tileSize + tileSize * 0.5f, -0.2f);
        GameObject player = Instantiate(PlayerPrefab, pos, Quaternion.identity); //Quaternion.identity 회전없이 정방향

        CameraController camera = Camera.main.GetComponent<CameraController>(); // 생성된 플레이어와 카메라 연결
        if (camera != null)
        {
            camera.target = player.transform;
        }
    }

    bool IsInBounds(Vector2Int pos)
    {
        return pos.x >= 0 && pos.x < width && pos.y >= 0 && pos.y < height;
    }
}
