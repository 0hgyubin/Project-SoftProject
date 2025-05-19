using UnityEngine;
using TMPro;
using UnityEngine.UI;


public class EventNPCController8 : MonoBehaviour
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
    private GameObject buttonYesObject; // ButtonYes 게임 오브젝트를 연결할 변수
    private Button buttonYes;

    [SerializeField]
    private GameObject buttonNoObject;   // ButtonNo 게임 오브젝트를 연결할 변수
    private Button buttonNo;

    private bool isPlayerSelected = false;

    private bool isButtonPushed = false;

    private bool onFirstDialoge = false;

    private int dialogIndex = 0;
    private string[] dialogs = {
        "검정 선글라스를 쓴 흑인 남자가 수상한 제안을 했다.",
        "알약을 먹으면 강력한 힘을 가질 수 있다고 한다. 하지만 극심한 고통이 따른다고 한다.",
        "알약을 먹을 것인가?",
        " ",
        "알약을 먹으니 손가락 끝이 여러 개로 갈라지는 것 같은 끔찍한 고통이 느껴졌다. 하지만 고통이 끝나자 팔에 들어가는 힘이 2배로 강해진 것 같은 기분이다.",
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

                if (dialogIndex < dialogs.Length)
                {
                    talkText.text = dialogs[dialogIndex];

                    if(dialogIndex == 3)
                    {
                        buttonYesObject.SetActive(true);
                        buttonNoObject.SetActive(true);
                    }
                    else if(dialogIndex == 4)
                    {
                        player.attackDamage = player.attackDamage * 2;
                        player.hpUI.TakeDamaged(70);
                    }


                    dialogIndex++;


                    if(dialogIndex == 4 && !isButtonPushed)
                    {
                        dialogIndex = 3; //버튼 선택 안 했을 때 대비
                    }



                    if (dialogIndex >= dialogs.Length)
                    {
                        talkText.text = "";
                        talkPanel.SetActive(false);
                        portalController.setIsEventOn(true);
                        player.isDialoging = false;
                        dialogIndex = 0; // 대화 종료 후 인덱스 초기화 (선택 사항)
                        isButtonPushed = false;
                        onFirstDialoge = false;
                    }
                }
            //}
        }

        isPlayerSelected = false; // 이 부분을 Update 함수의 맨 마지막으로 이동
    }

    public void OnClickedButtonYes()
    {
        isPlayerSelected = true;
        isButtonPushed = true;
        dialogIndex++;
        // wKeyPressed = false;
    }
    public void OnClickedButtonNo()
    {
        isPlayerSelected = true;
        isButtonPushed = true;
        dialogIndex = 5;
        // wKeyPressed = false;
    }

}