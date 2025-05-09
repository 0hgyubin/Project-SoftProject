using UnityEngine;
using TMPro;

public class DialogManager : MonoBehaviour
{
    [SerializeField]
    public TextMeshProUGUI talkText;

    [SerializeField]
    public GameObject talkPanel;

    public bool isAction = false;

    public void Action(GameObject eventNPC)
    {     
        if(isAction)
        {
            isAction = false;
        }
        else
        {
            isAction = true;
            talkText.text = "대화 상자 사용해보기";
        }
        
        talkPanel.SetActive(isAction);
    }

}
