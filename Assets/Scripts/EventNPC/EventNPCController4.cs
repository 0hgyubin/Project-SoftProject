using UnityEngine;
using TMPro;

public class EventNPC4 : MonoBehaviour
{
    [SerializeField]
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

    private int dialogIndex = 0;
    private string[] dialogs = {
        "수상해보이는 남자가 빠르게 달려왔다.",
        "달리면서 부딪힌 남자는 그대로 사라졌다.",
        "!!!",
        "남자가 50골드를 가져갔다.",
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
        if (Input.GetKeyDown(KeyCode.W) && player.canDialoging)
        {
            // if (!wKeyPressed) // W 키가 이전에 눌리지 않았을 때만 처리
            // {
            //     wKeyPressed = true;
                player.isDialoging = true;
                talkPanel.SetActive(true);

                if (dialogIndex < dialogs.Length)
                {
                    talkText.text = dialogs[dialogIndex];

                    if (dialogIndex == 2) 
                    {
                        gameManager.coin -= 50;
                    }

                    if(dialogIndex == 1)
                    {
                        spriteRenderer.enabled = false;
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
                    }
                //}
            }
        }
    }
}
