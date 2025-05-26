using UnityEngine;
using TMPro;


//보물상자 열었더니 미믹의 공격의 당해 체력 깎이는 이벤트

public class EventNPCController : MonoBehaviour
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

    private bool onFirstDialoge = false;

    private int dialogIndex = 0;
    private string[] dialogs = {
        "보물상자를 열어보았다.",
        "보물상자 속 가시에 찔려 체력이 20 깎였다.",
        "보물상자 속에는 아무 것도 없었다.",
        "" // 대화 종료 시 빈 문자열
    };
    //private bool wKeyPressed = false; // W 키가 눌렸는지 여부를 추적


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        portalController.SetIsEventRoom(true);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.W) && player.canDialoging)
        {
            onFirstDialoge = true;
        }
        // if (!wKeyPressed) // W 키가 이전에 눌리지 않았을 때만 처리
        // {
        //     wKeyPressed = true;
        if (Input.GetKeyDown(KeyCode.W) && onFirstDialoge)
        {
            player.isDialoging = true;
            talkPanel.SetActive(true);

            if (dialogIndex < dialogs.Length)
            {
                talkText.text = dialogs[dialogIndex];

                if (dialogIndex == 1) // 두 번째 대사에서 체력 감소
                {
                    player.hpUI.TakeDamaged(20);
                }

                dialogIndex++;

                if (dialogIndex >= dialogs.Length)
                {
                    talkText.text = "";
                    talkPanel.SetActive(false);
                    portalController.SetIsEventOn(true);
                    player.isDialoging = false;
                    dialogIndex = 0; // 대화 종료 후 인덱스 초기화 (선택 사항)
                    gameObject.SetActive(false);
                    onFirstDialoge = false;
                }
            }
            //}
        }


        // if (Input.GetKeyUp(KeyCode.W))
        // {
        //     wKeyPressed = false; // W 키에서 손을 떼면 다시 누를 수 있도록 초기화
        // }
    }
}
