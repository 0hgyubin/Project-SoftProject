using UnityEngine;
using System.Collections.Generic;

public class MapController : MonoBehaviour
{
    public GameObject EnemyTilePrefab; // 적 타일 (빨간 색)
    public GameObject FloorTilePrefab; // 바닥 타일 (흰 색)
    public GameObject NextTilePrefab;  // 다음 층으로 올라가는 타일 (노란 색)
    public GameObject WallTilePrefab;  // 벽 타일 (곤색)
    public GameObject PlayerPrefab;    // 플레이어

    public int width = 25;
    public int height = 25;
    public int extraRoadCnt = 0;
    public int extraMaxCnt = 20;
    public float tileSize = 1f;


    private int[,] Map;
    private Vector2Int startPos;      // 시작 지점 (1~width, 1~height)
    private Vector2Int desPos;        // 도착 지점

    // 생성된 타일 오브젝트를 관리용 dic
    private Dictionary<Vector2Int, GameObject> tileObjects = new Dictionary<Vector2Int, GameObject>();

    void Start()
    {
        GenerateEmptyMap();      // 1. 모든 맵을 벽으로 초기화
        SetStartAndGoal();       // 2. 시작점과 목적지 정하기
        MakePathToDes();         // 3. 경로 강제 연결 + 주변 랜덤 확장
        RenderMap();             // 4. 전체 타일 렌더링
        SpawnPlayer();           // 5. 플레이어 생성 및 카메라 연결
        MakeExtraPath();         // 6. 추가 경로 생성
        LastCheck();             // 7. 의미없는 타일 모두 벽 타일로 교체하는 함수
        MakeBoundWall();         // 8. 마지막으로 플레이어 밖으로 못나가게 벽 타일로 두루는 함수
    }

    // 0 : Wall, 1 : Enemy, 2 : Floor, 3 : des
    void GenerateEmptyMap()
    {
        Map = new int[width + 2, height + 2];
        for (int x = 0; x < width + 2; x++)
        {
            for (int y = 0; y < height + 2; y++)
            {
                Map[x, y] = 0;
            }
        }
    }

    void SetStartAndGoal()
    {
        // 내부 영역 (플레이어 이동 가능 영역): 인덱스 1~width, 1~height
        startPos = new Vector2Int(Random.Range(1, width + 1), Random.Range(1, height + 1));
        Map[startPos.x, startPos.y] = 2; // 시작은 Floor(2)

        desPos = GetFarthestFrom(startPos);
        Map[desPos.x, desPos.y] = 3; // 목적지는 3
    }

    Vector2Int GetFarthestFrom(Vector2Int sPos)
    {
        Vector2Int farthest = sPos;
        float maxDist = 0f;
        for (int x = 1; x <= width; x++)
        {
            for (int y = 1; y <= height; y++)
            {
                float dist = Vector2Int.Distance(sPos, new Vector2Int(x, y));
                if (dist > maxDist)
                {
                    maxDist = dist;
                    farthest = new Vector2Int(x, y);
                }
            }
        }
        Debug.Log("sPos: " + sPos);
        Debug.Log("farthest: " + farthest);
        return farthest;
    }

