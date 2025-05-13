using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class EventNPCController6 : MonoBehaviour
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
    private GameObject button1_1Object; // ButtonYes 게임 오브젝트를 연결할 변수
    private Button button1_1;
    [SerializeField]
    private GameObject button1_2Object; // ButtonYes 게임 오브젝트를 연결할 변수
    private Button button1_2;
    [SerializeField]
    private GameObject button1_3Object; // ButtonYes 게임 오브젝트를 연결할 변수
    private Button button1_3;
    [SerializeField]
    private GameObject button2_1Object; // ButtonYes 게임 오브젝트를 연결할 변수
    private Button button2_1;
    [SerializeField]
    private GameObject button2_2Object; // ButtonYes 게임 오브젝트를 연결할 변수
    private Button button2_2;
    [SerializeField]
    private GameObject button2_3Object; // ButtonYes 게임 오브젝트를 연결할 변수
    private Button button2_3;
    [SerializeField]
    private GameObject button3_1Object; // ButtonYes 게임 오브젝트를 연결할 변수
    private Button button3_1;
    [SerializeField]
    private GameObject button3_2Object; // ButtonYes 게임 오브젝트를 연결할 변수
    private Button button3_2;
    [SerializeField]
    private GameObject button3_3Object; // ButtonYes 게임 오브젝트를 연결할 변수
    private Button button3_3;

    private bool isPlayerSelected = false;
    private bool isAnswer1Pushed = false;
    private bool isAnswer2Pushed = false;
    private bool isAnswer3Pushed = false;

    [SerializeField]
    private GameObject coin;
    


    private int dialogIndex = 0;
    private string[] dialogs = {
        "숭실대 마스코트 슝슝이를 만났다", //0 > 1
        "슝슝이의 퀴즈를 모두 푼다면 거대한 보상이 있다고 한다.", //1 > 2
        "틀린다면 화가 난 슝슝이가 당신을 때릴 것이다.", //2 > 3
        "Q1. 숭실대의 상징 동물은?", //3 > 4
        " ", // 4 > 5 or 10
        "Q2. 숭실대가 있는 시는?", //5 > 6
        " ", // 6 > 7 or 10
        "Q3. 숭실대에 없는 학과는?", // 7 > 8
        " ", // 8 > 9 or 10
        "모두 맞춘 당신은 슝슝이의 축복(이동속도 10% 증가)와 500골드를 받았다!", // 9 > 11
        "답을 틀렸다. 화가 난 슝슝이의 말굽이 당신을 때렸다.", // 11  > 12(대화 종료)
        ""
    };
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        portalController.setIsEventRoom(true);
        button1_1 = button1_1Object.GetComponent<Button>();
        button1_2 = button1_2Object.GetComponent<Button>();
        button1_3 = button1_3Object.GetComponent<Button>();
        button2_1 = button2_1Object.GetComponent<Button>();
        button2_2 = button2_2Object.GetComponent<Button>();
        button2_3 = button2_3Object.GetComponent<Button>();
        button3_1 = button3_1Object.GetComponent<Button>();
        button3_2 = button3_2Object.GetComponent<Button>();
        button3_3 = button3_3Object.GetComponent<Button>();
    }

    // Update is called once per frame
    void Update()
    {
        if ((Input.GetKeyDown(KeyCode.W) || isPlayerSelected) && player.canDialoging)
        {
            // if (!wKeyPressed) // W 키가 이전에 눌리지 않았을 때만 처리
            // {

                //wKeyPressed = true;
                player.isDialoging = true;
                talkPanel.SetActive(true);
                button1_1Object.SetActive(false);
                button1_2Object.SetActive(false);
                button1_3Object.SetActive(false);
                button2_1Object.SetActive(false);
                button2_2Object.SetActive(false);
                button2_3Object.SetActive(false);
                button3_1Object.SetActive(false);
                button3_2Object.SetActive(false);
                button3_3Object.SetActive(false);
                if (dialogIndex < dialogs.Length)
                {
                    talkText.text = dialogs[dialogIndex];

                    if(dialogIndex == 4)
                    {
                        button1_1Object.SetActive(true);
                        button1_2Object.SetActive(true);
                        button1_3Object.SetActive(true);
                    }
                    if(dialogIndex == 6)
                    {
                        button2_1Object.SetActive(true);
                        button2_2Object.SetActive(true);
                        button2_3Object.SetActive(true);
                    }
                    if(dialogIndex == 8)
                    {
                        button3_1Object.SetActive(true);
                        button3_2Object.SetActive(true);
                        button3_3Object.SetActive(true);
                    }

                    if(dialogIndex == 10)
                    {
                        player.hpUI.TakeDamaged(5);
                    }

                    if(dialogIndex == 9)
                    {
                        Debug.Log("이게됨?");
                        talkText.text = dialogs[dialogIndex];
                        player.MoveSpeed += 2;
                        for(int i = 0 ; i < 500; i++){
                             Instantiate(coin, transform.position, Quaternion.identity);
                         }
                        dialogIndex = 10;
                    }



                    




                    dialogIndex++;


                    if(dialogIndex == 5 && !isAnswer1Pushed)
                    {
                        dialogIndex = 4;
                    }
                    if(dialogIndex == 7 && !isAnswer2Pushed)
                    {
                        dialogIndex = 6;
                    }
                    if(dialogIndex == 9 && !isAnswer3Pushed)
                    {
                        dialogIndex = 8;
                    }



                    if (dialogIndex >= dialogs.Length)
                    {
                        talkText.text = "";
                        talkPanel.SetActive(false);
                        portalController.setIsEventOn(true);
                        player.isDialoging = false;
                        isAnswer1Pushed = false;
                        isAnswer2Pushed = false;
                        isAnswer3Pushed = false;
                        dialogIndex = 0; // 대화 종료 후 인덱스 초기화 (선택 사항)
                    }
                }
            //}
        }

        isPlayerSelected = false; // 이 부분을 Update 함수의 맨 마지막으로 이동        
    }


    public void OnClickedButton1Correct()
    {
        dialogIndex = 5;
        isPlayerSelected = true;
        isAnswer1Pushed = true;
    }

    public void OnClickedButton1Incorrect()
    {
        dialogIndex = 10;
        isPlayerSelected = true;
        isAnswer1Pushed = true;
    }

    public void OnClickedButton2Correct()
    {
        dialogIndex = 7;
        isPlayerSelected = true;
        isAnswer1Pushed = true;
    }

    public void OnClickedButton2Incorrect()
    {
        dialogIndex = 10;
        isPlayerSelected = true;
        isAnswer1Pushed = true;
    }

    public void OnClickedButton3Correct()
    {
        dialogIndex = 9;
        isPlayerSelected = true;
        isAnswer1Pushed = true;
    }

    public void OnClickedButton3Incorrect()
    {
        dialogIndex = 10;
        isPlayerSelected = true;
        isAnswer1Pushed = true;
    }
}
