using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;
using SoftProject.Enums;
using static SoftProject.Enums.MAP_TILE;
using UnityEngine.SceneManagement;
using TMPro;

/*
MAP_FLOOR는 2 이니까,
MAP_FLOOR로 쓰거나,
(MAP_TILE)2로 캐스팅해서 사용하기.

*/

public class MapController : MonoBehaviour
{

    // MapController 스크립트에 접근할 수 있게 만들어 줆. CharacterController Script에서의 접근 때문에 필수 
    public static MapController Instance
    {
        get; // 읽기용
        private set; //쓰기용
    }

    [SerializeField] private TextMeshProUGUI distanceText; //남은 거리 출력

    public GameObject EnemyTilePrefab; // 적 타일 
    public GameObject RandomTilePrefab; // 랜덤 이벤트 타일
    public GameObject FloorTilePrefab; // 바닥 타일 
    public GameObject NextTilePrefab;  // 다음 층으로 올라가는 타일 
    public GameObject WallTilePrefab;  // 벽 타일 
    public GameObject PlayerPrefab;    // 플레이어
    public GameObject StorePrefab; // 상점 프리팹
    public GameObject StonePillarPrefab; //석조 기둥 프리팹

    const int INF = 0x3f3f3f3f; //약 10.5억
    public int width = 40;
    public int height = 40;
    public int extraMaxCnt = 30;
    public float tileSize = 1f;
    public int maxExtraPathCnt = 10; // 7번 함수에 사용
    public int randomCycle = 180; // 적 타일을 랜덤 이벤트 타일로 변경 후 잠시 변환하지 않는 주기. 수정 가능
    int totalRandomMapCnt = 5; // 이벤트 타일의 총 개수

    public MAP_TILE[,] Map;
    private Vector2Int startPos;      // 시작 지점 (1~width, 1~height)
    private Vector2Int desPos;        // 도착 지점
    public GameObject player;        // 맵의 캐릭터 스프라이트

    // 생성된 타일 오브젝트를 관리용 dic //ReplaceTileAt 함수에 사용
    private Dictionary<Vector2Int, GameObject> tileObjects = new Dictionary<Vector2Int, GameObject>();


    public GameObject mapParent;
    [Header("랜덤 시드(0으로 두면 자동으로 랜덤 생성)")]
    public int seed = 0; //0이면 자동으로 새 시드 생성


    //현위치 타일의 상태 반환
    public MAP_TILE GetTileType(int x, int y)
    {
        return Map[x, y];
    }

    // 타일 교체 코드. (CharacterController 스크립트에서 참조중)
    public void ReplaceTile(int x, int y, MAP_TILE newTile) //현재 newTile은 2, 즉 Floor
    {
        Vector2Int ChangePos = new Vector2Int(x, y);

        // 기존 타일 삭제
        if (tileObjects.ContainsKey(ChangePos)) //해당 좌표에 타일 있다면
        {
            GameObject toDeleteTile = tileObjects[ChangePos];
            Destroy(toDeleteTile);
            tileObjects.Remove(ChangePos);
        }

        Map[x, y] = newTile; // 2 

        // 새 타일 생성
        GameObject prefab = GetNewTile(newTile);
        if (prefab == null) return;

        Vector3 pos = new Vector3(
            (x - 1) * tileSize + tileSize * 0.5f,
            (y - 1) * tileSize + tileSize * 0.5f,
            manageZ(newTile)
        );

        GameObject newTileObject = Instantiate(prefab, pos, Quaternion.identity, mapParent.transform); // 참조 반환
        tileObjects[ChangePos] = newTileObject;
    }