    void MakePathToDes()
    {
        List<Vector2Int> path = BFS(startPos, desPos);
        if (path == null || path.Count == 0)
        {
            GenerateEmptyMap();
            BFS(startPos, desPos);
            return;
        }

        for (int i = 1; i < path.Count - 1; i++)
        {
            Vector2Int pos = path[i];
            if (Random.value < 0.1f)
                Map[pos.x, pos.y] = 1;
            else
                Map[pos.x, pos.y] = 2;
        }

        Queue<Vector2Int> Q = new Queue<Vector2Int>();
        bool[,] visited = new bool[width + 2, height + 2];
        Vector2Int[] dirs = new Vector2Int[] { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };
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
                        Map[next.x, next.y] = (Random.value < 0.1f) ? 1 : 0;
                        Q.Enqueue(next);
                    }
                }
            }
        }
    }

    // BFS: List<Vecter2Int> 반환 (C++에서의 pair<int,int>)
    List<Vector2Int> BFS(Vector2Int start, Vector2Int des)
    {
        Queue<Vector2Int> queue = new Queue<Vector2Int>();
        Dictionary<Vector2Int, Vector2Int> parent = new Dictionary<Vector2Int, Vector2Int>();
        bool[,] visited = new bool[width + 2, height + 2];

        queue.Enqueue(start);
        visited[start.x, start.y] = true;
        Vector2Int[] dirs = new Vector2Int[] { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };

        while (queue.Count > 0)
        {
            Vector2Int current = queue.Dequeue();
            if (current == des)
                break;
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
        if (!parent.ContainsKey(des))
            return null;

        List<Vector2Int> path = new List<Vector2Int>();  // path 리스트는 경로 저장. 앞으로 확률에 따라 주어진 경로에는 랜덤한 타일이 배치됨
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

    // 타일 배치. 배열 그냥 전부 읽음 O(n^2)  
    // 플레이어 이동 가능 좌표는 (x-1)*tileSize, (y-1)*tileSize로 변환하여, 플레이 영역이 (0,0)부터 시작하는 것처럼 보임.
    void RenderMap()
    {
        // 기존 타일 파괴
        foreach (var tile in tileObjects.Values)
            Destroy(tile);
        tileObjects.Clear();

        GameObject mapParent = new GameObject("Map");
        for (int x = 0; x < width + 2; x++)
        {
            for (int y = 0; y < height + 2; y++)
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
                    Vector3 pos = new Vector3((x - 1) * tileSize + tileSize * 0.5f, (y - 1) * tileSize + tileSize * 0.5f, zPos);
                    GameObject tile = Instantiate(prefab, pos, Quaternion.identity, mapParent.transform);
                    tileObjects[new Vector2Int(x, y)] = tile;
                }
            }
        }
    }

    float manageZ(int type)
    {
        switch (type)
        {
            case 0: return -0.5f; // Wall
            case 1: return -0.6f; // Enemy
            case 2: return -1.0f; // Floor:
            case 3: return -0.8f; // NextLevelTile
            default: return 0f;
        }
    }

    void SpawnPlayer()
    {
        Vector3 pos = new Vector3((startPos.x - 1) * tileSize + tileSize * 0.5f, (startPos.y - 1) * tileSize + tileSize * 0.5f, -0.2f);
        GameObject player = Instantiate(PlayerPrefab, pos, Quaternion.identity);
        CameraController camera = Camera.main.GetComponent<CameraController>();
        if (camera != null)
            camera.target = player.transform;
    }

    // 기존 로직을 그대로 사용 /(10% 확률로 적, 90% 확률로 바닥)으로 변경
    void MakeExtraPath()
    {
        for (int x = 1; x <= width; x++)
        {
            for (int y = 1; y <= height; y++)
            {
                if (extraRoadCnt< extraMaxCnt/2) // 경로 max/2개까지 만듦, 지금은 20/2
                {
                    // 새로운 경로 시작 조건: 적이 아닌 타일(즉, 1이 아닌 타일)에서 시작하 && 8% 확률
                    if (Map[x, y] != 1 && Random.value < 0.08f)
                    {
                        extraRoadCnt++;
                        Vector2Int secondStartPos = new Vector2Int(x, y);
                        List<Vector2Int> path = BFS(secondStartPos, desPos);
                        if (path != null && path.Count > 1)
                        {
                            foreach (Vector2Int pos in path)
                            {
                                if (Map[pos.x, pos.y] != 3) // 혹시나 목적지가 오염되지 않게 예외 처리
                                    Map[pos.x, pos.y] = (Random.value < 0.1f) ? 1 : 2;
                            }
                        }
                    }
                }
            }
        }


        for (int x = width; x >= 1; x--)
        {
            for (int y = height; y >= 1; y--)
            {
                if (extraRoadCnt < extraMaxCnt) // 경로이번에는 오른쪽 아래부터 시작해서 만들기
                {
                    // 새로운 경로 시작 조건: 적이 아닌 타일(즉, 1이 아닌 타일)에서 시작하 && 8% 확률
                    if (Map[x, y] != 1 && Random.value < 0.08f)
                    {
                        extraRoadCnt++;
                        Vector2Int secondStartPos = new Vector2Int(x, y);
                        List<Vector2Int> path = BFS(secondStartPos, startPos);
                        if (path != null && path.Count > 1)
                        {
                            foreach (Vector2Int pos in path)
                            {
                                if (Map[pos.x, pos.y] != 3) // 혹시나 목적지가 오염되지 않게 예외 처리
                                    Map[pos.x, pos.y] = (Random.value < 0.1f) ? 1 : 2;
                            }
                        }
                    }
                }
            }
        }

        RenderMap();
    }

    void LastCheck() //이상한 곳에 생성된 벽 외 타일 제거
    {
        for (int x = 1; x <= width; x++)
        {
            for (int y = 1; y <= height; y++)
            {
                if (Map[x, y] != 2 && Map[x, y] != 3)
                {
                    bool hasAdjacentFloor = false;
                    if (y + 1 <= height && Map[x, y + 1] == 2)
                        hasAdjacentFloor = true;

                    if (y - 1 >= 1 && Map[x, y - 1] == 2)
                        hasAdjacentFloor = true;

                    if (x + 1 <= width && Map[x + 1, y] == 2)
                        hasAdjacentFloor = true;

                    if (x - 1 >= 1 && Map[x - 1, y] == 2)
                        hasAdjacentFloor = true;

                    if (!hasAdjacentFloor)
                        Map[x, y] = 0;
                }
            }
        }
        RenderMap();
    }

    // 플레이어 벽 못나가게 막기
    void MakeBoundWall()
    {
        for (int x = 0; x < width + 2; x++)
        {
            Map[x, 0] = 0;
            Map[x, height + 1] = 0;
        }
        for (int y = 0; y < height + 2; y++)
        {
            Map[0, y] = 0;
            Map[width + 1, y] = 0;
        }
        RenderMap();
    }

    bool IsInBounds(Vector2Int pos)
    {
        return pos.x >= 1 && pos.x <= width && pos.y >= 1 && pos.y <= height; // == 벽 태투리 제외 범위
    }
}
