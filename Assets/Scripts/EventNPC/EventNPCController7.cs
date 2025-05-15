using UnityEngine;
using TMPro;

public class EventNPCController7 : MonoBehaviour
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
        "분수대를 발견했다.",
        "분수대에는 체력을 회복할 수 있는 물이라고 적혀 있었다.",
        "깨끗한 물은 마셔도 될 것처럼 보였다.",
        "물을 마시니 상처가 회복되고 체력이 차올랐다.",
        "" // 대화 종료 시 빈 문자열
    };
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        portalController.setIsEventRoom(true);        
    }

    // Update is called once per frame
    void Update()
    {

        if(Input.GetKeyDown(KeyCode.W) && player.canDialoging)
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

                    if (dialogIndex == 3) // 세세 번째 대사에서 체력 감소
                    {
                        player.hpUI.TakeDamaged(-30);
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
                }
            //}
        }

        // if (Input.GetKeyUp(KeyCode.W))
        // {
        //     wKeyPressed = false; // W 키에서 손을 떼면 다시 누를 수 있도록 초기화
        // }       
    }
}