    // t값에 맞는 타일 반환 함수
    GameObject GetNewTile(MAP_TILE t)
    {
        switch (t)
        {
            case MAP_WALL: return WallTilePrefab;
            case MAP_ENEMY: return EnemyTilePrefab;
            case MAP_FLOOR: return FloorTilePrefab;
            case MAP_EXIT: return NextTilePrefab;
            case MAP_EVENT: return RandomTilePrefab;
            case MAP_SHOP: return StorePrefab;
            case MAP_PILLAR: return StonePillarPrefab;
            default: return null;
        }
    }

    //    이 안에서 Instance 를 자신(this)으로 세팅하고,
    //    DontDestroyOnLoad 로 씬 전환 후에도 맵 데이터를 유지시키죠.
    void Awake()
    {
        if (seed == 0)
            seed = System.Environment.TickCount;

        UnityEngine.Random.InitState(seed);
        Debug.Log($"[MapController] 사용된 시드 값: {seed}");

        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject); // 씬 바뀌어도 유지
        }
        else
        {
            // 이미 다른 씬에 MapController가 존재한다면, 중복제거를 위해 GameObject (=자기자신) 삭제
            Destroy(this.gameObject);
        }

        // 씬 로드 콜백 등록
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        bool isMapScene = scene.name == "MapTest";

        if (mapParent != null)
            mapParent.SetActive(isMapScene);

        if (player != null)
            player.SetActive(isMapScene);
    }

    void Start()
    {
        while (true)
        {
            GenerateEmptyMap();      // 1. 모든 맵을 벽으로 초기화
            SetStartAndGoal();       // 2. 시작점과 목적지 정하기

            MakePathToDes();         // 3. 경로 하나는 강제 연결
            MakeExtraPath();         // 4. 추가 경로 생성
            MakeVariablePath();      // 5. 벽으로부터 새로운 길을 만들어 미로를 다채롭게 바꾸는 함수

            ChangeLongFloor();       // 6. 바닥 타일의 길이가 너무 길 경우 수정해주는 함수                   
            CheckUseless();          // 7. 의미없는 타일 모두 벽 타일로 교체하는 함수
            CheckWhiteHole();       //  8. 바닥의 두께가 3칸 이상으로 두껍게 생성된 것을 수정하는 함수
            MakeRandomTile();       //  9. 랜덤 이벤트 타일을 생성하는 함수


            MakeStoreNextToDes();
            MakeStonePillar();
            MakeBoundWall();         // 10. 플레이어를 범위 밖으로 못나가게 벽 타일로 두루는 함수




            RenderMap();             // 11. 전체 타일 렌더링하는 함수
            SpawnPlayer();           // 12. 플레이어 생성 및 카메라 연결
            if (GetDistance() < INF) break; // INF는 미방문을 의미. 10억.
            // INF는 미방문을 의미. 10억.
            int d = GetDistance();
            Debug.Log(" current Cost : " + d);
            UpdateDistanceUI(d);
            if (d < INF) break;
            // LastCheck();
        }
    }

    void UpdateDistanceUI(int distance)
    {
        if (distanceText != null)
        {
            distanceText.text = "Distance: " + distance.ToString();
        }
    }

    // 0: Wall, 1 : Enemy, 2: Floor, 3: Des, 4: EventMap, 5: Store, 6: Pillar
    void GenerateEmptyMap()
    {
        Map = new MAP_TILE[width + 2, height + 2];
        for (int x = 0; x < width + 2; x++)
        {
            for (int y = 0; y < height + 2; y++)
            {
                Map[x, y] = MAP_WALL;
            }
        }
    }

    void SetStartAndGoal()
    {
        // 내부 영역 (플레이어 이동 가능 영역)
        startPos = new Vector2Int(Random.Range(width / 4, width / 4 * 3), Random.Range(height / 4, height / 4 * 3)); // 시작지점은 랜덤 좌표
        Map[startPos.x, startPos.y] = MAP_FLOOR; // 시작 지점 Floor 타일로 설정.
        Debug.Log("startPos: " + startPos);
        desPos = GetDes();
        Map[desPos.x, desPos.y] = MAP_EXIT; // 목적지는 3
    }

    Vector2Int GetDes()
    {
        Vector2Int[] corners = new Vector2Int[]
        {
        new Vector2Int(1, 1),
        new Vector2Int(1, height),
        new Vector2Int(width , 1),
        new Vector2Int(width, height)
        };

        int idx = Random.Range(0, corners.Length);
        Vector2Int des = corners[idx];

        Debug.Log("desPos: " + des);
        return des;
    }

    // MakePathToDes() : 최소 1개의 확정 경로를 만드는 함수.B
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
            if (pos != startPos && Random.value < 0.1f)
                Map[pos.x, pos.y] = MAP_ENEMY; // 적
            else
                Map[pos.x, pos.y] = MAP_FLOOR; // 벽
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

    void MakeStoreNextToDes()
    {
        // 0 : Wall, 1 : Enemy, 2 : Floor, 3 : des, 4: EventMap
        if (desPos.x == 1 && desPos.y == 1)
        {
            if (Map[2, 1] != MAP_WALL && Map[1, 2] == MAP_WALL)
                Map[2, 1] = MAP_SHOP;
            if (Map[1, 2] != MAP_WALL && Map[2, 1] == MAP_WALL)
                Map[1, 2] = MAP_SHOP;
            if (Map[2, 1] != MAP_WALL && Map[1, 2] != MAP_WALL)
                Map[2, 1] = MAP_SHOP;
        }

        if (desPos.x == 1 && desPos.y == height)
        {
            if (Map[2, height] != MAP_WALL && Map[1, height - 1] == MAP_WALL)
                Map[2, height] = MAP_SHOP;
            if (Map[1, height - 1] != MAP_WALL && Map[2, height] == MAP_WALL)
                Map[1, height - 1] = MAP_SHOP;
            if (Map[1, height - 1] != MAP_WALL && Map[2, height] != MAP_WALL)
                Map[1, height - 1] = MAP_SHOP;
        }

        if (desPos.x == width && desPos.y == 1) //!
        {
            if (Map[width - 1, 1] != MAP_WALL && Map[width, 2] == MAP_WALL)
                Map[width - 1, 1] = MAP_SHOP;
            if (Map[width, 2] != MAP_WALL && Map[width - 1, 1] == MAP_WALL)
                Map[width, 2] = MAP_SHOP;
            if (Map[width, 2] != MAP_WALL && Map[width - 1, 1] != MAP_WALL)
                Map[width - 1, 1] = MAP_SHOP;
        }

        if (desPos.x == width && desPos.y == height)
        {
            if (Map[width - 1, height] != MAP_WALL && Map[width, height - 1] == MAP_WALL)
                Map[width - 1, height] = MAP_SHOP;
            if (Map[width - 1, height] != MAP_WALL && Map[width, height - 1] == MAP_FLOOR)
                Map[width, height - 1] = MAP_SHOP;
            if (Map[width - 1, height] != MAP_WALL && Map[width, height - 1] != MAP_WALL)
                Map[width - 1, height] = MAP_SHOP;
        }
    }

    // 타일 배치. 배열 그냥 전부 읽음 O(n^2)  
    // 플레이어 이동 가능 좌표는 (x-1)*tileSize, (y-1)*tileSize로 변환하여, 플레이 영역이 (0,0)부터 시작하는 것처럼 보임.
    public void RenderMap()
    {
        // 기존 타일 파괴
        foreach (var tile in tileObjects.Values)
            Destroy(tile);
        tileObjects.Clear();

        mapParent = new GameObject("Map");
        DontDestroyOnLoad(mapParent);
        for (int x = 0; x < width + 2; x++)
        {
            for (int y = 0; y < height + 2; y++)
            {
                GameObject prefab = null;
                switch (Map[x, y])
                {
                    case MAP_WALL: prefab = WallTilePrefab; break;
                    case MAP_ENEMY: prefab = EnemyTilePrefab; break;
                    case MAP_FLOOR: prefab = FloorTilePrefab; break;
                    case MAP_EXIT: prefab = NextTilePrefab; break;
                    case MAP_EVENT: prefab = RandomTilePrefab; break;
                    case MAP_SHOP: prefab = StorePrefab; break;
                    case MAP_PILLAR: prefab = StonePillarPrefab; break;
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

    void SpawnPlayer()
    {
        Vector3 pos = new Vector3((startPos.x - 1) * tileSize + tileSize * 0.5f, (startPos.y - 1) * tileSize + tileSize * 0.5f, -2f);
        player = Instantiate(PlayerPrefab, pos, Quaternion.identity);
        DontDestroyOnLoad(player);
    }

    private void Update()
    {
        CameraController camera = Camera.main.GetComponent<CameraController>();
        if (camera != null)
            camera.target = player.transform;
    }

    // 기존 로직을 그대로 사용 /(10% 확률로 적, 90% 확률로 바닥)으로 변경
    void MakeExtraPath()
    {
        int extraRoadCnt = 0;
        for (int x = 1; x <= width; x++)
        {
            for (int y = 1; y <= height; y++)
            {
                if (extraRoadCnt < extraMaxCnt / 4) // 경로 max/2개까지 만듦,
                {
                    // 새로운 경로 시작 조건: 적이 아닌 타일(즉, 1이 아닌 타일)에서 시작하며 && 8% 확률
                    if (Map[x, y] != MAP_ENEMY && !(x == startPos.x && y == startPos.y) && Random.value < 0.08f)
                    {
                        extraRoadCnt++;
                        Vector2Int secondStartPos = new Vector2Int(x, y);
                        List<Vector2Int> path = BFS(secondStartPos, desPos);
                        if (path != null && path.Count > 1) // 경로 리스트에 적절한 값이 들어있다면
                        {
                            foreach (Vector2Int pos in path)
                            {
                                if (pos != startPos && Map[pos.x, pos.y] != MAP_EXIT)
                                    Map[pos.x, pos.y] = (Random.value < 0.1f) ? MAP_ENEMY : MAP_FLOOR;
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
                if (extraRoadCnt < extraMaxCnt / 4 * 2) // 경로이번에는 오른쪽 아래부터 시작해서 만들기
                {
                    // 새로운 경로 시작 조건: 적이 아닌 타일(즉, 1이 아닌 타일)에서 시작하 && 8% 확률
                    if (Map[x, y] != MAP_ENEMY && !(x == startPos.x && y == startPos.y) && Random.value < 0.08f)
                    {
                        extraRoadCnt++;
                        Vector2Int secondStartPos = new Vector2Int(x, y);
                        List<Vector2Int> path = BFS(secondStartPos, startPos);
                        if (path != null && path.Count > 1)
                        {
                            foreach (Vector2Int pos in path)
                            {
                                if (Map[pos.x, pos.y] != MAP_EXIT && !(x == startPos.x && y == startPos.y)) // 혹시나 목적지가 오염되지 않게 예외 처리
                                    Map[pos.x, pos.y] = (Random.value < 0.1f) ? MAP_ENEMY : MAP_FLOOR;
                            }
                        }
                    }
                }
            }
        }

        for (int x = width / 2; x >= 1; x--)
        {
            for (int y = height / 2; y >= 1; y--)
            {
                if (extraRoadCnt < extraMaxCnt / 4 * 4) // 경로이번에는 가운데 시작
                {
                    // 새로운 경로 시작 조건: 적이 아닌 타일(즉, 1이 아닌 타일)에서 시작하 && 8% 확률
                    if (Map[x, y] != MAP_ENEMY && !(x == startPos.x && y == startPos.y) && Random.value < 0.12f) // 약간 높음
                    {
                        extraRoadCnt++;
                        Vector2Int secondStartPos = new Vector2Int(x, y);
                        List<Vector2Int> path = BFS(secondStartPos, startPos);
                        if (path != null && path.Count > 1)
                        {
                            foreach (Vector2Int pos in path)
                            {
                                if (Map[pos.x, pos.y] != MAP_EXIT && !(x == startPos.x && y == startPos.y))
                                    Map[pos.x, pos.y] = (Random.value < 0.1f) ? MAP_ENEMY : MAP_FLOOR;
                            }
                        }
                    }
                }
            }
        }
    }

    void CheckUseless()
    {
        for (int x = 1; x <= width; x++)
        {
            for (int y = 1; y <= height; y++)
            {
                if (Map[x, y] != MAP_FLOOR && Map[x, y] != MAP_EXIT)
                {
                    if ((x == startPos.x && y == startPos.y) ||
                        (x == desPos.x && y == desPos.y))
                        continue;

                    bool hasAdjacentFloor = false;
                    if (IsInBounds(new Vector2Int(x, y + 1)) && Map[x, y + 1] == MAP_FLOOR)
                        hasAdjacentFloor = true;
                    if (IsInBounds(new Vector2Int(x, y - 1)) && Map[x, y - 1] == MAP_FLOOR)
                        hasAdjacentFloor = true;
                    if (IsInBounds(new Vector2Int(x + 1, y)) && Map[x + 1, y] == MAP_FLOOR)
                        hasAdjacentFloor = true;
                    if (IsInBounds(new Vector2Int(x - 1, y)) && Map[x - 1, y] == MAP_FLOOR)
                        hasAdjacentFloor = true;

                    if (!hasAdjacentFloor)
                        Map[x, y] = MAP_WALL;
                }
            }
        }
    }

    void CheckWhiteHole()
    {
        for (int x = 1; x <= width; x++)
        {
            for (int y = 1; y <= height; y++)
            {
                if (Map[x, y] != MAP_FLOOR)
                    continue;

                if (Map[x + 1, y] == MAP_FLOOR && Map[x, y - 1] == MAP_FLOOR && Map[x - 1, y] == MAP_FLOOR && Map[x + 1, y + 1] != 0 && Map[x + 1, y - 1] != 0 && Map[x - 1, y + 1] != 0 && Map[x - 1, y - 1] != 0) //2=floor
                {
                    Map[x, y] = 0;
                }
            }
        }
    }

    void ChangeLongFloor()
    {
        for (int x = 1; x <= width - 3; x++)
        {
            for (int y = 1; y <= height - 3; y++)
            {
                // 범위 이탈이면 스킵
                if (x - 3 < 1 || x + 3 > width || y - 3 < 1 || y + 3 > height)
                    continue;
                // 바닥이 아니라면 스킵
                if (Map[x, y] != MAP_FLOOR) continue;

                //시작지점 (플레이어 위) 라면 스킵
                if (x == startPos.x && y == startPos.y) continue;

                // 1+6칸 연속이면 수정함.  추후 수정 가능! (랜덤칸 및 적으로)

                if (Map[x + 1, y] == MAP_FLOOR && Map[x + 2, y] == MAP_FLOOR && Map[x + 3, y] == MAP_FLOOR &&
                    Map[x - 1, y] == MAP_FLOOR && Map[x - 2, y] == MAP_FLOOR && Map[x - 3, y] == MAP_FLOOR)
                {
                    if (x == startPos.x && y == startPos.y) continue;
                    Map[x, y] = MAP_ENEMY;
                }

                if (Map[x, y + 1] == MAP_FLOOR && Map[x, y + 2] == MAP_FLOOR && Map[x, y + 3] == MAP_FLOOR &&
                    Map[x, y - 1] == MAP_FLOOR && Map[x, y - 2] == MAP_FLOOR && Map[x, y - 3] == MAP_FLOOR)
                {
                    if (x == startPos.x && y == startPos.y) continue;
                    Map[x, y] = MAP_ENEMY;
                }
            }
        }
        // 범위에서 벗어나는 width-1 ~ width행 추가 검사
        for (int x = width - 1; x <= width; x++)
        {
            for (int y = 3; y <= height - 3; y++)
            {
                if (Map[x, y] != MAP_FLOOR) continue;

                if (x == startPos.x && y == startPos.y) continue;

                if (Map[x, y + 1] == MAP_FLOOR && Map[x, y + 2] == MAP_FLOOR && Map[x, y + 3] == MAP_FLOOR &&
                    Map[x, y - 1] == MAP_FLOOR && Map[x, y - 2] == MAP_FLOOR && Map[x, y - 3] == MAP_FLOOR)
                {
                    if (x == startPos.x && y == startPos.y) continue;
                    Map[x, y] = MAP_ENEMY;
                }
            }
        }

        //범위에서 벗어나는 1 ~ 2 행 추가 검사
        for (int x = 1; x <= 2; x++)
        {
            for (int y = 3; y <= height - 3; y++)
            {
                if (Map[x, y] != MAP_FLOOR) continue;

                if (x == startPos.x && y == startPos.y) continue;

                if (Map[x, y + 1] == MAP_FLOOR && Map[x, y + 2] == MAP_FLOOR && Map[x, y + 3] == MAP_FLOOR &&
                    Map[x, y - 1] == MAP_FLOOR && Map[x, y - 2] == MAP_FLOOR && Map[x, y - 3] == MAP_FLOOR)
                {
                    if (x == startPos.x && y == startPos.y) continue;
                    Map[x, y] = MAP_ENEMY;
                }
            }
        }

        //범위에서 벗어나는 1 ~ 2 열 추가 검사
        for (int y = 1; y <= 2; y++)
        {
            for (int x = 3; x <= width - 3; x++)
            {
                if (Map[x, y] != MAP_FLOOR) continue;
                if (x == startPos.x && y == startPos.y) continue;

                if (Map[x + 1, y] == MAP_FLOOR && Map[x + 2, y] == MAP_FLOOR && Map[x + 3, y] == MAP_FLOOR &&
                    Map[x - 1, y] == MAP_FLOOR && Map[x - 2, y] == MAP_FLOOR && Map[x - 3, y] == MAP_FLOOR)
                {
                    if (x == startPos.x && y == startPos.y) continue;
                    Map[x, y] = MAP_ENEMY;
                }
            }
        }

        //범위에서 벗어나는 height-1 ~ height 열 추가 검사
        for (int y = height - 1; y <= height; y++)
        {
            for (int x = 3; x <= width - 3; x++)
            {
                if (Map[x, y] != MAP_FLOOR) continue;
                if (x == startPos.x && y == startPos.y) continue;

                if (Map[x + 1, y] == MAP_FLOOR && Map[x + 2, y] == MAP_FLOOR && Map[x + 3, y] == MAP_FLOOR &&
                    Map[x - 1, y] == MAP_FLOOR && Map[x - 2, y] == MAP_FLOOR && Map[x - 3, y] == MAP_FLOOR)
                {
                    if (x == startPos.x && y == startPos.y) continue;
                    Map[x, y] = MAP_ENEMY;
                }
            }
        }
    }

    void MakeRandomTile()
    {
        int curRandomMapCnt = 0;
        int curRandomCycle = 0;
        int lastX = 0;
        int lastY = 0;

        while (curRandomMapCnt < totalRandomMapCnt)
        {
            lastX = 0;
            lastY = 0;
            for (int x = lastX; x <= width; x++)
            {
                for (int y = lastY; y <= height; y++)
                {
                    curRandomCycle++;
                    if (curRandomCycle > randomCycle && curRandomMapCnt < totalRandomMapCnt)
                    {
                        // 현재 검사 타일이 적 타일 이면서, 10퍼센트의 확률이라면
                        if (Map[x, y] == MAP_ENEMY && Random.value < 0.1f)
                        {
                            if (x == startPos.x && y == startPos.y) continue;
                            Map[x, y] = MAP_EVENT;
                            curRandomMapCnt++;
                            curRandomCycle = 0;
                            lastX = x;
                            lastY = y;
                        }
                    }
                }
            }
        }
        Debug.Log("curRandomMapCnt: " + curRandomMapCnt);

    }

    void MakeVariablePath()
    {
        int curExtraCnt = 0;

        for (int x = 2; x <= width - 2; x++)
        {
            for (int y = 2; y <= height - 2; y++)
            {
                if (Map[x, y] != MAP_WALL)
                    continue;
                // 추가 경로 개수 제한.많으면 많을 수록 연결된 길이 수없이 많아져서 미로로서의 기능은 상실 우려 그러나 유저의 순식간의 도착 방지 가능
                if (curExtraCnt > maxExtraPathCnt)
                    continue;

                // 왼쪽만 floor, 나머지 3방향이 벽
                if (Map[x - 1, y] == MAP_FLOOR && Map[x + 1, y] == MAP_WALL &&
                    Map[x + 2, y] == MAP_WALL && Map[x, y - 1] == MAP_WALL && Map[x, y + 1] == MAP_WALL)
                {
                    Map[x, y] = MAP_FLOOR;
                    Map[x + 1, y] = MAP_FLOOR;
                    Map[x + 2, y] = MAP_FLOOR;

                    CarveFrom(new Vector2Int(x + 2, y));
                    curExtraCnt++;
                }
                // 오른쪽만 floor, 나머지 3방향이 벽
                else if (Map[x + 1, y] == MAP_FLOOR && Map[x - 1, y] == MAP_WALL &&
                         Map[x - 2, y] == MAP_WALL && Map[x, y - 1] == MAP_WALL && Map[x, y + 1] == MAP_WALL)
                {
                    Map[x, y] = MAP_FLOOR;
                    Map[x - 1, y] = MAP_FLOOR;
                    Map[x - 2, y] = MAP_FLOOR;

                    CarveFrom(new Vector2Int(x - 2, y));
                    curExtraCnt++;
                }
                // 상단만 floor, 나머지 3방향이 벽
                else if (Map[x, y + 1] == MAP_FLOOR && Map[x, y - 1] == MAP_WALL &&
                         Map[x, y - 2] == MAP_WALL && Map[x - 1, y] == MAP_WALL && Map[x + 1, y] == MAP_WALL)
                {
                    Map[x, y] = MAP_FLOOR;
                    Map[x, y - 1] = MAP_FLOOR;
                    Map[x, y - 2] = MAP_FLOOR;

                    CarveFrom(new Vector2Int(x, y - 2));
                }
                // 하단만 floor, 나머지 3방향이 벽
                else if (Map[x, y - 1] == MAP_FLOOR && Map[x, y + 1] == MAP_WALL &&
                         Map[x, y + 2] == MAP_WALL && Map[x - 1, y] == MAP_WALL && Map[x + 1, y] == MAP_WALL)
                {
                    Map[x, y] = MAP_FLOOR;
                    Map[x, y + 1] = MAP_FLOOR;
                    Map[x, y + 2] = MAP_FLOOR;

                    CarveFrom(new Vector2Int(x, y + 2));
                    curExtraCnt++;
                }
            }
        }
    }

    void MakeStonePillar()
    {
        // 상하좌우가 모두 FloorTile일때, 그 타일을 기둥 장식타일로
        for (int x = 1; x <= width - 3; x++)
        {
            for (int y = 1; y <= height - 3; y++)
            {
                if (Map[x, y + 1] == MAP_FLOOR && Map[x, y - 1] == MAP_FLOOR && Map[x - 1, y] == MAP_FLOOR && Map[x + 1, y] == MAP_FLOOR)
                {
                    Map[x, y] = MAP_PILLAR;
                }
            }
        }
    }

    // 공통 BFS 경로 생성 부분
    void CarveFrom(Vector2Int variableStart)
    {
        List<Vector2Int> path = BFS(variableStart, desPos);
        if (path == null || path.Count < 2)
            return;

        for (int i = 1; i < path.Count - 1; i++)
        {
            Vector2Int pos = path[i];
            if (pos != startPos)
                Map[pos.x, pos.y] = (Random.value < 0.1f) ? MAP_ENEMY : MAP_FLOOR;
        }
    }

    // 플레이어 벽 못나가게 막기
    void MakeBoundWall()
    {
        for (int x = 0; x < width + 2; x++)
        {
            Map[x, 0] = MAP_WALL;
            Map[x, height + 1] = MAP_WALL;
        }
        for (int y = 0; y < height + 2; y++)
        {
            Map[0, y] = MAP_WALL;
            Map[width + 1, y] = MAP_WALL;
        }
    }

    //마지막 디버깅 함수
    void LastCheck()
    {
        // 시작지점이 적 타일
        if (Map[startPos.x, startPos.y] != MAP_FLOOR)
        {
            startPos.x++;
            LastCheck();
        }

        //시작 지점이 싹 다 가로막힘
        if (Map[startPos.x - 1, startPos.y] != MAP_FLOOR && Map[startPos.x + 1, startPos.y] != MAP_FLOOR &&
            Map[startPos.x, startPos.y + 1] != MAP_FLOOR && Map[startPos.x, startPos.y - 1] != MAP_FLOOR)
        {
            startPos.y++;
            LastCheck();
        }
    }

    float manageZ(MAP_TILE type)
    {
        switch (type)
        {
            //Z가 작을 수록 앞으로 나온다.
            case MAP_WALL: return -0.5f; // WallTile
            case MAP_ENEMY: return -0.6f; // EnemyTile
            case MAP_FLOOR: return -1.0f; // FloorTil(MAP_TILE)e:
            case MAP_EXIT: return -0.8f; // DesTile
            case MAP_EVENT: return -0.9f; // EventTile
            case MAP_SHOP: return -0.7f; //StoreTile
            case MAP_PILLAR: return -1.1f; //StonePillar
            default: return 0f;
        }
    }
    bool IsInBounds(Vector2Int pos)
    {
        return pos.x >= 1 && pos.x <= width && pos.y >= 1 && pos.y <= height; // == 벽 태투리 제외 범위
    }

    //목적지에 도달하기 위해 밟아야 하는 최소한의 MAP_ENEMY의 수.
    int GetDistance()
    {
        Queue<Vector2Int> que = new Queue<Vector2Int>();
        int[,] dist = new int[width + 2, height + 2];
        for (int i = 0; i < width + 2; i++)
        {
            for (int j = 0; j < height + 2; j++)
            {
                dist[i, j] = INF;
            }
        }

        que.Enqueue(startPos);
        dist[startPos.x, startPos.y] = 0;
        Vector2Int[] dirs = new Vector2Int[] { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };

        while (que.Count > 0)
        {
            Vector2Int cur = que.Dequeue();
            if (cur == desPos) break;

            foreach (Vector2Int dir in dirs)
            {
                Vector2Int next = cur + dir;
                if (!IsInBounds(next)) continue;
                if (Map[next.x, next.y] == MAP_WALL) continue;
                if (Map[next.x, next.y] == MAP_PILLAR) continue;

                int ndist = dist[cur.x, cur.y];
                if (Map[next.x, next.y] == MAP_ENEMY) ndist++;
                if (ndist >= dist[next.x, next.y]) continue;
                dist[next.x, next.y] = ndist;
                que.Enqueue(next);
            }
        }

        return dist[desPos.x, desPos.y];
    }

}