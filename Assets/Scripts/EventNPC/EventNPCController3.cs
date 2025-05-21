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
    private GameObject buttonYesObject; // ButtonYes 게임 오브젝트를 연결할 변수
    private Button buttonYes;

    [SerializeField]
    private GameObject buttonNoObject;   // ButtonNo 게임 오브젝트를 연결할 변수
    private Button buttonNo;


    [SerializeField]
    private GameObject buttonRockObject;   // ButtonNo 게임 오브젝트를 연결할 변수
    private Button buttonRock;

    [SerializeField]
    private GameObject buttonScissorsObject;   // ButtonNo 게임 오브젝트를 연결할 변수
    private Button buttonScissors;

    [SerializeField]
    private GameObject buttonPaperObject;   // ButtonNo 게임 오브젝트를 연결할 변수
    private Button buttonPaper;


    private bool isPlayerSelected = false;
    private enum isOnGamble{no = 0, yes = 1, notPushed = 2};
    isOnGamble onGamble = isOnGamble.notPushed;
    private enum gamble{Win = 0, lose = 1, draw = 2 ,notPushed = 3};
    gamble gambleResult = gamble.notPushed;

    private bool isGambleOver = false;

    private bool isCoinOver50 = false;

    private int dialogIndex = 0;

    private bool onFirstDialoge = false;
    private string[] dialogs = {
        "가위바위보 도박을 하는 고블린을 발견했다.", // 0 > 1 or 3(gameManager.Coin >= 50)
        "50코인도 없는 거렁뱅이는 썩 꺼지라는 말을 들었다.", // 1 > 2
        "고블린은 곧 사라졌다", // 2 > 9
        "50코인이 있는 걸 보여주자 고블린은 만족스럽게 웃었다.", // 3 > 4
        "50코인을 걸고 하는 가위바위보 내기에 참여할 것인가?", // 4 > 5 (버튼 선택)
        " ", //게임 참가 고를 때. 게임 참여 시 5 > 6, 게임 미참여 시 5 > 11
        "당신은 무엇을 낼 것인가?", // 6 > 8 or 9 or 10 (어떤 걸 고르든 33% 확률)
        " ",
        "가위바위보에서 이겼다! 당신은 고블린의 50코인을 얻었다!", // 8 > 11
        "가위바위보에서 졌다. 당신은 50코인을 빼앗겼다.", // 9 > 11
        "가위바위보에서 비겼다.", // 10 > 6
        ""   // 대화 종료 시 빈 문자열
    };
    //private bool wKeyPressed = false; // W 키가 눌렸는지 여부를 추적


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        portalController.setIsEventRoom(true);


        // Button 컴포넌트 캐싱
        buttonYes = buttonYesObject.GetComponent<Button>();
        buttonNo = buttonNoObject.GetComponent<Button>();
        buttonRock = buttonRockObject.GetComponent<Button>();
        buttonScissors = buttonScissorsObject.GetComponent<Button>();
        buttonPaper = buttonPaperObject.GetComponent<Button>();

    }

    // Update is called once per frame
    void Update()
    {

        if(Input.GetKeyDown(KeyCode.W) && player.canDialoging)
        {
            onFirstDialoge = true;
        }
        if ((Input.GetKeyDown(KeyCode.W) || isPlayerSelected) && onFirstDialoge)
        {
            // if (!wKeyPressed) // W 키가 이전에 눌리지 않았을 때만 처리
            // {

                //wKeyPressed = true;
                player.isDialoging = true;
                talkPanel.SetActive(true);
                buttonYesObject.SetActive(false);
                buttonNoObject.SetActive(false);
                buttonRockObject.SetActive(false);
                buttonScissorsObject.SetActive(false);
                buttonPaperObject.SetActive(false);

                if (dialogIndex < dialogs.Length)
                {
                    talkText.text = dialogs[dialogIndex];

                    if(isGambleOver)
                    {
                        dialogIndex = 11;
                    }



                    if (dialogIndex == 0 && gameManager.coin >= 50) // 코인 50개 이상인지 검사
                    {
                        isCoinOver50 = true;
                        onGamble = isOnGamble.notPushed;
                        dialogIndex = 2; // 그래야 한 번 더 커져서 3으로 감.
                    }

                    if(!isCoinOver50 && dialogIndex == 3)
                    {
                        dialogIndex = 11; //대화 종료
                    }



                    if(isCoinOver50 && dialogIndex == 5)
                    {
                        buttonYesObject.SetActive(true);
                        buttonNoObject.SetActive(true);
                    }




                    if(isCoinOver50 && dialogIndex == 7)
                    {
                        buttonRockObject.SetActive(true);
                        buttonScissorsObject.SetActive(true);
                        buttonPaperObject.SetActive(true);
                    }



                    if(isCoinOver50 && dialogIndex == 8)
                    {
                        gameManager.coin = gameManager.coin + 50;
                        isGambleOver = true;
                        //dialogIndex = 11;
                    }

                    if(isCoinOver50 && dialogIndex == 9)
                    {
                        gameManager.coin = gameManager.coin - 50;
                        isGambleOver = true;
                        //dialogIndex = 11;
                    }

                    if(isCoinOver50 && dialogIndex == 10)
                    {
                        dialogIndex = 5;
                    }


                    dialogIndex++;


                    if(isCoinOver50 && dialogIndex == 6 && onGamble == isOnGamble.notPushed)
                    {
                        dialogIndex = 5; //버튼 선택 안 했을 때 대비
                    }

                    if(isCoinOver50 && dialogIndex == 8 && gambleResult == gamble.notPushed)
                    {
                        dialogIndex = 7;
                    }


                    if (dialogIndex >= dialogs.Length)
                    {
                        talkText.text = "";
                        talkPanel.SetActive(false);
                        portalController.setIsEventOn(true);
                        player.isDialoging = false;
                        dialogIndex = 0; // 대화 종료 후 인덱스 초기화 (선택 사항)
                        onGamble = isOnGamble.notPushed;
                        gambleResult = gamble.notPushed;
                        isGambleOver = false;
                        isCoinOver50 = false;
                        onFirstDialoge = false;
                        if(!isCoinOver50)
                        {
                            gameObject.SetActive(false);
                        }
                    }
                }
            //}
        }

        isPlayerSelected = false; // 이 부분을 Update 함수의 맨 마지막으로 이동
    }

    public void OnClickedButtonYes()
    {
        isPlayerSelected = true;
        onGamble = isOnGamble.yes;
        dialogIndex = 6;
        // wKeyPressed = false;
    }
    public void OnClickedButtonNo()
    {
        isPlayerSelected = true;
        onGamble = isOnGamble.no;
        dialogIndex = 11;
        // wKeyPressed = false;
    }
    public void OnClickedRokcScissorsPaper()
    {
        isPlayerSelected = true;
        // wKeyPressed = false;
        gambleResult = (gamble)Random.Range(0,3);
        if(gambleResult == (gamble)0)
        {
            dialogIndex = 8;
        }
        else if(gambleResult == (gamble)1)
        {
            dialogIndex = 9;
        }
        else if(gambleResult == (gamble)2)
        {
            dialogIndex = 10;
        }
    }
}