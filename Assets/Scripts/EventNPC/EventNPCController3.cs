using UnityEngine;
using TMPro;
using UnityEngine.UI;


// 가위바위보 도박 할 수 있는 

public class EventNPCController3 : MonoBehaviour
{

    private SpriteRenderer spriteRenderer;

    [SerializeField]
    private Player player;
    
    [SerializeField]
    private PortalController portalController;

    [SerializeField]
    public TextMeshProUGUI talkText;

    [SerializeField]
    public GameObject talkPanel;

    [SerializeField]
    private GameManager gameManager;

    [SerializeField]
    private Button buttonYes;

    [SerializeField]
    private Button buttonNo;
    

    private bool isOnGame = false;
    private bool isGameButtonPushed = false;
    private enum rockScissorsPaper{rock = 0, scissors, paper};

    private bool isCoinOver50 = false;

    private int dialogIndex = 0;
    private string[] dialogs = {
        "가위바위보 도박을 하는 남자를 발견했다.", // 0 > 1 or 3(gameManager.Coin >= 50)
        "50코인도 없는 거렁뱅이는 썩 꺼지라는 말을 들었다.", // 1 > 2
        "남자는 곧 사라졌다", // 2 > 9
        "50코인이 있는 걸 보여주자 남자는 만족스럽게 웃었다.", // 3 > 4
        "50코인을 걸고 하는 가위바위보 내기에 참여할 것인가?", // 4 > 5 or 9 (버튼 선택)
        "당신은 무엇을 낼 것인가?", // 5 > 6 or 7 or 8 (어떤 걸 고르든 33% 확률)
        "가위바위보에서 이겼다! 당신은 남자의 50코인을 얻었다!", // 6 > 9
        "가위바위보에서 졌다. 당신은 50코인을 빼앗겼다.", // 7 > 9
        "가위바위보에서 비겼다.", // 8 > 5
        ""  // 대화 종료 시 빈 문자열
    };
    private bool wKeyPressed = false; // W 키가 눌렸는지 여부를 추적


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        portalController.setIsEventRoom(true);
  
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.W) && player.canDialoging)
        {
            if (!wKeyPressed) // W 키가 이전에 눌리지 않았을 때만 처리
            {
                wKeyPressed = true;
                player.isDialoging = true;
                talkPanel.SetActive(true);

                if (dialogIndex < dialogs.Length)
                {
                    talkText.text = dialogs[dialogIndex];

                    if (dialogIndex == 0 && gameManager.coin >= 50) // 코인 50개 이상인지 검사
                    {
                        isCoinOver50 = true;
                        dialogIndex = 3; // 그래야 한 번 더 커져서 3으로 감.
                    }

                    if(!isCoinOver50 && dialogIndex == 3)
                    {
                        dialogIndex = 9;
                    }

                    if(isCoinOver50 && dialogIndex == 4)
                    {
                        if(isGameButtonPushed)
                        {

                        }
                    }

                    dialogIndex++;

                    if (dialogIndex >= dialogs.Length)
                    {
                        talkText.text = "";
                        talkPanel.SetActive(false);
                        portalController.setIsEventOn(true);
                        player.isDialoging = false;
                        dialogIndex = 0; // 대화 종료 후 인덱스 초기화 (선택 사항)
                        if(!isCoinOver50)
                        {
                            gameObject.SetActive(false);
                        }
                    }
                }
            }
        }

        if (Input.GetKeyUp(KeyCode.W))
        {
            wKeyPressed = false; // W 키에서 손을 떼면 다시 누를 수 있도록 초기화
        }
    }

    public void OnClickedButtonYes()
    {
        isOnGame = true;
        isGameButtonPushed = true;
    }
    public void OnClickedButtonNo()
    {
        isOnGame = false;
        isGameButtonPushed = true;
    }
}
