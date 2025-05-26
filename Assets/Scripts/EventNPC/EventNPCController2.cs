using UnityEngine;
using TMPro;


// 보물상자에서 아이템 랜덤하게 하나 얻는 이벤트트
public class EventNPCController2 : MonoBehaviour
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

    // [SerializeField]
    // public GameObject[] droppedItem;
    // 나중에 아이템들 모아지면 여기서 랜덤으로 선택해서 아이템 떨어트리기.

    private int dialogIndex = 0;
    private string[] dialogs = {
        "보물상자를 열어보았다.",
        "보물상자 속 아이템을 발견했다.",
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

        if (Input.GetKeyDown(KeyCode.W) && onFirstDialoge)
        {
            // if (!wKeyPressed) // W 키가 이전에 눌리지 않았을 때만 처리
            // {
            //     wKeyPressed = true;
            player.isDialoging = true;
            talkPanel.SetActive(true);

            if (dialogIndex < dialogs.Length)
            {
                talkText.text = dialogs[dialogIndex];

                if (dialogIndex == 1) // 두 번째 대사에서 아이템 떨어트림.
                {
                    // droppedItem 배열 크기만큼 랜덤 숫자 뽑기
                    // droppedItem[랜덤 숫자]에 해당하는 아이템 프리팹 떨어트리기
                }

                dialogIndex++;

                if (dialogIndex >= dialogs.Length)
                {
                    talkText.text = "";
                    talkPanel.SetActive(false);
                    portalController.setIsEventOn(true);
                    player.isDialoging = false;
                    dialogIndex = 0; // 대화 종료 후 인덱스 초기화 (선택 사항)
                    gameObject.SetActive(false);
                    onFirstDialoge = false;
                }
                //}
            }
        }

        // if (Input.GetKeyUp(KeyCode.W))
        // {
        //     wKeyPressed = false; // W 키에서 손을 떼면 다시 누를 수 있도록 초기화
        // }
    }
}
