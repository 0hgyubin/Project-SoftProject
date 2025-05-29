using UnityEngine;
using UnityEngine.SceneManagement;

public class PortalController : MonoBehaviour
{

    private SpriteRenderer spriteRenderer;

    private bool isEventRoom = false;
    private bool isEventOn = false;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.enabled = false; 
    }

    // Update is called once per frame
    void Update()
    {
        if(!isEventRoom)
        {
            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
            if (enemies.Length == 0)
            {
               spriteRenderer.enabled = true; // 적이 모두 제거되면 SpriteRenderer를 켭니다.
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                if (player != null)
                {
                    float distance = Vector3.Distance(transform.position, player.transform.position);
                    if (distance <= 1.5f && Input.GetKeyDown(KeyCode.W))
                    {
                        SceneManager.LoadScene("MapTest"); // 다른 씬으로 전환
                        Debug.Log("포탈 이동 제대로 됨.");
                    }
                }

            }
        }
        else //EventRoom == false;
        {
            if(isEventOn)//이벤트 사용되었을 때
            {
                spriteRenderer.enabled = true; // SpriteRenderer를 켭니다.
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                if (player != null)
                {
                    float distance = Vector3.Distance(transform.position, player.transform.position);
                    if (distance <= 1.0f && Input.GetKeyDown(KeyCode.W))
                    {
                        SceneManager.LoadScene("MapTest"); // 다른 씬으로 전환
                        Debug.Log("포탈 이동 제대로 됨.");
                    }
                }
            }
        }
    }

    public void setIsEventRoom(bool isEventRoom){
        this.isEventRoom = isEventRoom;
    }
    public void setIsEventOn(bool isEventOn){
        this.isEventOn = isEventOn;
    }
}
